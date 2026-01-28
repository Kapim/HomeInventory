using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace HomeInventory.Client.Services
{
    public class HttpAuthService(HttpClient http) : IAuthService
    {
        private readonly HttpClient _http = http;
        public Task<bool> IsLoggedInAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken ct = default)
        {
            try
            {
                var req = new LoginRequest(username, password);

                using var resp = await _http.PostAsJsonAsync("api/auth", req, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new InvalidCredentialsException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<LoginResult>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            } catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Nelze se připojit k serveru.", ex);
            }
            
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
