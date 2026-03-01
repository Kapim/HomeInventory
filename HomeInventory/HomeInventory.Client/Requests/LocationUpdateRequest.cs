using HomeInventory.Client.Models;

namespace HomeInventory.Client.Requests
{
    public sealed record LocationUpdateRequest(string Name, LocationType LocationType, Guid? ParentLocationId, int SortOrder, string? Description);

}
