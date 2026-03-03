using HomeInventory.Application.Models;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.UseCases
{
    public interface ILocationUseCase
    {

        public Task<Location> GetLocationAsync(Guid id, CancellationToken ct);

        public Task<Location> AddLocationAsync(LocationCreateRequest request, CancellationToken ct);

        public Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null, CancellationToken ct = default);

        public Task<IReadOnlyList<Location>> SearchAsync(string name, Guid? withinParentId = null, int limit = 50, CancellationToken ct = default);
        public Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct);
        public Task<IReadOnlyList<Item>> GetItemsAsync(Guid locationId, CancellationToken ct);
    }
}
