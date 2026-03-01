namespace HomeInventory.Contracts
{
    public sealed record LocationListItemDto(
        Guid Id,
        string Name,
        LocationTypeDto LocationType,
        Guid? ParentLocationId,
        int SortOrder
    );
}
