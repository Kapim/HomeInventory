using HomeInventory.Application.Models;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
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

        public static ItemCreateRequest Map(CreateItemRequestDto request) =>
            new(request.Name,
                request.Quantity,
                request.LocationId,
                request.PlacementNote,
                request.Description);

        public static CreateItemRequestDto Map(ItemCreateRequest request) =>
            new(request.Name,
                request.Quantity,
                request.LocationId,
                request.PlacementNote,
                request.Description);

        public static ItemUpdateRequest Map(UpdateItemRequestDto request) =>
            new(request.Name,
                request.Description,
                request.Quantinty,
                request.PlacementNote,
                request.LocationId);

        public static UpdateItemRequestDto Map(ItemUpdateRequest request) =>
            new(request.Name,
                request.Description,
                request.Quantinty,
                request.PlacementNote,
                request.LocationId);
    }

}
