using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{
    public class HouseholdUseCase(IHouseholdRepository households, IItemRepository items) : IHouseholdUseCase
    {
        private readonly IHouseholdRepository _households = households;
        private readonly IItemRepository _items = items;

        public async Task AddHouseholdAsync(string name)
        {
            Household household = new(name); 
            await _households.AddAsync(household, new CancellationTokenSource().Token);
        }
        public async Task<Household> GetHouseholdAsync(Guid id)
        {
            return await _households.GetByIdAsync(id, new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Household>> GetHouseholdsAsync()
        {
            return await _households.GetHouseholdsAsync(new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Location>> GetLocationsAsync(Guid householdId)
        {
            return await _households.GetLocationsAsync(householdId, new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Item>> GetItemsAsync(Guid householdId)
        {
            var locations = await _households.GetLocationsAsync(householdId, new CancellationTokenSource().Token);
            var result = new List<Item>();
            foreach (var loc in locations)
                result.AddRange(await _items.GetByLocationAsync(loc.Id, new CancellationTokenSource().Token));
            return result;
        }
    }
}
