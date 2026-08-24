using HomeInventory.Client.Auth;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Services
{
    /// <summary>
    /// Token store that keeps the JWT in memory for the running session and — when <see cref="Persist"/>
    /// is true — also on disk, encrypted with Windows DPAPI (per Windows user), so "Zůstat přihlášen"
    /// survives app restarts. When false, only the in-memory copy is kept and any on-disk token is removed.
    /// </summary>
    public class PersistentTokenStore : ITokenStore
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HomeInventory", "auth.dat");

        private string? _memory;

        public bool Persist { get; set; } = true;

        public Task SaveAsync(string token, CancellationToken ct = default)
        {
            _memory = token;
            try
            {
                if (Persist)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                    var encrypted = ProtectedData.Protect(
                        Encoding.UTF8.GetBytes(token), null, DataProtectionScope.CurrentUser);
                    File.WriteAllBytes(FilePath, encrypted);
                }
                else if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
            }
            catch { /* best-effort persistence; the in-memory token still works this session */ }

            return Task.CompletedTask;
        }

        public Task<string?> LoadAsync(CancellationToken ct = default)
        {
            if (_memory is not null)
                return Task.FromResult<string?>(_memory);

            try
            {
                if (File.Exists(FilePath))
                {
                    var decrypted = ProtectedData.Unprotect(
                        File.ReadAllBytes(FilePath), null, DataProtectionScope.CurrentUser);
                    _memory = Encoding.UTF8.GetString(decrypted);
                }
            }
            catch { _memory = null; }

            return Task.FromResult(_memory);
        }

        public Task ClearAsync(CancellationToken ct = default)
        {
            _memory = null;
            try { if (File.Exists(FilePath)) File.Delete(FilePath); }
            catch { /* ignore */ }
            return Task.CompletedTask;
        }
    }
}
