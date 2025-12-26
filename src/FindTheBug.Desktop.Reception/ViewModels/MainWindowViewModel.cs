using System.Windows.Input;
using FindTheBug.Desktop.Reception.Commands;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private object? _currentView;
        private bool _isLoggedIn;
        private string _selectedMenuItem = string.Empty;

        public MainWindowViewModel()
        {
            // Initialize with Login view
            var loginViewModel = new LoginViewModel(OnLoginSuccess);
            _currentView = new Views.LoginView { DataContext = loginViewModel };
            _isLoggedIn = false;
            _selectedMenuItem = string.Empty;

            // Setup commands
            HomeCommand = new RelayCommand(ExecuteNavigateToHome, CanExecuteMenuCommands);
            ReceiptsCommand = new RelayCommand(ExecuteNavigateToReceipts, CanExecuteMenuCommands);
            DoctorsCommand = new RelayCommand(ExecuteNavigateToDoctors, CanExecuteMenuCommands);
            SyncDataCommand = new RelayCommand(ExecuteSyncData, CanExecuteMenuCommands);
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

        public ICommand SyncDataCommand { get; }

        public ICommand LogoutCommand { get; }

        private void OnLoginSuccess()
        {
            IsLoggedIn = true;
            ExecuteNavigateToHome(null);
        }

        private void ExecuteNavigateToHome(object? parameter)
        {
            SelectedMenuItem = "Home";
            var receiptViewModel = new ReceiptFormViewModel(ExecuteLogout, ExecuteSyncDataFromMenu);
            _currentView = new Views.ReceiptView { DataContext = receiptViewModel };
            OnPropertyChanged(nameof(CurrentView));
        }

        private void ExecuteNavigateToReceipts(object? parameter)
        {
            SelectedMenuItem = "Receipts";
            var receiptsViewModel = new ReceiptsViewModel();
            _currentView = new Views.ReceiptsView { DataContext = receiptsViewModel };
            OnPropertyChanged(nameof(CurrentView));
        }

        private void ExecuteNavigateToDoctors(object? parameter)
        {
            SelectedMenuItem = "Doctors";
            var doctorsViewModel = new DoctorsViewModel();
            _currentView = new Views.DoctorsView { DataContext = doctorsViewModel };
            OnPropertyChanged(nameof(CurrentView));
        }

        private void ExecuteSyncData(object? parameter)
        {
            // Sync data functionality
            var currentViewViewModel = (_currentView as Views.ReceiptView)?.DataContext as ReceiptFormViewModel;
            if (currentViewViewModel is not null)
            {
                currentViewViewModel.SyncDataCommand.Execute(null);
            }
        }

        private void ExecuteSyncDataFromMenu(object? parameter)
        {
            // Sync data called from menu
            // This can be expanded to show a message or update status
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
}