using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface ITagsApiClient
    {
        Task<IReadOnlyList<TagDto>> GetTagsForHouseholdAsync(Guid householdId, CancellationToken ct);
        Task<TagDto> CreateTagAsync(CreateTagRequestDto request, CancellationToken ct);
        Task DeleteTagAsync(Guid tagId, CancellationToken ct);
        Task<ItemDto> AssignTagAsync(Guid itemId, Guid tagId, CancellationToken ct);
        Task<ItemDto> RemoveTagAsync(Guid itemId, Guid tagId, CancellationToken ct);
    }
}
