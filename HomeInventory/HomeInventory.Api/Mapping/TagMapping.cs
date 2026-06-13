using HomeInventory.Application.Models;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using HomeInventory.Domain;

namespace HomeInventory.Api.Mapping
{
    public static class TagMapping
    {
        public static TagDto Map(Tag tag) => new(tag.Id, tag.Name, tag.Color);

        public static TagCreateRequest Map(CreateTagRequestDto dto) =>
            new(dto.Name, dto.Color, dto.HouseholdId);
    }
}
