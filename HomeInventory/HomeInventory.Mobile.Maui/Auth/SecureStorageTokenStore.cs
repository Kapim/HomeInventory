using HomeInventory.Client.Auth;

namespace HomeInventory.Mobile.Maui.Auth;

public class SecureStorageTokenStore : ITokenStore
{
    private const string TokenKey = "auth_token";

    private string? _memory;

    public bool Persist { get; set; } = true;

    public async Task SaveAsync(string token, CancellationToken ct = default)
    {
        _memory = token;
        if (Persist)
            await SecureStorage.Default.SetAsync(TokenKey, token);
        else
            SecureStorage.Default.Remove(TokenKey);
    }

    public async Task<string?> LoadAsync(CancellationToken ct = default)
    {
        if (_memory is not null) return _memory;
        try { return _memory = await SecureStorage.Default.GetAsync(TokenKey); }
        catch { return null; }
    }

    public Task ClearAsync(CancellationToken ct = default)
    {
        _memory = null;
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }
}
