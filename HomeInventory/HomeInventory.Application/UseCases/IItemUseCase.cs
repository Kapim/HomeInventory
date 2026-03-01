using HomeInventory.Application.Models;
using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public interface IItemUseCase
    {
        public Task<Item> AddItemAsync(ItemCreateRequest request, CancellationToken ct);

        public Task<Item?> GetItemAsync(Guid itemId, CancellationToken ct);

        public Task<Item> UpdateItemAsync(Guid id, ItemUpdateRequest request, CancellationToken ct);

        public Task DeleteItemAsync(Guid id, CancellationToken ct);

        public Task<IReadOnlyList<Item>> SearchAsync(string name, CancellationToken ct);
    }
}
