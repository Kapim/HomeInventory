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
    public partial class LoginViewModel(INavigationService nav, IAuthService auth, IDialogService dialogs, IErrorLocalizer errorLocalizer, IServerConfig serverConfig, ITokenStore tokenStore) : ObservableObject
    {
        private readonly INavigationService _nav = nav;
        private readonly IAuthService _auth = auth;
        private readonly IDialogService _dialogs = dialogs;
        private readonly IErrorLocalizer _errorLocalizer = errorLocalizer;
        private readonly IServerConfig _serverConfig = serverConfig;
        private readonly ITokenStore _tokenStore = tokenStore;

        [ObservableProperty]
        private bool stayLoggedIn = true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string userName = "Kapi";
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string password = "heslo";

        public IReadOnlyList<string> Servers => _serverConfig.ProfileNames;

        [ObservableProperty]
        private string selectedServer = serverConfig.ActiveProfile;

        partial void OnSelectedServerChanged(string value) => _serverConfig.ActiveProfile = value;

        [RelayCommand(CanExecute = nameof(CanLogin))]
        public async Task Login()
        {
            try
            {
                _tokenStore.Persist = StayLoggedIn;
                var result = await _auth.LoginAsync(UserName, Password);
                await _nav.NavigateTo<MainViewModel>();
            }  catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                if (ex.Type == ApiErrorTypes.Network)
                {
                    // Surface the target server + underlying reason so connection issues are diagnosable.
                    message += $"\n\nServer: {_serverConfig.BaseUrl}";
                    if (ex.InnerException is not null)
                        message += $"\nDetail: {ex.InnerException.Message}";
                }
                _dialogs.ShowError("Operace selhala", message);
            }

        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
