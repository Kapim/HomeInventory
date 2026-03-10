using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using System.Collections.ObjectModel;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class RightPaneViewModel(ILocationsService locations, IItemsService items, IDialogService dialogs, IErrorLocalizer errorLocalizer) : ObservableObject
    {
        public ObservableCollection<ItemViewModel> Items { get; } = [];

        private readonly ILocationsService _locations = locations;
        private readonly IItemsService _items = items;
        private readonly IDialogService _dialogs = dialogs;
        private readonly IErrorLocalizer _errorLocalizer = errorLocalizer;
        private LocationNodeViewModel? _location;
        [ObservableProperty]
        private ItemViewModel? selectedItem;
        private bool addingNewItem = false;
        [ObservableProperty]
        private bool isBusy = false;


        public async Task LoadAsync(LocationNodeViewModel location, CancellationToken ct = default)
        {
            Items.Clear();
            addingNewItem = false;
            _location = location;
            try
            {
                var items = await _locations.GetItemsAsync(location.Id, ct);
                foreach (var item in items)
                {
                    Items.Add(new ItemViewModel(ItemNameChanged, ItemDescriptionChanged, ItemPlaceNoteChanged, ItemQuantityChanged, item));
                }
            }
            catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
        }

        public void AddNewItem()
        {
            if (addingNewItem)
            {
                _dialogs.ShowInfo("Failed to add a new item", "Finish a new item first by adding a name");
                return;
            }
            var row = new ItemViewModel(ItemNameChanged, ItemDescriptionChanged, ItemPlaceNoteChanged, ItemQuantityChanged, null);
            Items.Add(row);
            SelectedItem = row;
            addingNewItem = true;
        }

        private async Task ItemNameChanged(ItemViewModel itemViewModel, string? newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                _dialogs.ShowError("Operace selhala", "Jméno musí být vyplněno!");
                if (itemViewModel.Item != null)
                    itemViewModel.Name = itemViewModel.Item.Name;
                return;
            }
            if (_location is null)
                throw new NullReferenceException("Location was not loaded!");
           
            if (itemViewModel.IsNew)
            {
                try
                {
                    var item = await RunBusy(() => _items.CreateAsync(new(newName, itemViewModel.Quantity, _location.Id, itemViewModel.PlacementNote, itemViewModel.Description), new CancellationTokenSource().Token));
                    itemViewModel.SetItem(item);
                    addingNewItem = false;
                    //fast adding of new items
                    AddNewItem();
                    return;
                }
                catch (ApiException ex)
                {
                    var message = _errorLocalizer.GetString(ex.Type);
                    _dialogs.ShowError("Operace selhala", message);
                }
                catch (InvalidOperationException ex)
                {
                    _dialogs.ShowError("Operace selhala", ex.Message);
                }
            }
            else
            {
                try
                {
                    itemViewModel.Item!.ChangeName(newName);
                } catch (ArgumentException ex)
                {
                    _dialogs.ShowError("Operace selhala", ex.Message);
                }
                try
                { 
                    await RunBusy(() => _items.UpdateAsync(itemViewModel.Item!.Id, ItemViewModel.GetUpdateRequest(itemViewModel.Item), new CancellationTokenSource().Token));
                }
                catch (ApiException ex)
                {
                    var message = _errorLocalizer.GetString(ex.Type);
                    _dialogs.ShowError("Operace selhala", message);
                }
                catch (InvalidOperationException ex)
                {
                    _dialogs.ShowError("Operace selhala", ex.Message);
                }
            }          
            
        }

        private async Task ItemDescriptionChanged(ItemViewModel itemViewModel, string? newDescription)
        {
            if (itemViewModel.IsNew)
            {
                itemViewModel.Description = newDescription;
                return;
            }
            
            itemViewModel.Item!.Description = newDescription;
            try 
            { 
                await RunBusy(() => _items.UpdateAsync(itemViewModel.Item.Id, ItemViewModel.GetUpdateRequest(itemViewModel.Item), new CancellationTokenSource().Token));
            }
            catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
            catch (InvalidOperationException ex)
            {
                _dialogs.ShowError("Operace selhala", ex.Message);
            }

        }

        private async Task ItemPlaceNoteChanged(ItemViewModel itemViewModel, string? newPlaceNote)
        {
            if (itemViewModel.IsNew)
            {
                itemViewModel.PlacementNote = newPlaceNote;
                return; 
            }
            
            itemViewModel.Item!.PlacementNote = newPlaceNote;
            try
            {
                await RunBusy(() => _items.UpdateAsync(itemViewModel.Item.Id, ItemViewModel.GetUpdateRequest(itemViewModel.Item), new CancellationTokenSource().Token));
            }
            catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
            catch (InvalidOperationException ex)
            {
                _dialogs.ShowError("Operace selhala", ex.Message);
            }
        }

        private async Task ItemQuantityChanged(ItemViewModel itemViewModel, int newQuantinty)
        {
            if (itemViewModel.IsNew)
            {
                itemViewModel.Quantity = newQuantinty;
                return;
            }

            itemViewModel.Item!.Quantity = newQuantinty;
            try
            {
                var request = ItemViewModel.GetUpdateRequest(itemViewModel.Item);
                await RunBusy(() => _items.UpdateAsync(itemViewModel.Item.Id, request, new CancellationTokenSource().Token));
            }
            catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            } catch (InvalidOperationException ex)
            {
                _dialogs.ShowError("Operace selhala", ex.Message);
            }
        }

        private async Task<T> RunBusy<T>(Func<Task<T>> action)
        {
            if (IsBusy)
                throw new InvalidOperationException("Operation already in progress");
            try
            {
                IsBusy = true;
                return await action();
            } finally {
                IsBusy = false;
            }
        }
    }
}
