using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using System.Diagnostics;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class LoginViewModel(INavigationService nav, IAuthService auth, IDialogService dialogs) : ObservableObject 
    {
        private readonly INavigationService _nav = nav;
        private readonly IAuthService _auth = auth;
        private readonly IDialogService _dialogs = dialogs;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string userName = "";
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string password = "";

        [RelayCommand(CanExecute = nameof(CanLogin))]
        public async Task Login()
        {
            try
            {
                var result = await _auth.LoginAsync(UserName, Password);
                Debug.WriteLine(result);
                _nav.NavigateTo<ItemsSearchViewModel>();
            } catch (InvalidCredentialsException)
            {
                _dialogs.ShowInfo("Přihlášení selhalo", "Špatné jméno nebo heslo.");
            } catch (ApiUnavailableException)
            {
                _dialogs.ShowInfo("Přihlášení selhalo", "Server není dostupný. Zkus to prosím později.");
            } catch (Exception ex)
            {
                _dialogs.ShowInfo("Přihlášení selhalo", "Neočekáváná chyba: " + ex.Message);
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
