using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class MockItemsService : IItemsService
    {
        private readonly Dictionary<Guid, Item> _items = [];

        public int DeleteCalls { get; private set; }
        public int UpdateCalls { get; private set; }

        public void GenerateItems(List<Guid> guids, Guid locationId)
        {
            foreach (var guid in guids)
            {
                _items.Add(guid, new(guid, "name", 0, locationId, Guid.NewGuid(), null, null));
            }
        }

        public bool Exists(Guid id) => _items.ContainsKey(id);

        public Item? GetLocal(Guid id) => _items.TryGetValue(id, out var item) ? item : null;

        public async Task<Item> CreateAsync(ItemCreateRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ApiException(ApiErrorTypes.Validation, "", 400);
            var item = new Item(Guid.NewGuid(), request.Name, request.Quantity, request.LocationId, Guid.NewGuid(), request.PlacementNote, request.Description);
            _items.Add(item.Id, item);
            return item;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            DeleteCalls++;
            if (!_items.Remove(id))
                throw new ApiException(ApiErrorTypes.NotFound, "", 404);
        }

        public async Task<Item> GetByIdAsync(Guid id, CancellationToken ct)
        {
            if (_items.TryGetValue(id, out var item))
            {
                return item;
            }
            else
            {
                throw new ApiException(ApiErrorTypes.NotFound, "", 404);
            }
        }

        public async Task<IReadOnlyList<Item>> SearchAsync(string query, CancellationToken ct)
        {
            return [.. _items.Select(x => x.Value).Where(x => x.Name.Contains(query.Trim()))];
        }

        public async Task<Item> UpdateAsync(Guid id, ItemUpdateRequest request, CancellationToken ct)
        {
            UpdateCalls++;
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
