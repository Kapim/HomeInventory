using HomeInventory.Client.Models;
using HomeInventory.Contracts;

namespace HomeInventory.Client.Requests
{
    public sealed record LocationCreateRequest(string Name, LocationType LocationType, Guid? ParentLocationId, int SortOrder, string? Description, Guid HouseholdId);

}
