using CommunityToolkit.Mvvm.ComponentModel;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FindTheBug.Desktop.Reception.ViewModels;

public partial class ReceiptFormViewModel : ObservableObject
{
    #region Fields

    private ObservableCollection<LabTestDto> _tests;
    private ObservableCollection<DoctorItem> _doctors = new ObservableCollection<DoctorItem>();
    public bool IsSaved => !string.IsNullOrEmpty(InvoiceNumber);
    public PatientInformation PatientInfo { get; private set; }
    public TestInformation TestInfo { get; private set; }
    
    [ObservableProperty]
    private string _invoiceNumber; 

    #endregion

    #region Commands

    public ICommand AddTestCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SaveCommand { get; } 

    #endregion


    public ObservableCollection<DoctorItem> Doctors
    {
        get => _doctors;
        set => SetProperty(ref _doctors, value);
    }
    public ObservableCollection<LabTestDto> Tests
    {
        get => _tests;
        set => SetProperty(ref _tests, value);
    }

    public ReceiptFormViewModel(Action<object?>? onLogout = null, Action<object?>? onSyncData = null)
    {
        AddTestCommand = new RelayCommand(AddTest);
        PrintCommand = new RelayCommand(PrintInvoice);
        ResetCommand = new RelayCommand(ResetForm, CanReset);
        SaveCommand = new RelayCommand(SaveReceipt, CanSave);
        Tests = new ObservableCollection<LabTestDto>();

        // Initialize information models
        PatientInfo = new PatientInformation();
        TestInfo = new TestInformation();

        //LoadDoctors();
    }

    private void PrintInvoice(object? obj)
    {
        MessageBox.Show("Print");
    }

    private void AddTest(object? parameter)
    {
        // Validate test fields before adding
        if (!TestInfo.ValidateAll())
        {
            return;
        }

        Tests.Add(new LabTestDto
        {
            Name = TestInfo.TestName.Value ?? string.Empty,
            Amount = TestInfo.TestAmount.Value,
            Discount = TestInfo.TestDiscount.Value
        });

        OnPropertyChanged(nameof(Tests));

        // Reset test fields
        TestInfo.ClearAll();
    }

    private bool CanReset(object? parameter)
    {
        return Tests.Any() || PatientInfo.HasValue();
    }

    private bool CanSave(object? parameter)
    {
        return Tests.Any() && PatientInfo.ValidateAll();
    }

    private void ResetForm(object? parameter)
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
            Tests.Clear();
            InvoiceNumber = string.Empty;
            OnPropertyChanged(nameof(Tests));
        }
    }

    private void SaveReceipt(object? parameter)
    {
        // Validate all patient information
        if (!PatientInfo.ValidateAll())
        {
            return;
        }

        // Validate that at least one test is added
        if (!Tests.Any())
        {
            // TODO: Show message to user
            return;
        }

        // TODO: Implement save logic
        // Here you would:
        // 1. Create the receipt entity
        // 2. Save to local database
        // 3. Sync with cloud (if needed)
        // 4. Show success message
    }

    /// <summary>
    /// Forces validation on all fields
    /// </summary>
    public void ForceValidateAll()
    {
        PatientInfo.ForceValidateAll();
        TestInfo.ForceValidateAll();
    }
}