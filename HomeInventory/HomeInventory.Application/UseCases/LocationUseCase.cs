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
        public async Task<Location> GetLocationAsync(Guid id, CancellationToken ct)
        {
            return await _locations.GetByIdAsync(id, new CancellationTokenSource().Token);
        }


        public async Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null, CancellationToken ct = default)
        {
            return await _locations.FindByNameAsync(name, parentId, new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Location>> SearchAsync(string name, Guid? withinParentId = null, int limit = 50, CancellationToken ct = default)
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
            return await _items.GetByLocationAsync(locationId, ct);
        }

        public async Task<Location> AddLocationAsync(LocationCreateRequest request, CancellationToken ct)
        {
            Location location = new(request.Name, request.HouseholdId, request.OwnerId);
            location.MoveTo(request.ParentLocationId);
            location.LocationType = request.LocationType;
            await _locations.AddAsync(location, ct);
            return location;
        }

        public async Task DeleteLocation(Guid locationId, CancellationToken ct)
        {
            var locationChilds = await _locations.GetSubtreeAsync(locationId, true, ct);
            foreach (var l in locationChilds)
            {
                await _items.DeleteItemsFromLocationAsync(l.Id, ct);
                await _locations.DeleteAsync(l, ct);
            }
        }
    }
}
