using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{ 
    public class HouseholdUseCase(IHouseholdRepository households) : IHouseholdUseCase
    {
        private readonly IHouseholdRepository _households = households;

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

    }
}
