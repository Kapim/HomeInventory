using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;

namespace HomeInventory.Client.Mapping
{
    public static class ItemMapping
    {
        public static ItemDto Map(Item item) =>
            new(item.Id,
                item.Name,
                item.Quantity,
                item.LocationId,
                item.OwnerId,
                item.PlacementNote, 
                item.Description);

        public static Item Map(ItemDto itemDto) =>
            new(itemDto.Id,
                itemDto.Name,
                itemDto.Quantity,
                itemDto.LocationId,
                itemDto.OwnerId,
                itemDto.PlacementNote,
                itemDto.Description);

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

        public static ItemCreateRequest Map(CreateItemRequestDto requestDto) =>
            new(requestDto.Name,
                requestDto.Quantity,
                requestDto.LocationId,
                requestDto.OwnerId,
                requestDto.PlacementNote,
                requestDto.Description);

        public static CreateItemRequestDto Map(ItemCreateRequest request) =>
            new(request.Name,
                request.Quantity,
                request.LocationId,
                request.OwnerId,
                request.PlacementNote,
                request.Description);
    }
}