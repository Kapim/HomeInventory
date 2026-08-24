using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using HomeInventory.Mobile.Maui.Services.Navigation;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class LoginViewModel(
    INavigationService nav,
    IAuthService auth,
    IDialogService dialogs,
    IErrorLocalizer errorLocalizer,
    IServerConfig serverConfig,
    ITokenStore tokenStore) : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string userName = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string serverUrl = serverConfig.BaseUrl;

    [ObservableProperty]
    private bool stayLoggedIn = true;

    [ObservableProperty]
    private bool isBusy;

    partial void OnServerUrlChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            serverConfig.BaseUrl = value;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        IsBusy = true;
        try
        {
            tokenStore.Persist = StayLoggedIn;
            await auth.LoginAsync(UserName, Password);
            await nav.NavigateTo<HouseholdsViewModel>();
        }
        catch (ApiException ex)
        {
            var message = errorLocalizer.GetString(ex.Type);
            dialogs.ShowError("Přihlášení selhalo", message);
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Přihlášení selhalo", $"Nelze se připojit k serveru.\n{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanLogin()
        => !string.IsNullOrWhiteSpace(UserName)
           && !string.IsNullOrWhiteSpace(Password)
           && !string.IsNullOrWhiteSpace(ServerUrl);
}
