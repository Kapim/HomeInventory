namespace HomeInventory.Contracts.Requests
{
    public sealed record MoveLocationRequestDto(Guid? NewParentLocationId, int? SortOrder = null);
}
