using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface ILocationsService
    {
        Task<Location> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Location> RenameAsync(Guid id, string newName, CancellationToken ct);
        Task<Location> ReorderAsync(Guid id, int newSortOrder, CancellationToken ct);
        Task<Location> MoveAsync(Guid id, Guid? newParentId, CancellationToken ct);
        Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<Item>> GetItemsAsync(Guid id, CancellationToken ct);
        Task<Location> CreateLocationAsync(LocationCreateRequest request, CancellationToken ct);
    }
}
