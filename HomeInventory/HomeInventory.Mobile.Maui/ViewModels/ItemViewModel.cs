using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using System.Diagnostics;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class ItemViewModel : ObservableObject
{
    public Item? Item { get; private set; }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? placementNote;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private bool isSelected;

    private readonly Func<ItemViewModel, string?, Task> _nameChanged;
    private readonly Func<ItemViewModel, string?, Task> _descriptionChanged;
    private readonly Func<ItemViewModel, string?, Task> _placementNoteChanged;
    private readonly Func<ItemViewModel, int, Task> _quantityChanged;
    private readonly Func<ItemViewModel, bool, Task> _selectedChanged;

    public bool IsNew => Item is null;
    private bool _suppressNextOnChange;

    public ItemViewModel(
        Func<ItemViewModel, string?, Task> nameChanged,
        Func<ItemViewModel, string?, Task> descriptionChanged,
        Func<ItemViewModel, string?, Task> placementNoteChanged,
        Func<ItemViewModel, int, Task> quantityChanged,
        Func<ItemViewModel, bool, Task> selectedChanged,
        Item? item = null)
    {
        _nameChanged = nameChanged;
        _descriptionChanged = descriptionChanged;
        _placementNoteChanged = placementNoteChanged;
        _quantityChanged = quantityChanged;
        _selectedChanged = selectedChanged;
        Quantity = 1;
        Item = item;
        if (item is not null)
        {
            name = item.Name;
            placementNote = item.PlacementNote;
            description = item.Description;
            quantity = item.Quantity;
        }
    }

    public void SuppressNextOnChange() => _suppressNextOnChange = true;

    public void SetItem(Item item)
    {
        Item = item;
        Name = item.Name;
        PlacementNote = item.PlacementNote;
        Quantity = item.Quantity;
        Description = item.Description;
    }

    public static ItemUpdateRequest GetUpdateRequest(Item item)
        => new(item.Name, item.Description, item.Quantity, item.PlacementNote, item.LocationId);

    async partial void OnNameChanged(string? value)
    {
        if (CheckSuppress()) return;
        try { await _nameChanged(this, value); }
        catch (Exception ex) { Debug.WriteLine($"[ItemViewModel] OnNameChanged: {ex.Message}"); }
    }

    async partial void OnDescriptionChanged(string? value)
    {
        if (CheckSuppress()) return;
        try { await _descriptionChanged(this, value); }
        catch (Exception ex) { Debug.WriteLine($"[ItemViewModel] OnDescriptionChanged: {ex.Message}"); }
    }

    async partial void OnPlacementNoteChanged(string? value)
    {
        if (CheckSuppress()) return;
        try { await _placementNoteChanged(this, value); }
        catch (Exception ex) { Debug.WriteLine($"[ItemViewModel] OnPlacementNoteChanged: {ex.Message}"); }
    }

    async partial void OnQuantityChanged(int value)
    {
        if (CheckSuppress()) return;
        try { await _quantityChanged(this, value); }
        catch (Exception ex) { Debug.WriteLine($"[ItemViewModel] OnQuantityChanged: {ex.Message}"); }
    }

    async partial void OnIsSelectedChanged(bool value)
    {
        if (CheckSuppress()) return;
        try { await _selectedChanged(this, value); }
        catch (Exception ex) { Debug.WriteLine($"[ItemViewModel] OnIsSelectedChanged: {ex.Message}"); }
    }

    private bool CheckSuppress()
    {
        if (!_suppressNextOnChange) return false;
        _suppressNextOnChange = false;
        return true;
    }
}
