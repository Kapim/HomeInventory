using HomeInventory.Client.Auth;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Auth;
using HomeInventory.Mobile.Maui.Services;
using HomeInventory.Mobile.Maui.Services.Navigation;
using HomeInventory.Mobile.Maui.ViewModels;
using HomeInventory.Mobile.Maui.Views;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Mobile.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var services = builder.Services;

        // Infrastructure services
        services.AddSingleton<IServerConfig, ServerConfig>();
        services.AddSingleton<ISessionState, SessionState>();
        services.AddSingleton<INavigationService, MauiNavigationService>();
        services.AddSingleton<IDialogService, MauiDialogService>();
        services.AddSingleton<IBusyService, BusyService>();
        services.AddSingleton<INotificationsService, MauiNotificationsService>();
        services.AddSingleton<ITokenStore, SecureStorageTokenStore>();
        services.AddSingleton<IErrorLocalizer, ErrorLocalizerService>();

        // HTTP client — URL je konfigurovatelná v UI, uložená v Preferences
        services.AddHomeInventoryClientDynamic();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<HouseholdsViewModel>();
        services.AddSingleton<LocationsViewModel>();
        services.AddSingleton<ItemsSearchViewModel>();
        services.AddTransient<ItemsViewModel>();
        services.AddTransient<LocationDetailViewModel>();

        // Shell & pages
        services.AddSingleton<AppShell>();
        services.AddTransient<LoginPage>();
        services.AddTransient<HouseholdsPage>();
        services.AddSingleton<LocationsPage>();
        services.AddSingleton<ItemsSearchPage>();
        services.AddTransient<ItemsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
