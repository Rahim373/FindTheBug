using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.Services.CloudSync;
using System.Windows.Input;
using System.Windows.Media;

namespace FindTheBug.Desktop.Reception.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private object? _currentView;
    private bool _isLoggedIn;
    private string _selectedMenuItem = string.Empty;
    private readonly SyncState _syncState;

    private const string MENU_HOME = "Home";
    private const string MENU_DOCTORS = "Doctors";
    private const string MENU_RECEIPTS = "Receipts";

    public MainWindowViewModel(SyncState syncState)
    {
        _syncState = syncState;

        // Subscribe to sync state changes to trigger property updates
        _syncState.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(SyncState.IsSyncing):
                    OnPropertyChanged(nameof(IsSyncing));
                    break;
                case nameof(SyncState.LastSyncTime):
                    OnPropertyChanged(nameof(LastSyncTimeFormatted));
                    OnPropertyChanged(nameof(SyncStatusText));
                    break;
                case nameof(SyncState.LastSyncSuccess):
                case nameof(SyncState.ErrorMessage):
                    OnPropertyChanged(nameof(HasSyncError));
                    OnPropertyChanged(nameof(SyncStatusText));
                    break;
                case nameof(SyncState.IsCloudOnline):
                    OnPropertyChanged(nameof(CloudOnlineColor));
                    break;
            }
        };

        // Initialize with Login view
        var loginViewModel = new LoginViewModel(OnLoginSuccess);
        _currentView = new Views.LoginView { DataContext = loginViewModel };
        _isLoggedIn = false;
        _selectedMenuItem = string.Empty;

        // Setup commands
        HomeCommand = new RelayCommand(ExecuteNavigateToHome, CanExecuteMenuCommands);
        ReceiptsCommand = new RelayCommand(ExecuteNavigateToReceipts, CanExecuteMenuCommands);
        DoctorsCommand = new RelayCommand(ExecuteNavigateToDoctors, CanExecuteMenuCommands);
        LogoutCommand = new RelayCommand(ExecuteLogout, CanExecuteMenuCommands);
    }

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public string SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }

    public ICommand HomeCommand { get; }

    public ICommand ReceiptsCommand { get; }

    public ICommand DoctorsCommand { get; }

    public ICommand LogoutCommand { get; }

    /// <summary>
    /// Gets whether a sync is currently in progress
    /// </summary>
    public bool IsSyncing => _syncState.IsSyncing;

    /// <summary>
    /// Gets whether there is a sync error
    /// </summary>
    public bool HasSyncError => _syncState.HasError;

    /// <summary>
    /// Shows if the cloud is online
    /// </summary>
    public Color CloudOnlineColor => _syncState.IsCloudOnline ? Colors.LimeGreen : Colors.Gray;

    /// <summary>
    /// Gets a user-friendly sync status text
    /// </summary>
    public string SyncStatusText => _syncState.SyncStatusText;

    /// <summary>
    /// Gets the last sync time in formatted string
    /// </summary>
    public string LastSyncTimeFormatted => _syncState.LastSyncTimeFormatted;

    private void OnLoginSuccess()
    {
        IsLoggedIn = true;
        ExecuteNavigateToHome(null);
    }

    private void ExecuteNavigateToHome(object? parameter)
    {
        if (SelectedMenuItem == MENU_HOME)
            return;
        SelectedMenuItem = MENU_HOME;
        var receiptViewModel = new ReceiptFormViewModel();
        _currentView = new Views.ReceiptView { DataContext = receiptViewModel };
        OnPropertyChanged(nameof(CurrentView));
    }

    private void ExecuteNavigateToReceipts(object? parameter)
    {
        if (SelectedMenuItem == MENU_RECEIPTS)
            return;

        SelectedMenuItem = MENU_RECEIPTS;
        var receiptsViewModel = new ReceiptsViewModel();
        _currentView = new Views.ReceiptsView { DataContext = receiptsViewModel };
        OnPropertyChanged(nameof(CurrentView));
    }

    private void ExecuteNavigateToDoctors(object? parameter)
    {
        if (SelectedMenuItem == MENU_DOCTORS)
            return;

        SelectedMenuItem = MENU_DOCTORS;
        var doctorsViewModel = new DoctorsViewModel();
        _currentView = new Views.DoctorsView { DataContext = doctorsViewModel };
        OnPropertyChanged(nameof(CurrentView));
    }

    private void ExecuteLogout(object? parameter)
    {
        IsLoggedIn = false;
        SelectedMenuItem = string.Empty;
        
        // Navigate back to Login view
        var loginViewModel = new LoginViewModel(OnLoginSuccess);
        _currentView = new Views.LoginView { DataContext = loginViewModel };
        OnPropertyChanged(nameof(CurrentView));
    }

    private bool CanExecuteMenuCommands(object? parameter)
    {
        return IsLoggedIn;
    }
}