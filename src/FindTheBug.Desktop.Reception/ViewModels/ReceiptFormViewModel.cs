using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Models;
using FindTheBug.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FindTheBug.Desktop.Reception.ViewModels;

public partial class ReceiptFormViewModel : ObservableObject
{
    #region Fields

    [ObservableProperty]
    private ObservableCollection<LabTestDto> _tests;

    [ObservableProperty]
    private ObservableCollection<DoctorItem> _doctors;

    public bool IsSaved => !string.IsNullOrEmpty(PatientInfo.InvoiceNumber);
    
    public PatientInformation PatientInfo { get; private set; }
    
    public TestInformation TestInfo { get; private set; }

    #endregion

    public ReceiptFormViewModel()
    {
        Doctors = new ObservableCollection<DoctorItem>
        {
            new DoctorItem{ Name = "Self", Id = string.Empty}
        };

        Tests = new ObservableCollection<LabTestDto>();

        // Initialize information models
        PatientInfo = new PatientInformation();
        TestInfo = new TestInformation();

        LoadDoctors();
    }

    private void LoadDoctors()
    {
        var doctors = DbAccess.GetAllDoctorsAsync().GetAwaiter().GetResult();

        foreach (var doctor in doctors)
        {
            Doctors.Add(new DoctorItem
            {
                Id = doctor.Id.ToString(),
                Name = $"{doctor.Name} ({doctor.Degree})"
            });
        }
    }

    [RelayCommand]
    private void Print()
    {
        MessageBox.Show("Print");
    }

    [RelayCommand]
    private void AddTest()
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

    [RelayCommand(CanExecute = nameof(CanReset))]
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
            Tests.Clear();
            OnPropertyChanged(nameof(Tests));
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

        
        if (!TestInfo.ValidateAll())
        {
            return;
        }

        await DbAccess.SaveReceiptAsync(PatientInfo, TestInfo);
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
        return Tests.Any() || PatientInfo.HasValue();
    }
    
    private bool CanSave()
    {
        return Tests.Any() && PatientInfo.ValidateAll();
    }
}