using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IItemsService
    {
        Task<IReadOnlyList<Item>> SearchAsync(string query, CancellationToken ct);
        Task<Item> UpdateAsync(Guid id, ItemUpdateRequest request, CancellationToken ct);

        Task<Item> CreateAsync(ItemCreateRequest request, CancellationToken ct);
        Task<Item> GetByIdAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);

    }
}
