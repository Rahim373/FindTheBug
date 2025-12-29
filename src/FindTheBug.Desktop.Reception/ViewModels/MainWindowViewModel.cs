using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.Messages;
using FindTheBug.Desktop.Reception.Services.CloudSync;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Desktop.Reception.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _selectedMenuItem = string.Empty;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private SyncStateEnum _syncStatus = SyncStateEnum.NotStarted;

    private const string MENU_HOME = "Home";
    private const string MENU_DOCTORS = "Doctors";
    private const string MENU_RECEIPTS = "Receipts";

    public MainWindowViewModel()
    {
        WeakReferenceMessenger.Default.Register<SyncStatusUpdateMessage>(this, (r, m) =>
        {
            SyncStatus = m.state;
        });

        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) =>
        {
            Home(null);
            SetUserName(m.user);
            IsLoggedIn = true;
        });

        // Initialize with Login view
        var loginViewModel = new LoginViewModel();
        _currentView = new Views.LoginView { DataContext = loginViewModel };
        _isLoggedIn = false;
        _selectedMenuItem = string.Empty;
    }

    [RelayCommand]
    private void Home(object? parameter)
    {
        if (SelectedMenuItem == MENU_HOME)
            return;
        SelectedMenuItem = MENU_HOME;
        var receiptViewModel = new ReceiptFormViewModel();
        CurrentView = new Views.ReceiptView { DataContext = receiptViewModel };
    }

    [RelayCommand]
    private void Receipts(object? parameter)
    {
        if (SelectedMenuItem == MENU_RECEIPTS)
            return;

        SelectedMenuItem = MENU_RECEIPTS;
        var receiptsViewModel = new ReceiptsViewModel();
        CurrentView = new Views.ReceiptsView { DataContext = receiptsViewModel };
    }

    [RelayCommand]
    private void Doctors(object? parameter)
    {
        if (SelectedMenuItem == MENU_DOCTORS)
            return;

        SelectedMenuItem = MENU_DOCTORS;
        var doctorsViewModel = new DoctorsViewModel();
        CurrentView = new Views.DoctorsView { DataContext = doctorsViewModel };
    }

    [RelayCommand]
    private void Logout(object? parameter)
    {
        App.CurrentUser = null;
        IsLoggedIn = false;
        SelectedMenuItem = string.Empty;

        var loginViewModel = new LoginViewModel();
        CurrentView = new Views.LoginView { DataContext = loginViewModel };
    }

    private void SetUserName(User? user)
    {
        if (user is not null)
            UserName = $"{user.FirstName} {user.LastName} ({user.Phone})".Trim();
    }
}