using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FindTheBug.Common.Services;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Models;
using FindTheBug.Desktop.Reception.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace FindTheBug.Desktop.Reception.ViewModels;

public partial class ReceiptFormViewModel : ObservableObject
{
    const string CREATE_NEW_RECEIPT = "Create New Receipt";

    #region Fields

    [ObservableProperty]
    private ObservableCollection<DropdownOption> _tests;

    [ObservableProperty]
    private ObservableCollection<DropdownOption> _doctors;

    [ObservableProperty]
    private ObservableCollection<LabTestDto> _selectedTests;

    [ObservableProperty]
    private string pageTitle;

    public Task InitializationTask { get; private set; }

    public bool IsSaved => !string.IsNullOrEmpty(ReceiptInfo.InvoiceNumber);

    public ReceiptInformation ReceiptInfo { get; private set; }

    public TestInformation TestInfo { get; private set; }

    #endregion

    public ReceiptFormViewModel()
    {
        PageTitle = CREATE_NEW_RECEIPT;
        Doctors = new ObservableCollection<DropdownOption>
        {
            new DropdownOption(Guid.Empty, "Self")
        };

        Tests = new ObservableCollection<DropdownOption>();
        SelectedTests = new ObservableCollection<LabTestDto>();
        SelectedTests.CollectionChanged += SelectedTests_CollectionChanged;

        // Initialize information models
        ReceiptInfo = new ReceiptInformation();
        TestInfo = new TestInformation();

        InitializationTask = Task.WhenAll([
            LoadDoctorsAsync(),
            LoadDiagnosticsTestAsync()
        ]);
    }

    private void SelectedTests_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        var subTotal = 0m;
        var discount = 0m;

        foreach (var test in SelectedTests)
        {
            subTotal += test.Total;
            discount += test.Discount;
        }

        ReceiptInfo.SubTotal.Value = Math.Round(subTotal, 0);
        ReceiptInfo.Discount.Value = Math.Round(discount, 0);
        ReceiptInfo.Total.Value = ReceiptInfo.SubTotal.Value - ReceiptInfo.Discount.Value;
    }

    private async Task LoadDoctorsAsync()
    {
        var doctors = await DbAccess.GetAllDoctorsForDropdownAsync();
        doctors.ForEach(Doctors.Add);
    }

    private async Task LoadDiagnosticsTestAsync()
    {
        var tests = await DbAccess.GetAllDiagnosticsTestForDropdownAsync();
        tests.ForEach(Tests.Add);
    }

    [RelayCommand]
    private void Print()
    {
        var reportService = new ReportService();
        var data = reportService.GetInvoiceData(null, null);
        var reportPath = reportService.GetReportPath(ReportType.LabTestInvoice);
        ReportHelper.OpenReport(reportPath, data.DataSources, data.ReportParameters, "Lab Invoice");
    }

    [RelayCommand]
    private void AddTest()
    {
        // Validate test fields before adding
        if (!TestInfo.ValidateAll())
        {
            return;
        }

        SelectedTests.Add(new LabTestDto
        {
            Id = TestInfo.Id.Value,
            Name = TestInfo.TestName.Value ?? string.Empty,
            Amount = TestInfo.TestAmount.Value,
            Discount = TestInfo.TestDiscount.Value
        });

        OnPropertyChanged(nameof(Tests));

        // Reset test fields
        TestInfo.ClearAll();
    }

    [RelayCommand]
    private void DueChanged()
    {
        ReceiptInfo.Balance.Value = ReceiptInfo.Total.Value - ReceiptInfo.Due.Value;
    }

    [RelayCommand]
    private async Task TestSelectionChangedAsync()
    {
        if (TestInfo.Id.IsValid)
        {
            var test = await DbAccess.GetDiagnosticsTestByIdAsync(TestInfo.Id.Value);
            if (test is not null) {
                TestInfo.TestAmount.Value = test.Price;
                TestInfo.TestDiscount.Value = 0M;
                TestInfo.TestName.Value = test.TestName;
            }
        }
    }

    [RelayCommand]
    private void Reset()
    {
        MessageBoxResult messageBoxResult = MessageBox.Show(
            "Are you sure you want to reset this receipt?",
            "Reset Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (messageBoxResult == MessageBoxResult.Yes)
        {
            ReceiptInfo.ClearAll();
            TestInfo.ClearAll();
            SelectedTests.Clear();
            OnPropertyChanged(nameof(Tests));
            PageTitle = CREATE_NEW_RECEIPT;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SaveAsync()
    {
        // Validate all patient information
        if (!ReceiptInfo.ValidateAll())
        {
            return;
        }

        if (!SelectedTests.Any())
        {
            MessageBox.Show("Please add at least one test to the receipt.", "No Tests Added", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBoxResult messageBoxResult = MessageBox.Show(
            "Are you sure you want to save receipt? Once confirmed, nothing can modify.",
            "Confirm Save",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (messageBoxResult == MessageBoxResult.Yes)
        {
            try
            {
                // Save the receipt to database
                var receiptId = await DbAccess.SaveReceiptAsync(ReceiptInfo, SelectedTests.ToList());
                
                // Set the invoice number
                var dbContext = App.ServiceProvider?.GetService(typeof(Data.ReceptionDbContext)) as Data.ReceptionDbContext;
                if (dbContext != null)
                {
                    var savedReceipt = await dbContext.LabReceipts.FindAsync(receiptId);
                    if (savedReceipt != null)
                    {
                        ReceiptInfo.InvoiceNumber = savedReceipt.InvoiceNumber;
                    }
                }

                // Update page title with invoice number
                PageTitle = $"Receipt {ReceiptInfo.InvoiceNumber}";

                MessageBox.Show($"Receipt saved successfully!\nInvoice Number: {ReceiptInfo.InvoiceNumber}", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving receipt: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void CreateNew()
    {
        Reset();
        PageTitle = CREATE_NEW_RECEIPT;
    }

    /// <summary>
    /// Forces validation on all fields
    /// </summary>
    public void ForceValidateAll()
    {
        ReceiptInfo.ForceValidateAll();
        TestInfo.ForceValidateAll();
    }

    private bool CanReset()
    {
        return SelectedTests.Any() || ReceiptInfo.HasValue();
    }

    private bool CanSave()
    {
        return SelectedTests.Any() && ReceiptInfo.ValidateAll();
    }
}