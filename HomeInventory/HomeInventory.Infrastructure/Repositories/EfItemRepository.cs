using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Repositories
{
    public class EfItemRepository(HomeInventoryDbContext db) : IItemRepository
    {
        readonly HomeInventoryDbContext _db = db;

        public async Task AddAsync(Item item, CancellationToken ct = default)
        {
            await _db.AddAsync(item, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid itemId, CancellationToken ct = default)
        {
            var item = await GetByIdAsync(itemId, ct);
            if (item != null)
            {
                _db.Remove(item);
                await _db.SaveChangesAsync(ct);
            } else
            {
                throw new KeyNotFoundException();
            }
        }

        public async Task<bool> ExistsAsync(Guid itemId, CancellationToken ct = default)
        {
            return (await _db.Items.FindAsync([itemId], ct)) != null;
        }

        public async Task<IReadOnlyList<Item>> FindByNameAsync(Guid ownerUserId, string name, CancellationToken ct = default)
        {
            return await _db.Items.Where(x => x.OwnerUserId == ownerUserId && x.NormalizedName.Contains(name.ToLowerInvariant())).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Item>> GetAllForUserAsync(Guid ownerUserId, CancellationToken ct = default)
        {
            return await _db.Items.Where(x => x.OwnerUserId == ownerUserId).ToListAsync(ct);
        }

        public async Task<Item?> GetByIdAsync(Guid itemId, CancellationToken ct = default)
        {
            return await _db.Items.FindAsync([itemId], ct);
        }

        public async Task<IReadOnlyList<Item>> GetByLocationAsync(Guid ownerUserId, Guid locationId, CancellationToken ct = default)
        {
            return await _db.Items.Where(x => x.OwnerUserId == ownerUserId && x.LocationId == locationId).ToListAsync(ct);
        }

        public async Task UpdateAsync(Item item, CancellationToken ct = default)
        {
            _db.Items.Update(item);
            await _db.SaveChangesAsync(ct);
        }
    }
}
