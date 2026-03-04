using HomeInventory.Client.Auth;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace HomeInventory.Client.Http
{
    public class HttpAuthClient(HttpClient http) : IAuthApiClient
    {
        private readonly HttpClient _http = http;
       
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            try
            {
                
                using var resp = await _http.PostAsJsonAsync("api/auth/login", request, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

                var result = await resp.Content.ReadFromJsonAsync<LoginResponseDto>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Odpoveď serveru má neplatný formát.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            } 
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                //timeout
                throw HttpErrorMapper.MapNetwork(ex);
            }
        }

       
    }
}
