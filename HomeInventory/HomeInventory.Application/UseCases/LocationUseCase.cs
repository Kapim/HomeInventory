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
            await MoveLocationCoreAsync(location, request.ParentLocationId, request.SortOrder, ct);
            location.LocationType = request.LocationType;
            await _locations.UpdateAsync(location, ct);
            return location;
        }

        public async Task<Location> MoveAsync(Guid id, LocationMoveRequest request, CancellationToken ct)
        {
            var location = await _locations.GetByIdAsync(id, ct);
            await MoveLocationCoreAsync(location, request.NewParentLocationId, request.SortOrder, ct);
            await _locations.UpdateAsync(location, ct);
            return location;
        }

        private async Task MoveLocationCoreAsync(Location location, Guid? newParentLocationId, int? requestedSortOrder, CancellationToken ct)
        {
            if (location.Id == newParentLocationId)
                throw new ArgumentException("Cannot move location to itself", nameof(newParentLocationId));

            if (newParentLocationId is Guid parentId)
            {
                var parent = await _locations.GetByIdAsync(parentId, ct);
                if (parent.HouseholdId != location.HouseholdId)
                    throw new InvalidOperationException("Cannot move location to a different household");

                if (await _locations.IsDescendantOfAsync(parentId, location.Id, ct))
                    throw new ArgumentException("Cannot move location under its descendant", nameof(newParentLocationId));
            }

            var siblings = (await _locations.GetChildrenAsync(location.HouseholdId, newParentLocationId, ct: ct))
                .Where(x => x.Id != location.Id)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();

            if (siblings.Any(x => x.NormalizedName == location.NormalizedName))
                throw new InvalidOperationException("A location with the same name already exists under the target parent");

            var sortOrder = requestedSortOrder ?? siblings.Count;
            sortOrder = Math.Clamp(sortOrder, 0, siblings.Count);

            for (var index = 0; index < siblings.Count; index++)
                siblings[index].SortOrder = index >= sortOrder ? index + 1 : index;

            location.MoveTo(newParentLocationId);
            location.SortOrder = sortOrder;
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
            location.SortOrder = request.SortOrder;
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
