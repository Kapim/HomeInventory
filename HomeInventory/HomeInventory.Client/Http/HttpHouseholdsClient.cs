using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
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

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

                var result = await resp.Content.ReadFromJsonAsync<HouseholdDto>(ct);
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

        public async Task<IReadOnlyList<HouseholdDto>> GetAllAsync(CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync("api/households", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<HouseholdDto>>(ct);
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

        public async Task<HouseholdDto> GetByIdAsync(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

                var result = await resp.Content.ReadFromJsonAsync<HouseholdDto>(ct);
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

        public async Task<IReadOnlyList<ItemDto>> GetItems(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/items", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<ItemDto>> (ct);
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

        public async Task<IReadOnlyList<LocationListItemDto>> GetLocations(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/locations", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<LocationListItemDto>>(ct);
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
