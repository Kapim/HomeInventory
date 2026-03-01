namespace HomeInventory.Contracts
{
    public sealed record LocationDto(
        Guid Id,
        string Name,
        LocationTypeDto LocationType,
        Guid? ParentLocationId,
        Guid HouseholdId,
        Guid OwnerUserId,
        int SortOrder,
        string? Description
    );
}
