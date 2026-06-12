namespace HomeInventory.Application.Models
{
    public sealed record LocationMoveRequest(Guid? NewParentLocationId, int? SortOrder = null);
}
