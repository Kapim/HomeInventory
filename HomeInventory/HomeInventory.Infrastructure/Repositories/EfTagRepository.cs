using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Repositories
{
    public class EfTagRepository(HomeInventoryDbContext db) : ITagRepository
    {
        public async Task<Tag> AddAsync(Tag tag, CancellationToken ct = default)
        {
            await db.Tags.AddAsync(tag, ct);
            await db.SaveChangesAsync(ct);
            return tag;
        }

        public async Task<Tag?> GetByIdAsync(Guid tagId, CancellationToken ct = default)
            => await db.Tags.FindAsync([tagId], ct);

        public async Task<IReadOnlyList<Tag>> GetByHouseholdAsync(Guid householdId, CancellationToken ct = default)
            => await db.Tags.Where(t => t.HouseholdId == householdId).ToListAsync(ct);

        public async Task DeleteAsync(Guid tagId, CancellationToken ct = default)
        {
            var tag = await GetByIdAsync(tagId, ct) ?? throw new KeyNotFoundException();
            db.Tags.Remove(tag);
            await db.SaveChangesAsync(ct);
        }
    }
}
