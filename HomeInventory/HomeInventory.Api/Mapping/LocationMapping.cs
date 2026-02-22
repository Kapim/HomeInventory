using HomeInventory.Contracts;
using HomeInventory.Domain;

namespace HomeInventory.Api.Mapping
{
    public static class LocationMapping
    {
        public static LocationDto Map(Location location) =>
            new(location.Id,
                location.Name,
                LocationTypeMapping.Map(location.LocationType),
                location.ParentLocationId,
                location.HouseholdId,
                location.OwnerUserId,
                location.SortOrder,
                location.Description);
    }
}
