using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;

namespace HomeInventory.Wpf.Tests
{
    public class MockLocationsService : ILocationsService
    {
        List<Item> _items = [];
        public void GenerateItems(List<Guid> guids, Guid locationId)
        {
            foreach (var guid in guids)
            {
                _items.Add(new(guid, "name", 0, locationId, Guid.NewGuid(), null, null));
            }
        }
        public Task<Location> CreateLocationAsync(LocationCreateRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<Location> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return new(id, "test", LocationType.Room, null, Guid.NewGuid(), Guid.NewGuid(), 0, null);
        }

        public async Task<IReadOnlyList<Item>> GetItemsAsync(Guid id, CancellationToken ct)
        {
            return _items;
        }

        public Task<Location> MoveAsync(Guid id, Guid? newParentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Location> RenameAsync(Guid id, string newName, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Location> ReorderAsync(Guid id, int newSortOrder, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
