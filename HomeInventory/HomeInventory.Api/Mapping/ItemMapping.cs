using HomeInventory.Contracts;
using HomeInventory.Domain;

namespace HomeInventory.Api.Mapping
{
    public static class ItemMapping
    {
        public static ItemDto Map(Item item) =>
            new(item.Id, 
                item.Name, 
                item.Quantity, 
                item.LocationId, 
                item.OwnerUserId, 
                item.PlacementNote, 
                item.Description);
    }
}
