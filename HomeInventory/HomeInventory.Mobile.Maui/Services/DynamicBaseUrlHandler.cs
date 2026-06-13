namespace HomeInventory.Mobile.Maui.Services;

/// <summary>
/// Replaces the placeholder base URL with the runtime-configured server URL on every request.
/// HttpClients are registered with BaseAddress = "http://placeholder/" and this handler
/// rewrites it to whatever IServerConfig.BaseUrl returns at the time of the request.
/// </summary>
public class DynamicBaseUrlHandler(IServerConfig config) : DelegatingHandler
{
    private static readonly Uri Placeholder = new("http://placeholder/");

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
