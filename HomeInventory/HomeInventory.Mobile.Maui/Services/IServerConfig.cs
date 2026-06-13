namespace HomeInventory.Mobile.Maui.Services;

public interface IServerConfig
{
    string BaseUrl { get; set; }
}

public class ServerConfig : IServerConfig
{
    private const string PrefKey = "server_url";
    private const string DefaultUrl = "http://192.168.1.1:5046/";

    public string BaseUrl
    {
        get => Preferences.Default.Get(PrefKey, DefaultUrl);
        set
        {
            var url = value.Trim().TrimEnd('/') + "/";
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "http://" + url;
            Preferences.Default.Set(PrefKey, url);
        }
    }
}
