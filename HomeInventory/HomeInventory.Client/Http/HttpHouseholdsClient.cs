using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Net.Http.Headers;

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

        public async Task<string> ExportCsvAsync(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/export", ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                return await resp.Content.ReadAsStringAsync(ct);
            }
            catch (HttpRequestException ex)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            }
        }

        public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(Guid householdId, string query, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/search?q={Uri.EscapeDataString(query)}", ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));

                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<SearchResultDto>>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Odpoveď serveru má neplatný formát.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            }
        }

        public async Task<ImportResultDto> ImportCsvAsync(Guid householdId, Stream csvStream, string fileName, CancellationToken ct)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(csvStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                content.Add(fileContent, "file", fileName);

                using var resp = await _http.PostAsync($"api/households/{householdId}/import", content, ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));

                var result = await resp.Content.ReadFromJsonAsync<ImportResultDto>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Odpoveď serveru má neplatný formát.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                throw HttpErrorMapper.MapNetwork(ex);
            }
        }
    }
}
