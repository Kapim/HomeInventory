using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{
    public interface IHouseholdUseCase
    {

        public Task<Household> GetHouseholdAsync(Guid id);
        public Task AddHouseholdAsync(string name);
        public Task<IReadOnlyList<Household>> GetHouseholdsAsync();
        Task<IReadOnlyList<Location>> GetLocationsAsync(Guid householdId);
    }
}
