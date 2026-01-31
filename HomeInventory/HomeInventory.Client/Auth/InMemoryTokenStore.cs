using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Auth
{
    public class InMemoryTokenStore : ITokenStore
    {
        private string? _token = "";
        public Task ClearAsync(CancellationToken ct = default)
        {
            _token = null;
            return Task.CompletedTask;
        }

        public Task<string?> LoadAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_token);
        }

        public Task SaveAsync(string token, CancellationToken ct = default)
        {
            _token = token;
            return Task.CompletedTask;
        }
    }
}
