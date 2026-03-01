namespace HomeInventory.Contracts.Requests
{
    public record CreateLocationRequestDto(string Name, LocationTypeDto LocationType, Guid? ParentLocationId, int SortOrder, string? Description, Guid HouseholdId, Guid OwnerUserId);
}
