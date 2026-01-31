using HomeInventory.Client.Auth;
using HomeInventory.Client.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Client;

public static class ClientServiceCollectionExtensions
{
    public static IServiceCollection AddHomeInventoryClient(
        this IServiceCollection services,
        Uri baseAddress)
    {
        services.AddTransient<AuthHeaderHandler>();

        services.AddHttpClient<IAuthApiClient, HttpAuthService>(c =>
        {
            c.BaseAddress = baseAddress;
        });

        services.AddSingleton<IAuthService, AuthService>();


        return services;
    }
}
