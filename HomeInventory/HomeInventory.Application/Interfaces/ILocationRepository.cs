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
        
        public void Add(Location location);
        
        /**
         * Returns enumerable of locations with given name. If parentLocation is set, 
         * it returns empty or single item enumerable.
         */
        public IEnumerable<Location> FindByName(string name, Guid? parentId = null);
        /**
         * Returns enumerable of locations that are direct childs of the given location. If the filter
         * is set, it only returns locations of the given type.
         */
        public IEnumerable<Location> GetChildren(Guid? parentid, IEnumerable<LocationType>? filter = null);

        public Location GetById(Guid id);

        public bool ExistsSiblingWithName(Guid? parentLocationId, string name, Guid? excludeLocationId = null);

        public IEnumerable<Location> GetAncestors(Guid locationId);

        public bool IsDescendantOf(Guid locationsId, Guid potentialAncestorId);

        public IEnumerable<Location> GetSubtree(Guid rootLocationId, bool includeRoot = true);

        public IEnumerable<Location> Search(string query, Guid? withinParentId = null, int limit = 50);

        public IEnumerable<Location> GetRoots();

        public void Remove(Location location);

        public void Update(Location location);
    }
}
