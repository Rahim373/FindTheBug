using System.Collections.ObjectModel;
using System.Windows.Input;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.Dtos;

namespace FindTheBug.Desktop.Reception.ViewModels;

public class ReceiptFormViewModel : BaseViewModel
{
    private ObservableCollection<LabTestDto> _tests;


    public ICommand AddTestCommand { get; }

    public ReceiptFormViewModel(Action<object?>? onLogout = null, Action<object?>? onSyncData = null)
    {
        AddTestCommand = new RelayCommand(AddTest);
        Tests = new ObservableCollection<LabTestDto>();


    }

    public ObservableCollection<LabTestDto> Tests
    {
        get => _tests;
        set => SetProperty(ref _tests, value);
    }

    private void AddTest(object? parameter)
    {
        Tests.Add(new LabTestDto
        {
            Name = "X-Ray",
            Amount = 1000,
            Discount = 0
        });

        OnPropertyChanged(nameof(Tests));
    }

    private void InitializeSampleData()
    {
        
    }
}