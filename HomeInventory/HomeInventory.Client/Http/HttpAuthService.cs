using HomeInventory.Client.Auth;
using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace HomeInventory.Client.Http
{
    public class HttpAuthService(HttpClient http) : IAuthApiClient
    {
        private readonly HttpClient _http = http;
       
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            try
            {
                
                using var resp = await _http.PostAsJsonAsync("api/auth", request, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new InvalidCredentialsException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<LoginResponseDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Nelze se připojit k serveru.", ex);
            }
        }

       
    }
}
