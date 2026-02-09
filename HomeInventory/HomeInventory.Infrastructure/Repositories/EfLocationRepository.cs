using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace HomeInventory.Infrastructure.Repositories
{
    public class EfLocationRepository(HomeInventoryDbContext db) : ILocationRepository
    {
        readonly HomeInventoryDbContext _db = db;

        private IQueryable<Location> GetChildrenQuery(Guid? parentId) =>
            _db.Locations.Where(x => x.ParentLocationId == parentId);


        private async Task<Location?> GetLocationAsync(Guid? locationId, CancellationToken ct = default) =>
            locationId is null ? null : await _db.Locations.FirstOrDefaultAsync(x => x.Id == locationId, ct);      


        public async Task AddAsync(Location location, CancellationToken ct = default)
        {
            await _db.Locations.AddAsync(location, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsSiblingWithNameAsync(Guid? parentLocationId, string name, Guid? excludeLocationId = null, CancellationToken ct = default)
        {
            var query = GetChildrenQuery(parentLocationId)
                .AsNoTracking()
                .Where(x => x.NormalizedName == name.ToLowerInvariant());

            if (excludeLocationId is Guid id)
                query = query.Where(x => x.Id != id);

            return await  query.AnyAsync(ct);
        }

        public async Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null, CancellationToken ct = default)
        {
            var query = _db.Locations.AsNoTracking()
                .Where(x => x.NormalizedName == name.ToLowerInvariant());

            if (parentId is not null)
                query = query.Where(x => x.ParentLocationId == parentId);

            return await query.ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Location>> GetAncestorsAsync(Guid locationId, CancellationToken ct = default)
        {
            List<Location> locations = [];
            var location = await GetLocationAsync(locationId, ct);
            if (location != null)
                location = await GetLocationAsync(location.ParentLocationId, ct);
            while (location != null)
            {
                locations.Add(location);
                location = await GetLocationAsync(location.ParentLocationId, ct);
            }
            return locations;
        }

        public async Task<Location> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var loc = await _db.Locations.FindAsync([id], ct);
            return loc ?? throw new KeyNotFoundException();
        }

        public async Task<IReadOnlyList<Location>> GetChildrenAsync(Guid? parentid, IEnumerable<LocationType>? filter = null, CancellationToken ct = default)
        {
            var query = GetChildrenQuery(parentid);

            var types = filter?.ToArray();
            if (types is { Length: > 0 })            
                query = query.Where(x => types.Contains(x.LocationType));
            
            return await query.ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Location>> GetRootsAsync(CancellationToken ct = default)
        {
            return await _db.Locations.AsNoTracking().Where(x => x.ParentLocationId == null).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Location>> GetSubtreeAsync(Guid rootLocationId, bool includeRoot = true, CancellationToken ct = default)
        {
            List<Location> locations = [];
            var root = await GetLocationAsync(rootLocationId, ct);
            if (root is not null)
            {
                if (includeRoot)
                    locations.Add(root);
                locations.AddRange(await GetSubtreeAsync(root, ct));
                return locations;
            } else
            {
                throw new ArgumentException($"Location with Id {rootLocationId} not found!");
            }


        }

        public async Task<IReadOnlyList<Location>> GetSubtreeAsync(Location? location, CancellationToken ct = default)
        {
            if (location == null)
                return [];
            var childrens = await GetChildrenAsync(location.Id, ct: ct);
            List<Location> locations = [];
            locations.AddRange(childrens);
            foreach (var children in childrens)
            {
                locations.AddRange(await GetSubtreeAsync(children, ct));
            }
            return locations;
        }

        public async Task<bool> IsDescendantOfAsync(Guid locationsId, Guid potentialAncestorId, CancellationToken ct = default)
        {
            var location = await GetLocationAsync(locationsId, ct);
            if (location is not null)
                if (location.ParentLocationId is not null)
                    if (location.ParentLocationId == potentialAncestorId)
                        return true;
                    else
                        return (await IsDescendantOfAsync((Guid)location.ParentLocationId, potentialAncestorId, ct));
            return false;
        }

        public async Task RemoveAsync(Location location, CancellationToken ct = default)
        {
            var locations = await GetSubtreeAsync(location, ct);
            _db.Locations.RemoveRange(locations);
            _db.Locations.Remove(location);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Location>> SearchAsync(string query, Guid? withinParentId = null, int limit = 50, CancellationToken ct = default)
        {
            var queryNormalized = query.ToLowerInvariant();
            var q = _db.Locations.AsNoTracking().Where(x => x.NormalizedName.Contains(queryNormalized));

            if (withinParentId is not null)
                q = q.Where(x => x.ParentLocationId == withinParentId);
                    

            return await q.Take(limit).ToListAsync(cancellationToken: ct);
        }

        public async Task UpdateAsync(Location location, CancellationToken ct = default)
        {
            _db.Update(location);
            await _db.SaveChangesAsync(ct);
        }
    }
}
