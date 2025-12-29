using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FindTheBug.Common.Services;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Messages;
using FindTheBug.Desktop.Reception.Models;
using FindTheBug.Desktop.Reception.Utils;
using FindTheBug.Desktop.Reception.Windows;
using FindTheBug.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace FindTheBug.Desktop.Reception.ViewModels;

public partial class ReceiptFormViewModel : ObservableObject
{
    const string CREATE_NEW_RECEIPT = "Create New Receipt";
    private readonly ReportService _reportService;

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

    public bool IsSaved => !string.IsNullOrEmpty(PatientInfo.InvoiceNumber);

    public PatientInformation PatientInfo { get; private set; }

    public TestInformation TestInfo { get; private set; }

    [ObservableProperty]
    private decimal _subTotal;
    [ObservableProperty]
    private decimal _discount;
    [ObservableProperty]
    private decimal _total;
    [ObservableProperty]
    private decimal _due;
    [ObservableProperty]
    private decimal _balance;

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
        PatientInfo = new PatientInformation();
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

        SubTotal = Math.Round(subTotal, 0);
        Discount = Math.Round(discount, 0);
        Total = SubTotal - Discount;
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
        Balance = SubTotal - Due;
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
            PatientInfo.ClearAll();
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
        if (!PatientInfo.ValidateAll())
        {
            return;
        }


        if (!SelectedTests.Any())
        {
            return;
        }
        PatientInfo.InvoiceNumber = "MARC2502-0000012";
        await DbAccess.SaveReceiptAsync(PatientInfo, TestInfo);
        PageTitle = $"Receipt : {PatientInfo.InvoiceNumber}";
        OnPropertyChanged(nameof(IsSaved));
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
        PatientInfo.ForceValidateAll();
        TestInfo.ForceValidateAll();
    }

    private bool CanReset()
    {
        return SelectedTests.Any() || PatientInfo.HasValue();
    }

    private bool CanSave()
    {
        return SelectedTests.Any() && PatientInfo.ValidateAll();
    }
}