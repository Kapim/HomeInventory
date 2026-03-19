using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class MockItemsService : IItemsService
    {
        private readonly Dictionary<Guid, Item> _items = [];
        private readonly HashSet<Guid> _updateFailIds = [];
        private readonly HashSet<Guid> _deleteFailIds = [];

        public int DeleteCalls { get; private set; }
        public int UpdateCalls { get; private set; }

        public void GenerateItems(List<Guid> guids, Guid locationId)
        {
            foreach (var guid in guids)
            {
                _items.Add(guid, new(guid, "name", 0, locationId, Guid.NewGuid(), null, null));
            }
        }

        public void FailUpdateFor(Guid id) => _updateFailIds.Add(id);
        public void FailDeleteFor(Guid id) => _deleteFailIds.Add(id);

        public bool Exists(Guid id) => _items.ContainsKey(id);

        public Item? GetLocal(Guid id) => _items.TryGetValue(id, out var item) ? item : null;

        public Task<Item> CreateAsync(ItemCreateRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ApiException(ApiErrorTypes.Validation, "", 400);

            var item = new Item(Guid.NewGuid(), request.Name, request.Quantity, request.LocationId, Guid.NewGuid(), request.PlacementNote, request.Description);
            _items.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task DeleteAsync(Guid id, CancellationToken ct)
        {
            DeleteCalls++;
            if (_deleteFailIds.Contains(id))
                throw new ApiException(ApiErrorTypes.Server, "", 500);

            if (!_items.Remove(id))
                throw new ApiException(ApiErrorTypes.NotFound, "", 404);

            return Task.CompletedTask;
        }

        public Task<Item> GetByIdAsync(Guid id, CancellationToken ct)
        {
            if (_items.TryGetValue(id, out var item))
                return Task.FromResult(item);

            throw new ApiException(ApiErrorTypes.NotFound, "", 404);
        }

        public Task<IReadOnlyList<Item>> SearchAsync(string query, CancellationToken ct)
        {
            IReadOnlyList<Item> result = [.. _items.Values.Where(x => x.Name.Contains(query.Trim()))];
            return Task.FromResult(result);
        }

        public async Task<Item> UpdateAsync(Guid id, ItemUpdateRequest request, CancellationToken ct)
        {
            UpdateCalls++;
            if (_updateFailIds.Contains(id))
                throw new ApiException(ApiErrorTypes.Server, "", 500);

            var item = await GetByIdAsync(id, ct);
            item.ChangeName(request.Name);
            item.PlacementNote = request.PlacementNote;
            item.Description = request.Description;
            item.Quantity = request.Quantinty;
            item.MoveTo(request.LocationId);
            return item;
        }
    }
}
