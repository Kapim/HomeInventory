using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Net.Http.Json;

namespace HomeInventory.Client.Http
{
    public class HttpLocationsClient(HttpClient http) : ILocationsApiClient

    {
        private readonly HttpClient _http = http;

        public async Task<LocationDto> CreateAsync(CreateLocationRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PostAsJsonAsync("api/locations", request, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<LocationDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.DeleteAsync($"api/locations/{id}", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }

        }    
      

        public async Task<LocationDto> UpdateAsync(Guid id, UpdateLocationRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PatchAsJsonAsync($"api/locations/{id}", request, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<LocationDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task<LocationDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/locations/{id}", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<LocationDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task<IReadOnlyList<ItemDto>> GetItemsAsync(Guid locationId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/locations/{locationId}/items", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<ItemDto>>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

    }
}
