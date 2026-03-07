using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;


namespace HomeInventory.Client.Http
{
    public class HttpItemsClient(HttpClient http) : IItemsApiClient

    {
        private readonly HttpClient _http = http;

       
        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.DeleteAsync($"api/items/{id}", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }

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
      


        public async Task<ItemDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/items/{id}", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }


                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
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


        public async Task<IReadOnlyList<ItemDto>> SearchAsync(string name, CancellationToken ct)
        {
            try
            {
                var encodedName = WebUtility.UrlEncode(name);
                using var resp = await _http.GetAsync($"api/items/search?name={encodedName}", ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }


                var result = await resp.Content.ReadFromJsonAsync<IReadOnlyList<ItemDto>>(ct);
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

        public async Task<ItemDto> UpdateAsync(Guid id, UpdateItemRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PatchAsJsonAsync($"api/items/{id}", request, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }


                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
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

        public async Task<ItemDto> CreateAsync(CreateItemRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PostAsJsonAsync("api/items", request, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    throw HttpErrorMapper.Map(resp, await resp.Content.ReadAsStringAsync(ct));
                }


                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
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
