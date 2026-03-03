using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.Models
{
    public sealed record LocationCreateRequest(string Name, LocationType LocationType, Guid? ParentLocationId, int SortOrder, string? Description, Guid HouseholdId, Guid OwnerId);
}
