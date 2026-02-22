using HomeInventory.Client.Auth;
using HomeInventory.Client.Http;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Client;

public static class ClientServiceCollectionExtensions
{
    public static IServiceCollection AddHomeInventoryClient(
        this IServiceCollection services,
        Uri baseAddress)
    {
        services.AddTransient<AuthHeaderHandler>();

        services.AddHttpClient<IAuthApiClient, HttpAuthClient>(c =>
        {
            c.BaseAddress = baseAddress;
        });

        services.AddHttpClient<IHouseholdsApiClient, HttpHouseholdsClient>(c =>
        {
            c.BaseAddress = baseAddress;
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient<IHouseholdsService, HouseholdsService>();


        return services;
    }
}
