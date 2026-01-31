using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Infrastructure.Repositories
{
    public class EfLocationRepository(HomeInventoryDbContext db) : ILocationRepository
    {
        private readonly HomeInventoryDbContext _db = db;
        public void Add(Location location)
        {
            _db.Locations.Add(location);
            _db.SaveChanges();
        }

        public bool ExistsSiblingWithName(Guid? parentLocationId, string name, Guid? excludeLocationId = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> FindByName(string name, Guid? parentId = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetAncestors(Guid locationId)
        {
            throw new NotImplementedException();
        }

        public Location GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetChildren(Guid? parentid, IEnumerable<LocationType>? filter = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetRoots()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetSubtree(Guid rootLocationId, bool includeRoot = true)
        {
            throw new NotImplementedException();
        }

        public bool IsDescendantOf(Guid locationsId, Guid potentialAncestorId)
        {
            throw new NotImplementedException();
        }

        public void Remove(Location location)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> Search(string query, Guid? withinParentId = null, int limit = 50)
        {
            throw new NotImplementedException();
        }

        public void Update(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
