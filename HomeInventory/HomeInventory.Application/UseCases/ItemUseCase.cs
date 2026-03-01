using HomeInventory.Application.Interfaces;
using HomeInventory.Application.Models;
using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public class ItemUseCase(IItemRepository items) : IItemUseCase
    {

        private readonly IItemRepository _items = items;


        public async Task<Item> AddItemAsync(ItemCreateRequest request, CancellationToken ct)
        {
            Item item = new(request.Name, request.OwnerId, request.LocationId);
            item.Quantity = request.Quantity;
            item.PlacementNote = request.PlacementNote;
            item.Description = request.Description;
            await _items.AddAsync(item, ct);
            return item;
        }

        public async Task DeleteItemAsync(Guid id, CancellationToken ct)
        {
            await _items.DeleteAsync(id, ct);
        }      

        public async Task<Item?> GetItemAsync(Guid itemId, CancellationToken ct)
        {
            return await _items.GetByIdAsync(itemId, ct);
        }

        public async Task<IReadOnlyList<Item>> SearchAsync(string name, CancellationToken ct)
        {
            return await _items.FindByNameAsync(name, ct);
        }

        public async Task<Item> UpdateItemAsync(Guid id, ItemUpdateRequest request, CancellationToken ct)
        {
            var item = await _items.GetByIdAsync(id, ct);
            if (item != null)
            { 
                item.Rename(request.Name);
                item.Description = request.Description;
                item.Quantity = request.Quantinty;
                item.PlacementNote = request.PlacementNote;
                item.MoveToLocation(request.LocationId);
                return await _items.UpdateAsync(item, ct);
            } else
            {
                throw new KeyNotFoundException(nameof(id));
            }

        }
    }
}
