using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
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
      


        public async Task<ItemDto> GetByIdAsync(Guid id, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.GetAsync($"api/locations/{id}", ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }


        public Task<IReadOnlyList<ItemDto>> SearchAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemDto> UpdateAsync(Guid id, UpdateItemRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PatchAsJsonAsync($"api/items/{id}", request, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }

        public async Task<ItemDto> CreateAsync(CreateItemRequestDto request, CancellationToken ct)
        {
            try
            {
                using var resp = await _http.PostAsJsonAsync("api/items", request, ct);

                if (resp.StatusCode is System.Net.HttpStatusCode.Forbidden or System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                resp.EnsureSuccessStatusCode();

                var result = await resp.Content.ReadFromJsonAsync<ItemDto>(ct);
                return result ?? throw new InvalidOperationException("Empty response body.");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException("Unable to connect to server", ex);
            }
        }
    }
}
