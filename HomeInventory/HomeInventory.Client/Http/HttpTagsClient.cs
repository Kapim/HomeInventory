using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Net.Http.Json;

namespace HomeInventory.Client.Http
{
    public class HttpTagsClient(HttpClient http) : ITagsApiClient
    {
        private readonly HttpClient _http = http;

        public async Task<IReadOnlyList<TagDto>> GetTagsForHouseholdAsync(Guid householdId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/households/{householdId}/tags", ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<TagDto>>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Neplatný formát odpovědi.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex) { throw HttpErrorMapper.MapNetwork(ex); }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested) { throw HttpErrorMapper.MapNetwork(ex); }
        }

        public async Task<TagDto> CreateTagAsync(CreateTagRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PostAsJsonAsync("api/tags", request, ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                var result = await resp.Content.ReadFromJsonAsync<TagDto>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Neplatný formát odpovědi.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex) { throw HttpErrorMapper.MapNetwork(ex); }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested) { throw HttpErrorMapper.MapNetwork(ex); }
        }

        public async Task DeleteTagAsync(Guid tagId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.DeleteAsync($"api/tags/{tagId}", ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
            }
            catch (HttpRequestException ex) { throw HttpErrorMapper.MapNetwork(ex); }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested) { throw HttpErrorMapper.MapNetwork(ex); }
        }

        public async Task<ItemDto> AssignTagAsync(Guid itemId, Guid tagId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PostAsync($"api/items/{itemId}/tags/{tagId}", null, ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Neplatný formát odpovědi.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex) { throw HttpErrorMapper.MapNetwork(ex); }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested) { throw HttpErrorMapper.MapNetwork(ex); }
        }

        public async Task<ItemDto> RemoveTagAsync(Guid itemId, Guid tagId, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.DeleteAsync($"api/items/{itemId}/tags/{tagId}", ct);
                if (!resp.IsSuccessStatusCode)
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
                return result ?? throw new ApiException(ApiErrorTypes.InvalidResponse, "Neplatný formát odpovědi.", (int)resp.StatusCode);
            }
            catch (HttpRequestException ex) { throw HttpErrorMapper.MapNetwork(ex); }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested) { throw HttpErrorMapper.MapNetwork(ex); }
        }
    }
}
