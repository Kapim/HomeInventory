using HomeInventory.Application.Models;
using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{
    public interface ITagUseCase
    {
        Task<Tag> CreateTagAsync(TagCreateRequest request, CancellationToken ct = default);
        Task<IReadOnlyList<Tag>> GetTagsForHouseholdAsync(Guid householdId, CancellationToken ct = default);
        Task<Tag?> GetTagAsync(Guid tagId, CancellationToken ct = default);
        Task DeleteTagAsync(Guid tagId, CancellationToken ct = default);
        Task<Item> AssignTagToItemAsync(Guid itemId, Guid tagId, CancellationToken ct = default);
        Task<Item> RemoveTagFromItemAsync(Guid itemId, Guid tagId, CancellationToken ct = default);
    }
}
