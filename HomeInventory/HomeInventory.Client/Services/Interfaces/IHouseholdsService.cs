using HomeInventory.Client.Models;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IHouseholdsService
    {
        Task<Household> GetByIdAsync(Guid householdId, CancellationToken ct);
        Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct);
        Task<Household> CreateAsync(Guid ownerId, string name, CancellationToken ct);
        Task<IReadOnlyList<LocationListItem>> GetLocationsAsync(Guid householdId, CancellationToken ct);
        Task<IReadOnlyList<Item>> GetItemsAsync(Guid householdId, CancellationToken ct);
    }
}
