using System.Windows.Input;
using FindTheBug.Desktop.Reception.Commands;

namespace FindTheBug.Desktop.Reception.ViewModels;

public class ReceiptFormViewModel : BaseViewModel
{
    private string _receiptNumber = string.Empty;
    private string _patientName = string.Empty;
    private string _amount = string.Empty;
    private string _status = string.Empty;
    private readonly Action<object?>? _onLogout;
    private readonly Action<object?>? _onSyncData;

    public ReceiptFormViewModel(Action<object?>? onLogout = null, Action<object?>? onSyncData = null)
    {
        _onLogout = onLogout;
        _onSyncData = onSyncData;
        LogoutCommand = new RelayCommand(ExecuteLogout);
        SyncDataCommand = new RelayCommand(ExecuteSyncData);

        // Initialize with sample data
        InitializeSampleData();
    }

    public string ReceiptNumber
    {
        get => _receiptNumber;
        set => SetProperty(ref _receiptNumber, value);
    }

    public string PatientName
    {
        get => _patientName;
        set => SetProperty(ref _patientName, value);
    }

    public string Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public ICommand LogoutCommand { get; }

    public ICommand SyncDataCommand { get; }

    private void ExecuteLogout(object? parameter)
    {
        _onLogout?.Invoke(parameter);
    }

    private void ExecuteSyncData(object? parameter)
    {
        // In production, this would sync data with the server
        Status = "Syncing data...";
        System.Threading.Thread.Sleep(1000); // Simulate sync
        Status = "Data synced successfully at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _onSyncData?.Invoke(parameter);
    }

    private void InitializeSampleData()
    {
        ReceiptNumber = "RCT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
        PatientName = "Sample Patient";
        Amount = "$500.00";
        Status = "Ready";
    }
}