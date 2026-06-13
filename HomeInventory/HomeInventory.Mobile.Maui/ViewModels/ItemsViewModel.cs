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
    private readonly IHouseholdsService _householdsService;
    private readonly ILocationsService _locations;
    private readonly IItemsService _items;
    private readonly ITagsService _tags;
    private readonly IDialogService _dialogs;
    private readonly IErrorLocalizer _errorLocalizer;
    private readonly INotificationsService _notifications;
    private readonly IConnectivityService _connectivity;
    private readonly ISessionState _session;

    private Guid _locationId;

    private static readonly string[] _tagColorOptions =
    [
        "#EF4444", "#F97316", "#EAB308", "#22C55E",
        "#3B82F6", "#8B5CF6", "#EC4899", "#6B7280"
    ];

    [ObservableProperty]
    private ItemViewModel? selectedItem;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
    [NotifyCanExecuteChangedFor(nameof(MoveItemCommand))]
    private bool hasSelectedItems;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string locationName = "";

    public bool IsOffline => !_connectivity.IsOnline;

    public ItemsViewModel(
        IHouseholdsService householdsService,
        ILocationsService locations,
        IItemsService items,
        ITagsService tags,
        IDialogService dialogs,
        IErrorLocalizer errorLocalizer,
        INotificationsService notifications,
        IConnectivityService connectivity,
        ISessionState session)
    {
        _householdsService = householdsService;
        _locations = locations;
        _items = items;
        _tags = tags;
        _dialogs = dialogs;
        _errorLocalizer = errorLocalizer;
        _notifications = notifications;
        _connectivity = connectivity;
        _session = session;
        _selectedItems.CollectionChanged += (_, _) =>
            HasSelectedItems = _selectedItems.Count > 0;
        _connectivity.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IConnectivityService.IsOnline))
                OnPropertyChanged(nameof(IsOffline));
        };
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

    [RelayCommand(CanExecute = nameof(HasSelectedItems))]
    private async Task MoveItem()
    {
        if (_session.SelectedHouseholdId is null) return;

        IReadOnlyList<LocationListItem> locations;
        try
        {
            locations = await _householdsService.GetLocationsAsync(_session.SelectedHouseholdId.Value, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _dialogs.ShowError("Operace selhala", GetMessage(ex));
            return;
        }

        if (locations.Count == 0)
        {
            _dialogs.ShowError("Chyba", "Žádné dostupné lokace.");
            return;
        }

        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        var locationNames = locations.Select(l => l.Name).ToArray();
        var chosen = await page.DisplayActionSheetAsync("Přesunout do lokace", "Zrušit", null, locationNames);
        if (chosen is null || chosen == "Zrušit") return;

        var target = locations.FirstOrDefault(l => l.Name == chosen);
        if (target is null) return;

        var toMove = _selectedItems.Where(vm => !vm.IsNew && vm.Item is not null).ToList();
        if (toMove.Count == 0) return;

        IsBusy = true;
        int failed = 0;
        try
        {
            foreach (var vm in toMove)
            {
                var item = vm.Item!;
                try
                {
                    await _items.UpdateAsync(item.Id,
                        new ItemUpdateRequest(item.Name, item.Description, item.Quantity, item.PlacementNote, target.Id),
                        CancellationToken.None);
                }
                catch (ApiException) { failed++; }
            }

            if (failed > 0)
                _notifications.Warning($"Přesunuto {toMove.Count - failed} z {toMove.Count} položek.");
            else
                _notifications.Success($"Přesunuto {toMove.Count} položek do '{target.Name}'.");

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

    [RelayCommand]
    private async Task ManageTags(ItemViewModel vm)
    {
        if (vm.IsNew || vm.Item is null || _session.SelectedHouseholdId is null) return;

        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        IReadOnlyList<Tag> householdTags;
        try { householdTags = await _householdsService.GetTagsAsync(_session.SelectedHouseholdId.Value, CancellationToken.None); }
        catch (Exception ex) { _dialogs.ShowError("Operace selhala", GetMessage(ex)); return; }

        var allNames = householdTags.Select(t => $"{'✓'} {t.Name}").ToList();
        var untaggedNames = householdTags
            .Where(t => !vm.Tags.Any(vt => vt.Id == t.Id))
            .Select(t => t.Name)
            .ToArray();
        var taggedNames = vm.Tags.Select(t => $"✓ {t.Name}").ToArray();
        var options = taggedNames.Concat(untaggedNames).ToArray();

        var chosen = await page.DisplayActionSheetAsync($"Štítky: {vm.Name}", "Zavřít", "Nový štítek...", options);
        if (chosen is null || chosen == "Zavřít") return;

        if (chosen == "Nový štítek...")
        {
            await CreateAndAssignTag(vm, page);
            return;
        }

        var cleanName = chosen.TrimStart('✓', ' ');
        var tag = householdTags.FirstOrDefault(t => t.Name == cleanName);
        if (tag is null) return;

        try
        {
            bool isAssigned = vm.Tags.Any(t => t.Id == tag.Id);
            if (isAssigned)
            {
                var updated = await _tags.RemoveTagAsync(vm.Item.Id, tag.Id, CancellationToken.None);
                vm.SetItem(updated);
                _notifications.Success($"Štítek '{tag.Name}' odebrán.");
            }
            else
            {
                var updated = await _tags.AssignTagAsync(vm.Item.Id, tag.Id, CancellationToken.None);
                vm.SetItem(updated);
                _notifications.Success($"Štítek '{tag.Name}' přiřazen.");
            }
        }
        catch (Exception ex) { _dialogs.ShowError("Operace selhala", GetMessage(ex)); }
    }

    private async Task CreateAndAssignTag(ItemViewModel vm, Page page)
    {
        if (_session.SelectedHouseholdId is null || vm.Item is null) return;

        var name = await page.DisplayPromptAsync("Nový štítek", "Zadejte název štítku:", "Vytvořit", "Zrušit",
            maxLength: 30, keyboard: Keyboard.Text);
        if (string.IsNullOrWhiteSpace(name)) return;

        var colorNames = new[] { "Červená", "Oranžová", "Žlutá", "Zelená", "Modrá", "Fialová", "Růžová", "Šedá" };
        var chosenColor = await page.DisplayActionSheetAsync("Barva štítku", "Zrušit", null, colorNames);
        if (chosenColor is null || chosenColor == "Zrušit") return;

        var colorIndex = Array.IndexOf(colorNames, chosenColor);
        var color = colorIndex >= 0 ? _tagColorOptions[colorIndex] : "#6B7280";

        try
        {
            var tag = await _tags.CreateTagAsync(name, color, _session.SelectedHouseholdId.Value, CancellationToken.None);
            var updated = await _tags.AssignTagAsync(vm.Item.Id, tag.Id, CancellationToken.None);
            vm.SetItem(updated);
            _notifications.Success($"Štítek '{name}' vytvořen a přiřazen.");
        }
        catch (Exception ex) { _dialogs.ShowError("Operace selhala", GetMessage(ex)); }
    }

    [RelayCommand]
    private async Task ManageHouseholdTags()
    {
        if (_session.SelectedHouseholdId is null) return;

        var page = Shell.Current?.CurrentPage;
        if (page is null) return;

        IReadOnlyList<Tag> tags;
        try { tags = await _householdsService.GetTagsAsync(_session.SelectedHouseholdId.Value, CancellationToken.None); }
        catch (Exception ex) { _dialogs.ShowError("Operace selhala", GetMessage(ex)); return; }

        var options = tags.Select(t => $"🗑 {t.Name}").Append("+ Nový štítek").ToArray();
        var chosen = await page.DisplayActionSheetAsync("Spravovat štítky domácnosti", "Zavřít", null, options);
        if (chosen is null || chosen == "Zavřít") return;

        if (chosen == "+ Nový štítek")
        {
            await CreateNewHouseholdTag(page);
            return;
        }

        const string trashPrefix = "🗑 ";
        var tagName = chosen.StartsWith(trashPrefix) ? chosen[trashPrefix.Length..] : chosen;
        var tag = tags.FirstOrDefault(t => t.Name == tagName);
        if (tag is null) return;

        if (!await _dialogs.ShowConfirmationDialog("Smazat štítek", $"Smazat štítek '{tag.Name}'? Bude odebrán ze všech položek.")) return;

        try
        {
            await _tags.DeleteTagAsync(tag.Id, CancellationToken.None);
            await ReloadAsync();
            _notifications.Success($"Štítek '{tag.Name}' smazán.");
        }
        catch (Exception ex) { _dialogs.ShowError("Operace selhala", GetMessage(ex)); }
    }

    private async Task CreateNewHouseholdTag(Page page)
    {
        if (_session.SelectedHouseholdId is null) return;

        var name = await page.DisplayPromptAsync("Nový štítek", "Zadejte název štítku:", "Vytvořit", "Zrušit",
            maxLength: 30, keyboard: Keyboard.Text);
        if (string.IsNullOrWhiteSpace(name)) return;

        var colorNames = new[] { "Červená", "Oranžová", "Žlutá", "Zelená", "Modrá", "Fialová", "Růžová", "Šedá" };
        var chosenColor = await page.DisplayActionSheetAsync("Barva štítku", "Zrušit", null, colorNames);
        if (chosenColor is null || chosenColor == "Zrušit") return;

        var colorIndex = Array.IndexOf(colorNames, chosenColor);
        var color = colorIndex >= 0 ? _tagColorOptions[colorIndex] : "#6B7280";

        try
        {
            await _tags.CreateTagAsync(name, color, _session.SelectedHouseholdId.Value, CancellationToken.None);
            _notifications.Success($"Štítek '{name}' vytvořen.");
        }
        catch (Exception ex) { _dialogs.ShowError("Operace selhala", GetMessage(ex)); }
    }

    private string GetMessage(Exception ex)
        => ex is ApiException apiEx ? _errorLocalizer.GetString(apiEx.Type) : ex.Message;
}
