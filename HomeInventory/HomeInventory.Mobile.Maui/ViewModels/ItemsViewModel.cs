using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using System.Collections.ObjectModel;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class ItemsViewModel : ObservableObject
{
    public ObservableCollection<ItemViewModel> Items { get; } = [];

    private readonly ObservableCollection<ItemViewModel> _selectedItems = [];
    private readonly ILocationsService _locations;
    private readonly IItemsService _items;
    private readonly IDialogService _dialogs;
    private readonly IErrorLocalizer _errorLocalizer;
    private readonly INotificationsService _notifications;

    private Guid _locationId;

    [ObservableProperty]
    private ItemViewModel? selectedItem;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
    private bool hasSelectedItems;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string locationName = "";

    public ItemsViewModel(
        ILocationsService locations,
        IItemsService items,
        IDialogService dialogs,
        IErrorLocalizer errorLocalizer,
        INotificationsService notifications)
    {
        _locations = locations;
        _items = items;
        _dialogs = dialogs;
        _errorLocalizer = errorLocalizer;
        _notifications = notifications;
        _selectedItems.CollectionChanged += (_, _) =>
            HasSelectedItems = _selectedItems.Count > 0;
    }

    public async Task LoadByIdAsync(Guid locationId, string locationName, CancellationToken ct = default)
    {
        IsBusy = true;
        try
        {
            Clear();
            _locationId = locationId;
            LocationName = locationName;
            var items = await _locations.GetItemsAsync(locationId, ct);
            foreach (var item in items)
                Items.Add(CreateItemViewModel(item));
        }
        catch (ApiException ex)
        {
            _dialogs.ShowError("Operace selhala", _errorLocalizer.GetString(ex.Type));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ReloadAsync()
    {
        if (_locationId == Guid.Empty) return;
        await LoadByIdAsync(_locationId, LocationName, CancellationToken.None);
    }

    public void Clear()
    {
        Items.Clear();
        _selectedItems.Clear();
        SelectedItem = null;
    }

    private ItemViewModel CreateItemViewModel(Item? item = null)
        => new(ItemNameChanged, ItemDescriptionChanged, ItemPlacementNoteChanged, ItemQuantityChanged, ItemSelectedChanged, item);

    [RelayCommand]
    private void AddItem()
    {
        var row = CreateItemViewModel();
        Items.Add(row);
        SelectedItem = row;
    }

    [RelayCommand(CanExecute = nameof(HasSelectedItems))]
    private async Task DeleteItem()
    {
        var count = _selectedItems.Count;
        if (count == 0) return;

        var msg = count == 1 ? "Smazat tuto položku?" : $"Smazat {count} vybrané položky?";
        if (!await _dialogs.ShowConfirmationDialog("Smazat", msg)) return;

        IsBusy = true;
        try
        {
            int failed = 0;
            foreach (var vm in _selectedItems.ToList())
            {
                try { await _items.DeleteAsync(vm.Item!.Id, CancellationToken.None); }
                catch (ApiException) { failed++; }
            }

            if (failed > 0)
                _notifications.Warning($"Smazáno {count - failed} z {count} položek.");
            else
                _notifications.Success($"Smazáno {count} položek.");

            await ReloadAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ItemNameChanged(ItemViewModel vm, string? newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            _dialogs.ShowError("Operace selhala", "Jméno musí být vyplněno!");
            if (vm.Item is not null) vm.Name = vm.Item.Name;
            return;
        }
        if (_locationId == Guid.Empty) return;

        if (vm.IsNew)
        {
            try
            {
                var item = await _items.CreateAsync(
                    new ItemCreateRequest(newName, vm.Quantity, _locationId, vm.PlacementNote, vm.Description),
                    CancellationToken.None);
                vm.SetItem(item);
                _notifications.Success("Uloženo");
            }
            catch (Exception ex)
            {
                _dialogs.ShowError("Operace selhala", GetMessage(ex));
            }
        }
        else
        {
            var item = vm.Item!;
            try
            {
                var updated = await _items.UpdateAsync(item.Id,
                    new ItemUpdateRequest(newName, item.Description, item.Quantity, item.PlacementNote, item.LocationId),
                    CancellationToken.None);
                vm.SetItem(updated);
                _notifications.Success("Uloženo");
            }
            catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
            {
                vm.SuppressNextOnChange();
                vm.Name = item.Name;
                _dialogs.ShowError("Operace selhala", GetMessage(ex));
            }
        }
    }

    private async Task ItemDescriptionChanged(ItemViewModel vm, string? value)
    {
        if (vm.IsNew) return;
        var item = vm.Item!;
        try
        {
            var updated = await _items.UpdateAsync(item.Id,
                new ItemUpdateRequest(item.Name, value, item.Quantity, item.PlacementNote, item.LocationId),
                CancellationToken.None);
            vm.SetItem(updated);
            _notifications.Success("Uloženo");
        }
        catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
        {
            vm.SuppressNextOnChange();
            vm.Description = item.Description;
            _dialogs.ShowError("Operace selhala", GetMessage(ex));
        }
    }

    private async Task ItemPlacementNoteChanged(ItemViewModel vm, string? value)
    {
        if (vm.IsNew) return;
        var item = vm.Item!;
        try
        {
            var updated = await _items.UpdateAsync(item.Id,
                new ItemUpdateRequest(item.Name, item.Description, item.Quantity, value, item.LocationId),
                CancellationToken.None);
            vm.SetItem(updated);
            _notifications.Success("Uloženo");
        }
        catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
        {
            vm.SuppressNextOnChange();
            vm.PlacementNote = item.PlacementNote;
            _dialogs.ShowError("Operace selhala", GetMessage(ex));
        }
    }

    private async Task ItemQuantityChanged(ItemViewModel vm, int value)
    {
        if (vm.IsNew) return;
        var item = vm.Item!;
        try
        {
            var updated = await _items.UpdateAsync(item.Id,
                new ItemUpdateRequest(item.Name, item.Description, value, item.PlacementNote, item.LocationId),
                CancellationToken.None);
            vm.SetItem(updated);
            _notifications.Success("Uloženo");
        }
        catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
        {
            vm.SuppressNextOnChange();
            vm.Quantity = item.Quantity;
            _dialogs.ShowError("Operace selhala", GetMessage(ex));
        }
    }

    private Task ItemSelectedChanged(ItemViewModel vm, bool isSelected)
    {
        if (vm.IsNew) return Task.CompletedTask;
        if (isSelected) _selectedItems.Add(vm);
        else _selectedItems.Remove(vm);
        return Task.CompletedTask;
    }

    private string GetMessage(Exception ex)
        => ex is ApiException apiEx ? _errorLocalizer.GetString(apiEx.Type) : ex.Message;
}
