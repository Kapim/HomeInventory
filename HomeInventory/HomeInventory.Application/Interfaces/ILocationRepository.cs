using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HomeInventory.Application.Interfaces
{
    public interface ILocationRepository
    {
        
        public Task AddAsync(Location location, CancellationToken ct = default);
        
        /**
         * Returns enumerable of locations with given name. If parentLocation is set, 
         * it returns empty or single item enumerable.
         */
        public Task<IReadOnlyList<Location>> FindByNameAsync(string name, Guid? parentId = null, CancellationToken ct = default);
        /**
         * Returns enumerable of locations that are direct childs of the given location. If the filter
         * is set, it only returns locations of the given type.
         */
        public Task<IReadOnlyList<Location>> GetChildrenAsync(Guid? parentid, IEnumerable<LocationType>? filter = null, CancellationToken ct = default);

        public Task<Location> GetByIdAsync(Guid id, CancellationToken ct = default);

        public Task<bool> ExistsSiblingWithNameAsync(Guid? parentLocationId, string name, Guid? excludeLocationId = null, CancellationToken ct = default);

        public Task<IReadOnlyList<Location>> GetAncestorsAsync(Guid locationId, CancellationToken ct = default);

        public Task<bool> IsDescendantOfAsync(Guid locationsId, Guid potentialAncestorId, CancellationToken ct = default);

        public Task<IReadOnlyList<Location>> GetSubtreeAsync(Guid rootLocationId, bool includeRoot = true, CancellationToken ct = default);

        public Task<IReadOnlyList<Location>> SearchAsync(string query, Guid? withinParentId = null, int limit = 50, CancellationToken ct = default);

        public Task<IReadOnlyList<Location>> GetRootsAsync(CancellationToken ct = default);

        public Task RemoveAsync(Location location, CancellationToken ct = default);

        public Task UpdateAsync(Location location, CancellationToken ct = default);
    }
}
