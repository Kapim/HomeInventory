using HomeInventory.Application.Models;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.UseCases
{
    public interface ILocationUseCase
    {

        public Task<Location> GetLocationAsync(Guid id);

        public Task AddLocation(string name, Guid houseHoldId, Guid ownerUserId, Guid? parentLocationId = default, LocationType locationType = LocationType.Other);

        public Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null);

        public Task<IReadOnlyList<Location>> SearchAsync(string name, Guid? withinParentId = null, int limit = 50);
        public Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct);
        public Task<IReadOnlyList<Item>> GetItemsAsync(Guid locationId, CancellationToken ct);
    }
}
