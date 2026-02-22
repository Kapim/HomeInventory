using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Interfaces
{
    public interface IItemRepository
    {
        Task<Item?> GetByIdAsync(
       Guid itemId,
       CancellationToken ct = default);

        Task<IReadOnlyList<Item>> GetAllForUserAsync(
            Guid ownerUserId,
            CancellationToken ct = default);

        Task<IReadOnlyList<Item>> FindByNameAsync(
            Guid ownerUserId,
            string name,
            CancellationToken ct = default);

        Task<IReadOnlyList<Item>> GetByLocationAsync(
            Guid ownerUserId,
            Guid locationId,
            CancellationToken ct = default);

        Task<bool> ExistsAsync(
            Guid itemId,
            CancellationToken ct = default);

        Task AddAsync(
            Item item,
            CancellationToken ct = default);

        Task UpdateAsync(
            Item item,
            CancellationToken ct = default);

        Task DeleteAsync(
            Guid itemId,
            CancellationToken ct = default);
    }
}
