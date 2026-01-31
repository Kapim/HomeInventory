using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Auth
{
    public sealed class AuthHeaderHandler(ITokenStore tokenStore) : DelegatingHandler
    {
        private readonly ITokenStore _tokenStore = tokenStore;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = await _tokenStore.LoadAsync(ct);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, ct);
        }
    }
}
