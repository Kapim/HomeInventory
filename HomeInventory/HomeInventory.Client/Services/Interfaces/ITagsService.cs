using HomeInventory.Client.Models;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface ITagsService
    {
        Task<IReadOnlyList<Tag>> GetTagsForHouseholdAsync(Guid householdId, CancellationToken ct = default);
        Task<Tag> CreateTagAsync(string name, string color, Guid householdId, CancellationToken ct = default);
        Task DeleteTagAsync(Guid tagId, CancellationToken ct = default);
        Task<Item> AssignTagAsync(Guid itemId, Guid tagId, CancellationToken ct = default);
        Task<Item> RemoveTagAsync(Guid itemId, Guid tagId, CancellationToken ct = default);
    }
}
