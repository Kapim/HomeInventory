using HomeInventory.Application.Interfaces;
using HomeInventory.Application.Models;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.UseCases
{ 
    public class LocationUseCase(ILocationRepository locations, IItemRepository items) : ILocationUseCase
    {
        private readonly ILocationRepository _locations = locations;
        private readonly IItemRepository _items = items;
        public async Task<Location> GetLocationAsync(Guid id)
        {
            return await _locations.GetByIdAsync(id, new CancellationTokenSource().Token);
        }

        public async Task AddLocation(string name, Guid houseHoldId, Guid ownerUserId, Guid? parentLocationId = default, LocationType locationType = LocationType.Other)
        {
            Location location = new(name, houseHoldId, ownerUserId);
            location.MoveTo(parentLocationId);
            location.LocationType = locationType;
            await _locations.AddAsync(location);
            return;
        }

        public async Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null)
        {
            return await _locations.FindByNameAsync(name, parentId, new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Location>> SearchAsync(string name, Guid? withinParentId = null, int limit = 50)
        {
            return await _locations.SearchAsync(name, withinParentId, limit, new CancellationTokenSource().Token);
        }

        public async Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct)
        {
            var location = await _locations.GetByIdAsync(id, ct);
            location.Rename(request.Name);
            location.SetDescription(request.Description);
            location.MoveTo(request.ParentLocationId);
            location.SortOrder = request.SortOrder;
            location.LocationType = request.LocationType;
            await _locations.UpdateAsync(location, ct);
            return location;
        }

        public async Task<IReadOnlyList<Item>> GetItemsAsync(Guid locationId, CancellationToken ct)
        {
            return await _items.GetByLocationAsync(locationId);
        }
    }
}
