using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Services
{
    internal class ItemsService(IItemsApiClient items) : IItemsService
    {
        private readonly IItemsApiClient _items = items;

        public async Task<Item> CreateAsync(ItemCreateRequest request, CancellationToken ct)
        {
            return ItemMapping.Map(await _items.CreateAsync(ItemMapping.Map(request), ct));
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            await _items.DeleteAsync(id, ct);
        }

        public async Task<Item> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return ItemMapping.Map(await _items.GetByIdAsync(id, ct));
        }

        public async Task<IReadOnlyList<Item>> SearchAsync(string query, CancellationToken ct)
        {
            return [.. (await _items.SearchAsync(query, ct)).Select(x => ItemMapping.Map(x))];
        }

        public async Task<Item> UpdateAsync(Guid id, ItemUpdateRequest request, CancellationToken ct)
        {
            return ItemMapping.Map(await _items.UpdateAsync(id, ItemMapping.Map(request), ct));
        }
    }
}
