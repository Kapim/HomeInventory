using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Http;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace HomeInventory.Desktop.Wpf.Services;

/// <summary>
/// Named server profiles (e.g. Test / Produkce) loaded from appsettings.json.
/// The active profile is persisted so the choice survives restarts, and switching
/// takes effect immediately via <see cref="DynamicBaseUrlHandler"/> (no restart needed).
/// </summary>
public interface IServerConfig
{
    string BaseUrl { get; }
    IReadOnlyList<string> ProfileNames { get; }
    string ActiveProfile { get; set; }
}

public class ServerConfig : IServerConfig
{
    private static readonly string StatePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "HomeInventory", "active-server.txt");

    private readonly Dictionary<string, string> _profiles;
    private string _active;

    public ServerConfig(IConfiguration config)
    {
        _profiles = config.GetSection("Servers:Profiles").Get<Dictionary<string, string>>()
                    ?? new Dictionary<string, string> { ["Test"] = "http://localhost:5046/" };

        var configured = config["Servers:Active"];
        var persisted = File.Exists(StatePath) ? File.ReadAllText(StatePath).Trim() : null;

        _active = new[] { persisted, configured }.FirstOrDefault(n => n is not null && _profiles.ContainsKey(n))
                  ?? _profiles.Keys.First();
    }

    public IReadOnlyList<string> ProfileNames => _profiles.Keys.ToList();

    public string ActiveProfile
    {
        get => _active;
        set
        {
            if (!_profiles.ContainsKey(value) || value == _active) return;
            _active = value;
            Directory.CreateDirectory(Path.GetDirectoryName(StatePath)!);
            File.WriteAllText(StatePath, value);
        }
    }

    public string BaseUrl
    {
        get
        {
            var url = _profiles[_active].Trim();
            return url.EndsWith('/') ? url : url + "/";
        }
    }
}

/// <summary>
/// Rewrites the placeholder base URL to the active server's URL on every request,
/// so changing the profile in the UI applies without recreating the HttpClients.
/// </summary>
public class DynamicBaseUrlHandler(IServerConfig config) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.RequestUri is not null)
        {
            var baseUri = new Uri(config.BaseUrl);
            var path = request.RequestUri.IsAbsoluteUri
                ? request.RequestUri.PathAndQuery
                : request.RequestUri.OriginalString;
            request.RequestUri = new Uri(baseUri, path.TrimStart('/'));
        }
        return base.SendAsync(request, ct);
    }
}

public static class WpfClientExtensions
{
    private static readonly Uri Placeholder = new("http://placeholder/");

    /// <summary>
    /// Registers the API clients with a dynamic base URL resolved from <see cref="IServerConfig"/>
    /// at request time (mirrors the MAUI setup).
    /// </summary>
    public static IServiceCollection AddHomeInventoryClientDynamic(this IServiceCollection services)
    {
        services.AddTransient<AuthHeaderHandler>();
        services.AddTransient<DynamicBaseUrlHandler>();

        services.AddHttpClient<IAuthApiClient, HttpAuthClient>(c => c.BaseAddress = Placeholder)
            .AddHttpMessageHandler<DynamicBaseUrlHandler>();

        services.AddHttpClient<IHouseholdsApiClient, HttpHouseholdsClient>(c => c.BaseAddress = Placeholder)
            .AddHttpMessageHandler<DynamicBaseUrlHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ILocationsApiClient, HttpLocationsClient>(c => c.BaseAddress = Placeholder)
            .AddHttpMessageHandler<DynamicBaseUrlHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IItemsApiClient, HttpItemsClient>(c => c.BaseAddress = Placeholder)
            .AddHttpMessageHandler<DynamicBaseUrlHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ITagsApiClient, HttpTagsClient>(c => c.BaseAddress = Placeholder)
            .AddHttpMessageHandler<DynamicBaseUrlHandler>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient<IHouseholdsService, HouseholdsService>();
        services.AddTransient<ILocationsService, LocationService>();
        services.AddTransient<IItemsService, ItemsService>();
        services.AddTransient<ITagsService, TagsService>();

        return services;
    }
}
