using HomeInventory.Domain;

namespace HomeInventory.Application.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> AddAsync(Tag tag, CancellationToken ct = default);
        Task<Tag?> GetByIdAsync(Guid tagId, CancellationToken ct = default);
        Task<IReadOnlyList<Tag>> GetByHouseholdAsync(Guid householdId, CancellationToken ct = default);
        Task DeleteAsync(Guid tagId, CancellationToken ct = default);
    }
}
