using HomeInventory.Client.Models;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IHouseholdsService
    {
        Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct);
        Task<Household> CreateAsync(string name);
    }
}
