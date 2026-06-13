using HomeInventory.Client.Auth;

namespace HomeInventory.Mobile.Maui.Auth;

public class SecureStorageTokenStore : ITokenStore
{
    private const string TokenKey = "auth_token";

    public async Task SaveAsync(string token, CancellationToken ct = default)
        => await SecureStorage.Default.SetAsync(TokenKey, token);

    public async Task<string?> LoadAsync(CancellationToken ct = default)
    {
        try { return await SecureStorage.Default.GetAsync(TokenKey); }
        catch { return null; }
    }

    public Task ClearAsync(CancellationToken ct = default)
    {
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }
}
