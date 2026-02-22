using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public class ItemUseCase(IItemRepository items) : IItemUseCase
    {

        IItemRepository _items = items;

        public async Task AddItem(string name, Guid ownerUserId, Guid locationId)
        {
            Item item = new(name, ownerUserId, locationId);
            await _items.AddAsync(item, new CancellationTokenSource().Token);
        }

        public async Task<Item?> GetItem(Guid itemId)
        {
            return await _items.GetByIdAsync(itemId, new CancellationTokenSource().Token);
        }
    }
}
