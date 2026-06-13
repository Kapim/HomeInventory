using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Services
{
    public class TagsService(ITagsApiClient api) : ITagsService
    {
        public async Task<IReadOnlyList<Tag>> GetTagsForHouseholdAsync(Guid householdId, CancellationToken ct = default)
        {
            var dtos = await api.GetTagsForHouseholdAsync(householdId, ct);
            return dtos.Select(d => new Tag(d.Id, d.Name, d.Color)).ToList();
        }

        public async Task<Tag> CreateTagAsync(string name, string color, Guid householdId, CancellationToken ct = default)
        {
            var dto = await api.CreateTagAsync(new CreateTagRequestDto(name, color, householdId), ct);
            return new Tag(dto.Id, dto.Name, dto.Color);
        }

        public Task DeleteTagAsync(Guid tagId, CancellationToken ct = default)
            => api.DeleteTagAsync(tagId, ct);

        public async Task<Item> AssignTagAsync(Guid itemId, Guid tagId, CancellationToken ct = default)
        {
            var dto = await api.AssignTagAsync(itemId, tagId, ct);
            return ItemMapping.Map(dto);
        }

        public async Task<Item> RemoveTagAsync(Guid itemId, Guid tagId, CancellationToken ct = default)
        {
            var dto = await api.RemoveTagAsync(itemId, tagId, ct);
            return ItemMapping.Map(dto);
        }
    }
}
