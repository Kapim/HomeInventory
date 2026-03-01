using HomeInventory.Application.Models;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
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

        public static LocationListItemDto MapListItem(Location location) =>
            new(location.Id,
                location.Name,
                LocationTypeMapping.Map(location.LocationType),
                location.ParentLocationId,
                location.SortOrder);

        public static LocationUpdateRequest Map(UpdateLocationRequestDto request) =>
            new(request.Name,
                LocationTypeMapping.Map(request.Type),
                request.parentLocationId,
                request.SortOrder,
                request.Description);
    }
}
