using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class ItemViewModel : ObservableObject
    {
        public Item? Item { get; private set; }
        [ObservableProperty]
        private string? name;
        [ObservableProperty]
        private string? placementNote, description;
        [ObservableProperty]
        private int quantity;
        private readonly Func<ItemViewModel, string?, Task> _itemNameChanged;
        private readonly Func<ItemViewModel, string?, Task> _itemDescriptionChanged;
        private readonly Func<ItemViewModel, string?, Task> _itemPlacementNoteChanged;
        private readonly Func<ItemViewModel, int, Task> _itemQuantityChanged;

        public bool IsNew => Item == null;

        private bool _suprressNextOnChange = false;

        public void SupressNextOnChange()
        {
            _suprressNextOnChange = true;
        }

        public ItemViewModel(Func<ItemViewModel, string?, Task> itemNameChangedCallback,
            Func<ItemViewModel, string?, Task> itemDescriptionChanged,
            Func<ItemViewModel, string?, Task> itemPlacementNoteChanged,
            Func<ItemViewModel, int, Task> itemQuantityChanged,
            Item? item = null)
        {         
            _itemNameChanged = itemNameChangedCallback;
            _itemDescriptionChanged = itemDescriptionChanged;
            _itemPlacementNoteChanged = itemPlacementNoteChanged;
            _itemQuantityChanged = itemQuantityChanged;
            Quantity = 1;
            Item = item;
            if (Item != null)
            {
                name = Item.Name;
                placementNote = Item.PlacementNote;
                description = Item.Description;
                quantity = Item.Quantity;
            }
        }

        async partial void OnNameChanged(string? value)
        {
            if (_suprressNextOnChange)
            {
                _suprressNextOnChange = false;
                return;
            }
            await _itemNameChanged(this, value);        
        }

        async partial void OnDescriptionChanged(string? value)
        {
            if (_suprressNextOnChange)
            {
                _suprressNextOnChange = false;
                return;
            }
            await _itemDescriptionChanged(this, value);            
        }

        async partial void OnPlacementNoteChanged(string? value)
        {
            if (_suprressNextOnChange)
            {
                _suprressNextOnChange = false;
                return;
            }
            await _itemPlacementNoteChanged(this, value);
        }

        async partial void OnQuantityChanged(int value)
        {
            if (_suprressNextOnChange)
            {
                _suprressNextOnChange = false;
                return;
            }
            await _itemQuantityChanged(this, value);
        }

        public static ItemUpdateRequest GetUpdateRequest(Item item) =>
            new(item.Name, item.Description, item.Quantity, item.PlacementNote, item.LocationId);

        public void SetItem(Item item)
        {
            Item = item;
            Name = item.Name;
            PlacementNote = item.PlacementNote;
            Quantity = item.Quantity;
            Description = item.Description;
        }

    }
}
