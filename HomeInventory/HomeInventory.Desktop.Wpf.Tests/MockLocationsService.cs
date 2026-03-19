using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;

namespace HomeInventory.Wpf.Tests
{
    public class MockLocationsService : ILocationsService
    {
        private readonly List<Item> _items = [];

        public int GetItemsAsyncCalls { get; private set; }

        public void GenerateItems(List<Guid> guids, Guid locationId)
        {
            foreach (var guid in guids)
            {
                _items.Add(new(guid, "name", 0, locationId, Guid.NewGuid(), null, null));
            }
        }

        public Task<Location> CreateLocationAsync(LocationCreateRequest request, CancellationToken ct)
            => throw new NotImplementedException();

        public Task DeleteAsync(Guid id, CancellationToken ct)
            => throw new NotImplementedException();

        public Task<Location> GetByIdAsync(Guid id, CancellationToken ct)
            => Task.FromResult(new Location(id, "test", LocationType.Room, null, Guid.NewGuid(), Guid.NewGuid(), 0, null));

        public Task<IReadOnlyList<Item>> GetItemsAsync(Guid id, CancellationToken ct)
        {
            GetItemsAsyncCalls++;
            var filtered = _items.Where(x => x.LocationId == id).ToList();
            return Task.FromResult((IReadOnlyList<Item>)filtered);
        }

        public Task<Location> MoveAsync(Guid id, Guid? newParentId, CancellationToken ct)
            => throw new NotImplementedException();

        public Task<Location> RenameAsync(Guid id, string newName, CancellationToken ct)
            => throw new NotImplementedException();

        public Task<Location> ReorderAsync(Guid id, int newSortOrder, CancellationToken ct)
            => throw new NotImplementedException();

        public Task<Location> UpdateAsync(Guid id, LocationUpdateRequest request, CancellationToken ct)
            => throw new NotImplementedException();
    }
}
