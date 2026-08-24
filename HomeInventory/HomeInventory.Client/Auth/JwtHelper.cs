using System.Text;
using System.Text.Json;

namespace HomeInventory.Client.Auth
{
    /// <summary>
    /// Minimal, validation-free reading of a JWT's <c>exp</c> claim — used only to decide whether a
    /// stored token is still worth reusing on startup. The server remains the real authority.
    /// </summary>
    public static class JwtHelper
    {
        public static bool IsValid(string? token) =>
            GetExpiry(token) is { } exp && exp > DateTimeOffset.UtcNow;

        public static DateTimeOffset? GetExpiry(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var parts = token.Split('.');
            if (parts.Length < 2) return null;

            try
            {
                var payload = parts[1].Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("exp", out var expEl) && expEl.TryGetInt64(out var exp))
                    return DateTimeOffset.FromUnixTimeSeconds(exp);
            }
            catch { /* malformed token → treat as invalid */ }

            return null;
        }
    }
}
