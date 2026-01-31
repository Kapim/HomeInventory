using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Auth
{
    public interface ITokenStore
    {
        Task SaveAsync(string token, CancellationToken ct = default);
        Task<string?> LoadAsync(CancellationToken ct = default);
        Task ClearAsync(CancellationToken ct = default);
    }
}
