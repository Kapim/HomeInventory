using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Auth
{
    public interface ITokenStore
    {
        /// <summary>
        /// When true (default), <see cref="SaveAsync"/> writes the token to durable storage so the
        /// user stays logged in across app restarts. When false, the token lives only for this session.
        /// </summary>
        bool Persist { get; set; }

        Task SaveAsync(string token, CancellationToken ct = default);
        Task<string?> LoadAsync(CancellationToken ct = default);
        Task ClearAsync(CancellationToken ct = default);
    }
}
