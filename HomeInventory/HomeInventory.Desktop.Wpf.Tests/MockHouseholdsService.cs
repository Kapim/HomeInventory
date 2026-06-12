using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;

namespace HomeInventory.Wpf.Tests
{
    public class MockHouseholdsService : IHouseholdsService
    {
        private readonly List<LocationListItem> _locations = [];

        public void SetLocations(IEnumerable<LocationListItem> locations)
        {
            _locations.Clear();
            _locations.AddRange(locations);
        }

        public Task<IReadOnlyList<LocationListItem>> GetLocationsAsync(Guid householdId, CancellationToken ct)
            => Task.FromResult<IReadOnlyList<LocationListItem>>(_locations);

        public Task<Household> GetByIdAsync(Guid householdId, CancellationToken ct)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct)
            => throw new NotImplementedException();

        public Task<Household> CreateAsync(string name, CancellationToken ct)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<Item>> GetItemsAsync(Guid householdId, CancellationToken ct)
            => throw new NotImplementedException();
    }
}
