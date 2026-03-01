using HomeInventory.Domain;

namespace HomeInventory.Application.Interfaces
{
    public interface IHouseholdRepository
    {
        public Task AddAsync(Household household, CancellationToken ct = default);
        
        public Task RemoveAsync(Household household, CancellationToken ct = default);

        public Task UpdateAsync(Household household, CancellationToken ct = default);

        public Task<IReadOnlyList<Household>> GetHouseholdsAsync(CancellationToken ct = default);
        public Task<Household> GetByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<Location>> GetLocationsAsync(Guid householdId, CancellationToken token);
    }
}
