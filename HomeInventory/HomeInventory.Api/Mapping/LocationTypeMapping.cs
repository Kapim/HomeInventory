using HomeInventory.Contracts;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Api.Mapping
{
    public static class LocationTypeMapping
    {
        public static LocationTypeDto Map(LocationType locationType) => locationType switch
        {
            LocationType.Furniture => LocationTypeDto.Furniture,
            LocationType.Room => LocationTypeDto.Room,
            LocationType.Zone => LocationTypeDto.Zone,
            LocationType.Container => LocationTypeDto.Container,
            LocationType.Drawer => LocationTypeDto.Drawer,
            LocationType.Shelf => LocationTypeDto.Shelf,
            LocationType.Other => LocationTypeDto.Other,

            _ => throw new ArgumentOutOfRangeException()
        };

        public static LocationType Map(LocationTypeDto locationTypeDto) => locationTypeDto switch
        {
            LocationTypeDto.Furniture => LocationType.Furniture,
            LocationTypeDto.Room => LocationType.Room,
            LocationTypeDto.Zone => LocationType.Zone,
            LocationTypeDto.Container => LocationType.Container,
            LocationTypeDto.Drawer => LocationType.Drawer,
            LocationTypeDto.Shelf => LocationType.Shelf,
            LocationTypeDto.Other => LocationType.Other,

            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
