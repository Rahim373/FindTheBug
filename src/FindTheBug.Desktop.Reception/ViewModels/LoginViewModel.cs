using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using System.Windows.Input;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _phoneNumber = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private readonly Action? _onLoginSuccess;

        public LoginViewModel(Action? onLoginSuccess = null)
        {
            _onLoginSuccess = onLoginSuccess;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(PhoneNumber) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object? parameter)
        {
            var success = DbAccess.CheckLogin(PhoneNumber, Password).GetAwaiter().GetResult();
            if (success)
            {
                ErrorMessage = string.Empty;
                _onLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "Invalid username or password. Try: admin / admin123";
            }
        }
    }
}