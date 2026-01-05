using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Desktop.Reception.Messages;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _phoneNumber = "01734014433";

        [ObservableProperty]
        private string _password = "SuperSecretPassword123!";

        [ObservableProperty]
        private string _errorMessage = string.Empty;


        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task LoginAsync()
        {
            if (!CanExecuteLogin())
            {
                ErrorMessage = "Invalid phone number or password.";
            }

            var user = await DbAccess.CheckLoginAsync(PhoneNumber, Password);
            if (user is null)
            {
                ErrorMessage = "Invalid username or password";
                return;
            }

            ErrorMessage = string.Empty;
            App.CurrentUser = user;
            WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(user));
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(PhoneNumber) && !string.IsNullOrWhiteSpace(Password);
        }

    }
}