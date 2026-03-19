using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using HomeInventory.Desktop.Wpf.Enums;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class RightPaneViewModel : ObservableObject
    {
        public ObservableCollection<ItemViewModel> Items { get; } = [];

        private readonly ObservableCollection<ItemViewModel> selectedItems = [];

        private readonly ILocationsService _locations;
        private readonly IItemsService _items;
        private readonly IDialogService _dialogs;
        private readonly IErrorLocalizer _errorLocalizer;
        private readonly INotificationsService _notifications;
        private LocationNodeViewModel? _location;
        [ObservableProperty]
        private ItemViewModel? selectedItem;
        private bool addingNewItem = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveToLocationCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private bool isBusy = false;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveToLocationCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private bool isSelectingNewLocation = false;

        public event EventHandler? SelectNewLocationForItemsEvent;


        private bool ItemsCanBeManipulated => !IsSelectingNewLocation && !IsBusy && selectedItems.Count > 0;
        private bool ItemCanBeAdded => !IsSelectingNewLocation && !IsBusy;

        public RightPaneViewModel(ILocationsService locations, IItemsService items, IDialogService dialogs, IErrorLocalizer errorLocalizer, INotificationsService notifications)
        {
            _locations = locations;
            _items = items;
            _dialogs = dialogs;
            _errorLocalizer = errorLocalizer;
            _notifications = notifications;
            selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private void SelectedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DeleteItemCommand.NotifyCanExecuteChanged();
            MoveToLocationCommand.NotifyCanExecuteChanged();
        }

        public void ShowSaved()
        {
            _notifications.Success("Uloženo");
        }

        public async Task LoadAsync(LocationNodeViewModel location, CancellationToken ct = default)
        {
            IsBusy = true;
            Items.Clear();
            selectedItems.Clear();
            addingNewItem = false;
            _location = location;
            try
            {
                var items = await _locations.GetItemsAsync(location.Id, ct);
                foreach (var item in items)
                {
                    Items.Add(new ItemViewModel(ItemNameChanged, ItemDescriptionChanged, ItemPlaceNoteChanged, ItemQuantityChanged, ItemSelectedChanged, item));
                }
            }
            catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            } finally
            {
                IsBusy = false;
            }
        }

        public void AddNewItem()
        {
            if (addingNewItem)
            {
                _dialogs.ShowInfo("Failed to add a new item", "Finish a new item first by adding a name");
                return;
            }
            var row = new ItemViewModel(ItemNameChanged, ItemDescriptionChanged, ItemPlaceNoteChanged, ItemQuantityChanged, ItemSelectedChanged, null);
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
                    if (itemViewModel.IsSelected)
                        selectedItems.Add(itemViewModel);
                    ShowSaved();
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
                var item = itemViewModel.Item!;

                try
                {
                    var request = new ItemUpdateRequest(newName, item.Description, item.Quantity, item.PlacementNote, item.LocationId);                   
                    
                    var updatedItem = await RunBusy(() => _items.UpdateAsync(itemViewModel.Item!.Id, request, new CancellationTokenSource().Token));
                    itemViewModel.SetItem(updatedItem);
                    ShowSaved();
                }
                catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
                {
                    itemViewModel.SupressNextOnChange();
                    itemViewModel.Name = item.Name;
                    string message = ex.Message;
                    if (ex is ApiException apiEx)
                        message = _errorLocalizer.GetString(apiEx.Type);
                    _dialogs.ShowError("Operace selhala", message);
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

            var item = itemViewModel.Item!;
            try 
            {
                var request = new ItemUpdateRequest(item.Name, newDescription, item.Quantity, item.PlacementNote, item.LocationId);

                var updatedItem = await RunBusy(() => _items.UpdateAsync(itemViewModel.Item!.Id, request, new CancellationTokenSource().Token));
                itemViewModel.SetItem(updatedItem);
                ShowSaved();
            }
            catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
            {
                itemViewModel.SupressNextOnChange();
                itemViewModel.Description = item.Description;
                string message = ex.Message;
                if (ex is ApiException apiEx)
                    message = _errorLocalizer.GetString(apiEx.Type);
                _dialogs.ShowError("Operace selhala", message);
            }

        }

        private async Task ItemPlaceNoteChanged(ItemViewModel itemViewModel, string? newPlaceNote)
        {
            if (itemViewModel.IsNew)
            {
                itemViewModel.PlacementNote = newPlaceNote;
                return; 
            }

            var item = itemViewModel.Item!;
            try
            {

                var request = new ItemUpdateRequest(item.Name, item.Description, item.Quantity, newPlaceNote, item.LocationId);
                var updatedItem = await RunBusy(() => _items.UpdateAsync(itemViewModel.Item!.Id, request, new CancellationTokenSource().Token));
                itemViewModel.SetItem(updatedItem);
                ShowSaved();
            }
            catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
            {
                itemViewModel.SupressNextOnChange();
                itemViewModel.PlacementNote = item.PlacementNote;
                string message = ex.Message;
                if (ex is ApiException apiEx)
                    message = _errorLocalizer.GetString(apiEx.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
        }

        private async Task ItemQuantityChanged(ItemViewModel itemViewModel, int newQuantinty)
        {
            if (itemViewModel.IsNew)
            {
                itemViewModel.Quantity = newQuantinty;
                return;
            }
            var item = itemViewModel.Item!;
            try
            {
                var originalValue = item.Quantity;
                var request = new ItemUpdateRequest(item.Name, item.Description, newQuantinty, item.PlacementNote, item.LocationId);
                var updatedItem = await RunBusy(() => _items.UpdateAsync(itemViewModel.Item!.Id, request, new CancellationTokenSource().Token));
                itemViewModel.SetItem(updatedItem);
                ShowSaved();
            }
            catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
            {
                itemViewModel.SupressNextOnChange();
                itemViewModel.Quantity = item.Quantity;
                string message = ex.Message;
                if (ex is ApiException apiEx)
                    message = _errorLocalizer.GetString(apiEx.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
        }

        private async Task ItemSelectedChanged(ItemViewModel itemViewModel, bool isSelected)
        {
            if (itemViewModel.IsNew)
                return;
            if (isSelected)
                selectedItems.Add(itemViewModel);
            else
                selectedItems.Remove(itemViewModel);
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

        [RelayCommand(CanExecute = nameof(ItemsCanBeManipulated))]
        public async Task DeleteItem()
        {
            var itemsToDeleteCount = selectedItems.Count;
            if (itemsToDeleteCount == 0)
                return;
            string message = $"Do you want to delete {selectedItems.Count} ";
            if (itemsToDeleteCount == 1)
                message += "item?";
            else
                message += "items?";
            if (_dialogs.ShowConfirmationDialog("Delete items", message) == DialogResult.Yes)
            {
                IsBusy = true;
                try
                {
                    List<Task> tasks = [];
                    foreach (var itemVM in selectedItems)
                        tasks.Add(_items.DeleteAsync(itemVM.Item!.Id, new CancellationTokenSource().Token));
                    await Task.WhenAll(tasks);

                    _notifications.Success($"Successfully deleted {itemsToDeleteCount} items.");
                } catch (ApiException ex)
                {
                    message = _errorLocalizer.GetString(ex.Type);
                    _dialogs.ShowError("Operace selhala", message);
                } finally
                {
                    await LoadAsync(_location!);
                    IsBusy = false;

                }
            }
            
        }

        [RelayCommand(CanExecute = nameof(ItemCanBeAdded))]
        public void AddItem()
        {
            AddNewItem();
        }

        [RelayCommand(CanExecute = nameof(ItemsCanBeManipulated))]
        public void MoveToLocation()
        {
            IsSelectingNewLocation = true;
            SelectNewLocationForItemsEvent?.Invoke(this, new EventArgs());
        }

        public async Task MoveSelectedItemsToLocation(LocationNodeViewModel location)
        {
            
            IsSelectingNewLocation = false;
            var itemsToMoveCount = selectedItems.Count;
            if (itemsToMoveCount == 0)
                return;
                        
            IsBusy = true;
            try
            {
                int failedToMoveCount = 0;
                foreach (var itemVM in selectedItems)
                {
                    var item = itemVM.Item!;
                    ItemUpdateRequest request = new(item.Name, item.Description, item.Quantity, item.PlacementNote, location.Id);
                    try
                    {
                        await _items.UpdateAsync(itemVM.Item!.Id, request, new CancellationTokenSource().Token);
                    }
                    catch (ApiException)
                    {
                        ++failedToMoveCount;
                    }
                }
                if (failedToMoveCount > 0)
                {
                    _notifications.Success($"Successfully moved {itemsToMoveCount - failedToMoveCount} items out of {itemsToMoveCount} selected items.");
                }
                else
                {
                    _notifications.Success($"Successfully moved {itemsToMoveCount} items.");
                }
            } finally
            {
                await LoadAsync(location, new CancellationTokenSource().Token);
                IsBusy = false;
            }          


        
        }
    }
}
