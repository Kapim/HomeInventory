using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using System.Diagnostics;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class LoginViewModel(INavigationService nav, IAuthService auth, IDialogService dialogs, IErrorLocalizer errorLocalizer) : ObservableObject 
    {
        private readonly INavigationService _nav = nav;
        private readonly IAuthService _auth = auth;
        private readonly IDialogService _dialogs = dialogs;
        private readonly IErrorLocalizer _errorLocalizer = errorLocalizer;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string userName = "Kapi";
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string password = "heslo";

        [RelayCommand(CanExecute = nameof(CanLogin))]
        public async Task Login()
        {
            try
            {
                var result = await _auth.LoginAsync(UserName, Password);
                Debug.WriteLine(result);
            }  catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            }

            await _nav.NavigateTo<MainViewModel>();
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
