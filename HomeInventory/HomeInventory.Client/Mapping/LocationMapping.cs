using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Mapping
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

        public static Location Map(LocationDto locationDto) =>
            new(locationDto.Id,
                locationDto.Name,
                LocationTypeMapping.Map(locationDto.LocationType),
                locationDto.ParentLocationId,
                locationDto.HouseholdId,
                locationDto.OwnerUserId,
                locationDto.SortOrder,
                locationDto.Description);

        public static LocationListItem Map(LocationListItemDto locationListItemDto) =>
            new(locationListItemDto.Id,
                locationListItemDto.Name,
                LocationTypeMapping.Map(locationListItemDto.LocationType),
                locationListItemDto.ParentLocationId,
                locationListItemDto.SortOrder);

        public static UpdateLocationRequestDto Map(LocationUpdateRequest locationUpdateRequest) =>
            new(locationUpdateRequest.Name,
                LocationTypeMapping.Map(locationUpdateRequest.LocationType),
                locationUpdateRequest.SortOrder,
                locationUpdateRequest.Description,
                locationUpdateRequest.ParentLocationId);

        public static CreateLocationRequestDto Map(LocationCreateRequest request) =>
            new(request.Name,
                LocationTypeMapping.Map(request.LocationType),
                request.ParentLocationId,
                request.SortOrder,
                request.Description,
                request.HouseholdId);

        public static LocationCreateRequest Map(CreateLocationRequestDto request) =>
            new(request.Name,
                LocationTypeMapping.Map(request.LocationType),
                request.ParentLocationId,
                request.SortOrder,
                request.Description,
                request.HouseholdId);
    }
}