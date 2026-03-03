using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Services
{
    public class LocationService(ILocationsApiClient apiClient) : ILocationsService
    {
        private readonly ILocationsApiClient _apiClient = apiClient;

        public async Task<Location> CreateLocationAsync(LocationCreateRequest request, CancellationToken ct)
        {
            return LocationMapping.Map(await _apiClient.CreateAsync(LocationMapping.Map(request), ct));
        }


        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            await _apiClient.DeleteAsync(id, ct);
        }

        public async Task<Location> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return LocationMapping.Map(await _apiClient.GetByIdAsync(id, ct));
        }

        public async Task<IReadOnlyList<Item>> GetItemsAsync(Guid id, CancellationToken ct)
        {
            return [.. (await _apiClient.GetItemsAsync(id, ct)).Select(x => ItemMapping.Map(x))];
        }

        public async Task<Location> MoveAsync(Guid id, Guid? newParentId, CancellationToken ct)
        {
            var location = await _apiClient.GetByIdAsync(id, ct);
            var request = new UpdateLocationRequestDto(location.Name, location.LocationType, location.SortOrder, location.Description, newParentId);
            return LocationMapping.Map(await _apiClient.UpdateAsync(id, request, ct));
        }

        public async Task<Location> RenameAsync(Guid id, string newName, CancellationToken ct)
        {
            var location = await _apiClient.GetByIdAsync(id, ct);
            var request = new UpdateLocationRequestDto(newName, location.LocationType, location.SortOrder, location.Description, location.ParentLocationId);
            return LocationMapping.Map(await _apiClient.UpdateAsync(id, request, ct));
        }

        public async Task<Location> ReorderAsync(Guid id, int newSortOrder, CancellationToken ct)
        {
            var location = await _apiClient.GetByIdAsync(id, ct);
            var request = new UpdateLocationRequestDto(location.Name, location.LocationType, newSortOrder, location.Description, location.ParentLocationId);
            return LocationMapping.Map(await _apiClient.UpdateAsync(id, request, ct));
        }

        public async Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct)
        {
            return LocationMapping.Map(await _apiClient.UpdateAsync(id, LocationMapping.Map(request), ct));
        }

        
    }
}
