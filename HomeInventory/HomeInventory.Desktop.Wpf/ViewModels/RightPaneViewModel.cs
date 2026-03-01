using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using System.Collections.ObjectModel;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class RightPaneViewModel(ILocationsService locations, IItemsService items, IDialogService dialogs) : ObservableObject
    {
        public ObservableCollection<ItemViewModel> Items { get; } = [];

        private readonly ILocationsService _locations = locations;
        private readonly IItemsService _items = items;
        private readonly IDialogService _dialogs = dialogs;
        private LocationNodeViewModel? _location;
        [ObservableProperty]
        private ItemViewModel? selectedItem;

        public async Task LoadAsync(LocationNodeViewModel location, CancellationToken ct = default)
        {
            Items.Clear();
            _location = location;
            var items = await _locations.GetItemsAsync(location.Id, ct);
            foreach (var item in items)
            {
                Items.Add(new ItemViewModel(_items, _dialogs, ItemNameChanged, ItemDescriptionChanged, ItemPlaceNoteChanged, item));
            }
        }

        public void AddNewItem()
        {
            var row = new ItemViewModel(_items, _dialogs, ItemNameChanged, ItemDescriptionChanged, ItemPlaceNoteChanged, null);
            Items.Add(row);
            SelectedItem = row; 
        }

        private async Task ItemNameChanged(ItemViewModel itemViewModel, string newName)
        {
            if (_location is null)
                throw new NullReferenceException("Location was not loaded!");
            try
            {
                if (itemViewModel.Item == null)
                    await _items.CreateAsync(new(newName, 0, _location.Id, _location.Id, default, default), new CancellationTokenSource().Token);
                else
                {
                    itemViewModel.Item.ChangeName(newName);
                    await _items.UpdateAsync(itemViewModel.Item.Id, ItemViewModel.GetUpdateRequest(itemViewModel.Item), new CancellationTokenSource().Token);
                }
            }
            catch (ArgumentException ex)
            {
                _dialogs.ShowError("Failed to update description", ex.Message);
            }
        }

        private async Task ItemDescriptionChanged(ItemViewModel itemViewModel, string? newDescription)
        {
            if (itemViewModel.Item == null)
                throw new ArgumentNullException("itemViewModel.Item");
            try
            {
                itemViewModel.Item.Description = newDescription;
                await _items.UpdateAsync(itemViewModel.Item.Id, ItemViewModel.GetUpdateRequest(itemViewModel.Item), new CancellationTokenSource().Token);
            }
            catch (ArgumentException ex)
            {
                _dialogs.ShowError("Failed to update description", ex.Message);
            }
        }

        private async Task ItemPlaceNoteChanged(ItemViewModel itemViewModel, string? newPlaceNote)
        {
            if (itemViewModel.Item == null)
                throw new ArgumentNullException("itemViewModel.Item");
            try
            {
                itemViewModel.Item.PlacementNote = newPlaceNote;
                await _items.UpdateAsync(itemViewModel.Item.Id, ItemViewModel.GetUpdateRequest(itemViewModel.Item), new CancellationTokenSource().Token);
            }
            catch (ArgumentException ex)
            {
                _dialogs.ShowError("Failed to update description", ex.Message);
            }
        }
    }
}
