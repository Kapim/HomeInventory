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


        public async Task<Item> AddItemAsync(ItemCreateRequest request, Guid ownerId, CancellationToken ct)
        {
            try
            {
                Item item = new(request.Name, ownerId, request.LocationId);
                item.SetPlacementNote(request.PlacementNote);
                item.SetDescription(request.Description);
                item.SetQuantity(request.Quantity);
                await _items.AddAsync(item, ct);
                return item;
            } catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is ArgumentException)
            {
                throw;
            }
            
        }

        public async Task DeleteItemAsync(Guid id, CancellationToken ct)
        {
            try
            {
                await _items.DeleteAsync(id, ct);
            } catch (KeyNotFoundException)
            {
                throw;
            }
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
                try
                {
                    item.Rename(request.Name);
                    item.SetDescription(request.Description);
                    item.SetQuantity(request.Quantinty);
                    item.SetPlacementNote(request.PlacementNote);
                    item.MoveToLocation(request.LocationId);
                    return await _items.UpdateAsync(item, ct);
                } catch (Exception ex) when(ex is ArgumentOutOfRangeException || ex is ArgumentException)
                {
                    throw;
                }
        } else
            {
                throw new KeyNotFoundException(nameof(id));
            }

        }
    }
}
