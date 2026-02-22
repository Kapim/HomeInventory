using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace HomeInventory.Client.Http
{
    public class HttpHouseholdsClient(HttpClient http) : IHouseholdsApiClient

    {
        private readonly HttpClient _http = http;

        public Task<HouseholdDto> CreateAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<HouseholdDto>> GetAllAsync(CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync("api/household", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<HouseholdDto>>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

       

    }
}
