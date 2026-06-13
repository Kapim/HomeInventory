using HomeInventory.Client.Auth;
using HomeInventory.Mobile.Maui.Services;
using HomeInventory.Mobile.Maui.Views;

namespace HomeInventory.Mobile.Maui;

public partial class AppShell : Shell
{
    private readonly ITokenStore _tokenStore;
    private readonly ISessionState _session;

    public AppShell(ITokenStore tokenStore, ISessionState session)
    {
        _tokenStore = tokenStore;
        _session = session;
        InitializeComponent();

        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("households", typeof(HouseholdsPage));
        Routing.RegisterRoute("items", typeof(ItemsPage));
        Routing.RegisterRoute("locationdetail", typeof(LocationDetailPage));

        Loaded += OnShellLoaded;
    }

    private async void OnShellLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnShellLoaded;
        try
        {
            var token = await _tokenStore.LoadAsync();
            if (string.IsNullOrEmpty(token))
                await GoToAsync("login");
            else if (_session.SelectedHouseholdId is not null)
                await GoToAsync("//main");
            else
                await GoToAsync("households");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AppShell] OnShellLoaded failed: {ex}");
            await DisplayAlertAsync("Chyba spuštění", ex.Message, "OK");
        }
    }
}
