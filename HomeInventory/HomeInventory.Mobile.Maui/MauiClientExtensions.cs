using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Http;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Mobile.Maui;

/// <summary>
/// Registers HTTP clients with a dynamic base URL read from IServerConfig at request time.
/// Uses DynamicBaseUrlHandler to rewrite the placeholder base address per-request,
/// so changing the server URL in the UI takes effect immediately without app restart.
/// </summary>
public static class MauiClientExtensions
{
    private static readonly Uri Placeholder = new("http://placeholder/");

    public static IServiceCollection AddHomeInventoryClientDynamic(this IServiceCollection services)
    {
        services.AddTransient<AuthHeaderHandler>();
        services.AddTransient<DynamicBaseUrlHandler>();

        services.AddHttpClient<IAuthApiClient, HttpAuthClient>(c =>
        {
            c.BaseAddress = Placeholder;
        }).AddHttpMessageHandler<DynamicBaseUrlHandler>();

        services.AddHttpClient<IHouseholdsApiClient, HttpHouseholdsClient>(c =>
        {
            c.BaseAddress = Placeholder;
        })
        .AddHttpMessageHandler<DynamicBaseUrlHandler>()
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ILocationsApiClient, HttpLocationsClient>(c =>
        {
            c.BaseAddress = Placeholder;
        })
        .AddHttpMessageHandler<DynamicBaseUrlHandler>()
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IItemsApiClient, HttpItemsClient>(c =>
        {
            c.BaseAddress = Placeholder;
        })
        .AddHttpMessageHandler<DynamicBaseUrlHandler>()
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient<IHouseholdsService, HouseholdsService>();
        services.AddTransient<ILocationsService, LocationService>();
        services.AddTransient<IItemsService, ItemsService>();

        return services;
    }
}
