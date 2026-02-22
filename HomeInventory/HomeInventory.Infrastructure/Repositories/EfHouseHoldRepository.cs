using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Repositories
{
    public class EfHouseHoldRepository(HomeInventoryDbContext db) : IHouseholdRepository
    {
        readonly HomeInventoryDbContext _db = db;

        public async Task AddAsync(Household household, CancellationToken ct = default)
        {
            await _db.AddAsync(household, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Household> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var household = await _db.Households.FindAsync([id], ct);
            return household ?? throw new KeyNotFoundException();
        }

        public async Task<IReadOnlyList<Household>> GetHouseholdsAsync(CancellationToken ct = default)
        {
            return await _db.Households.ToListAsync(ct);
        }

        public async Task RemoveAsync(Household household, CancellationToken ct = default)
        {
            _db.Remove(household);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Household household, CancellationToken ct = default)
        {
            _db.Households.Update(household);
            await _db.SaveChangesAsync(ct);
        }
    }
}
