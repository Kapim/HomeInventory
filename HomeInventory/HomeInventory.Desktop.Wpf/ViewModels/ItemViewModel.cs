using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class ItemViewModel : ObservableObject
    {
        private readonly IItemsService _items;
        private readonly IDialogService _dialogs;
        public readonly Item? Item;
        [ObservableProperty]
        private string? name;
        [ObservableProperty]
        private string? placementNote, description;
        private readonly Func<ItemViewModel, string, Task> _itemNameChanged;
        private readonly Func<ItemViewModel, string?, Task> _itemDescriptionChanged;
        private readonly Func<ItemViewModel, string?, Task> _itemPlacementNoteChanged;

        public ItemViewModel(IItemsService items, 
            IDialogService dialogService, 
            Func<ItemViewModel, string, Task> itemNameChangedCallback,
            Func<ItemViewModel, string?, Task> itemDescriptionChanged,
            Func<ItemViewModel, string?, Task> itemPlacementNoteChanged,
        Item? item = null)
        {         
            _items = items;
            _dialogs = dialogService;
            _itemNameChanged = itemNameChangedCallback;
            _itemDescriptionChanged = itemDescriptionChanged;
            _itemPlacementNoteChanged = itemPlacementNoteChanged;
            Item = item;
            if (Item != null)
            {
                name = Item.Name;
                placementNote = Item.PlacementNote;
                description = Item.Description;
            }
        }

        async partial void OnNameChanged(string? value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            await _itemNameChanged(this, value);          

        }

        async partial void OnDescriptionChanged(string? value)
        {
            await _itemDescriptionChanged(this, value);
            
        }

        async partial void OnPlacementNoteChanged(string? value)
        {
            await _itemPlacementNoteChanged(this, value);
        }

        public static ItemUpdateRequest GetUpdateRequest(Item item) =>
            new(item.Name, item.Description, item.Quantity, item.PlacementNote, item.LocationId);

    }
}
