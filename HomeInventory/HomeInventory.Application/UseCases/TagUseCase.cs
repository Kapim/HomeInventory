using HomeInventory.Application.Interfaces;
using HomeInventory.Application.Models;
using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{
    public class TagUseCase(ITagRepository tags, IItemRepository items) : ITagUseCase
    {
        public async Task<Tag> CreateTagAsync(TagCreateRequest request, CancellationToken ct = default)
        {
            var tag = new Tag(request.Name, request.Color, request.HouseholdId);
            return await tags.AddAsync(tag, ct);
        }

        public Task<IReadOnlyList<Tag>> GetTagsForHouseholdAsync(Guid householdId, CancellationToken ct = default)
            => tags.GetByHouseholdAsync(householdId, ct);

        public Task<Tag?> GetTagAsync(Guid tagId, CancellationToken ct = default)
            => tags.GetByIdAsync(tagId, ct);

        public async Task DeleteTagAsync(Guid tagId, CancellationToken ct = default)
        {
            _ = await tags.GetByIdAsync(tagId, ct) ?? throw new KeyNotFoundException(nameof(tagId));
            await tags.DeleteAsync(tagId, ct);
        }

        public async Task<Item> AssignTagToItemAsync(Guid itemId, Guid tagId, CancellationToken ct = default)
        {
            var item = await items.GetByIdAsync(itemId, ct) ?? throw new KeyNotFoundException(nameof(itemId));
            var tag = await tags.GetByIdAsync(tagId, ct) ?? throw new KeyNotFoundException(nameof(tagId));
            item.AddTag(tag);
            return await items.UpdateAsync(item, ct);
        }

        public async Task<Item> RemoveTagFromItemAsync(Guid itemId, Guid tagId, CancellationToken ct = default)
        {
            var item = await items.GetByIdAsync(itemId, ct) ?? throw new KeyNotFoundException(nameof(itemId));
            item.RemoveTag(tagId);
            return await items.UpdateAsync(item, ct);
        }
    }
}
