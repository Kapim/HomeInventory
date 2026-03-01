using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace HomeInventory.Client.Http
{
    public class HttpHouseholdsClient(HttpClient http) : IHouseholdsApiClient

    {
        private readonly HttpClient _http = http;

        public async Task<HouseholdDto> CreateAsync(CreateHouseholdRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PostAsJsonAsync("api/households", request, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<HouseholdDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task<IReadOnlyList<HouseholdDto>> GetAllAsync(CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync("api/households", ct);

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

        public async Task<HouseholdDto> GetByIdAsync(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<HouseholdDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task<IReadOnlyList<ItemDto>> GetItems(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/items", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<ItemDto>> (ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task<IReadOnlyList<LocationListItemDto>> GetLocations(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/locations", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<LocationListItemDto>>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }
    }
}
