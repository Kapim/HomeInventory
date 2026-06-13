using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using HomeInventory.Mobile.Maui.Services.Navigation;
using System.Collections.ObjectModel;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class LocationsViewModel(
    IHouseholdsService householdsService,
    ILocationsService locationsService,
    IAuthService auth,
    INavigationService nav,
    IDialogService dialogs,
    IErrorLocalizer errorLocalizer,
    ISessionState session) : ObservableObject, IAsyncInitializable
{
    public ObservableCollection<LocationNodeViewModel> FlatLocations { get; } = [];

    public string? HouseholdName => session.SelectedHouseholdName;

    private Dictionary<Guid, LocationNodeViewModel> _byId = [];
    private Guid? _loadedHouseholdId;

    [ObservableProperty]
    private LocationNodeViewModel? selectedLocation;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isMovingLocation;

    private LocationNodeViewModel? _movingLocation;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (session.SelectedHouseholdId is null) return;

        if (_loadedHouseholdId != session.SelectedHouseholdId)
        {
            await LoadLocationsAsync(session.SelectedHouseholdId.Value, ct);
            _loadedHouseholdId = session.SelectedHouseholdId;
            OnPropertyChanged(nameof(HouseholdName));
        }

        if (session.PendingLocationId is Guid pending)
        {
            session.PendingLocationId = null;
            SelectLocation(pending);
        }
    }

    private async Task LoadLocationsAsync(Guid householdId, CancellationToken ct)
    {
        IsBusy = true;
        try
        {
            var locations = await householdsService.GetLocationsAsync(householdId, ct);
            BuildTree(locations);
        }
        catch (ApiException ex)
        {
            dialogs.ShowError("Operace selhala", errorLocalizer.GetString(ex.Type));
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Operace selhala", $"Chyba připojení: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BuildTree(IReadOnlyList<LocationListItem> locations)
    {
        var nodes = locations.Select(x => new LocationNodeViewModel(x)).ToList();
        _byId = nodes.ToDictionary(x => x.Id);

        foreach (var node in nodes)
        {
            if (node.ParentId is Guid parentId && _byId.TryGetValue(parentId, out var parent))
                parent.Children.Add(node);
        }

        foreach (var node in nodes)
            node.SortChildren();

        FlatLocations.Clear();
        var roots = nodes.Where(n => n.ParentId is null).OrderBy(n => n.SortOrder).ThenBy(n => n.Name);
        foreach (var root in roots)
            AppendFlat(root, 0);
    }

    private void AppendFlat(LocationNodeViewModel node, int depth)
    {
        node.Depth = depth;
        FlatLocations.Add(node);
        if (node.IsExpanded)
            foreach (var child in node.Children)
                AppendFlat(child, depth + 1);
    }

    private void RebuildFlat()
    {
        FlatLocations.Clear();
        var roots = _byId.Values.Where(n => n.ParentId is null).OrderBy(n => n.SortOrder).ThenBy(n => n.Name);
        foreach (var root in roots)
            AppendFlat(root, 0);
    }

    [RelayCommand]
    private void ToggleExpand(LocationNodeViewModel node)
    {
        node.IsExpanded = !node.IsExpanded;
        var index = FlatLocations.IndexOf(node);
        if (index < 0) return;

        if (node.IsExpanded)
            InsertSubtree(node, index + 1);
        else
            RemoveSubtree(node);
    }

    private int InsertSubtree(LocationNodeViewModel parent, int insertAt)
    {
        foreach (var child in parent.Children)
        {
            child.Depth = parent.Depth + 1;
            FlatLocations.Insert(insertAt++, child);
            if (child.IsExpanded)
                insertAt = InsertSubtree(child, insertAt);
        }
        return insertAt;
    }

    private void RemoveSubtree(LocationNodeViewModel parent)
    {
        foreach (var child in parent.Children)
        {
            RemoveSubtree(child);
            FlatLocations.Remove(child);
        }
    }

    private void SelectLocation(Guid locationId)
    {
        if (_byId.TryGetValue(locationId, out var node))
            SelectedLocation = node;
    }

    public async Task RefreshAsync()
    {
        if (session.SelectedHouseholdId is null) return;
        await LoadLocationsAsync(session.SelectedHouseholdId.Value, CancellationToken.None);
        _loadedHouseholdId = session.SelectedHouseholdId;
    }

    // Primary tap on a location card — routes based on current mode
    [RelayCommand]
    private async Task HandleTap(LocationNodeViewModel node)
    {
        SelectedLocation = node;
        if (IsMovingLocation)
            await CompleteMoveLocation(node);
        else
            await OpenLocation(node);
    }

    [RelayCommand]
    private async Task OpenLocation(LocationNodeViewModel location)
    {
        await Shell.Current.GoToAsync($"items?locationId={location.Id}&locationName={Uri.EscapeDataString(location.Name ?? "")}");
    }

    [RelayCommand]
    private async Task OpenLocationDetail(LocationNodeViewModel node)
    {
        SelectedLocation = node;
        await Shell.Current.GoToAsync("locationdetail", new Dictionary<string, object> { ["node"] = node });
    }

    [RelayCommand]
    private async Task AddRootLocation()
    {
        if (session.SelectedHouseholdId is null)
        {
            dialogs.ShowError("Chyba", "Není vybrána domácnost. Přihlaste se znovu.");
            return;
        }

        var name = await PromptNameAsync("Nová lokace", "Zadejte název:");
        if (string.IsNullOrWhiteSpace(name)) return;

        try
        {
            var sortOrder = FlatLocations.Count(n => n.ParentId is null);
            await locationsService.CreateLocationAsync(
                new LocationCreateRequest(name, LocationType.Other, null, sortOrder, null, session.SelectedHouseholdId.Value),
                CancellationToken.None);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Operace selhala", ex is ApiException apiEx ? errorLocalizer.GetString(apiEx.Type) : ex.Message);
        }
    }

    [RelayCommand]
    private async Task AddChildLocationNode(LocationNodeViewModel parent)
    {
        if (session.SelectedHouseholdId is null) return;

        var name = await PromptNameAsync("Nová podzložka", "Zadejte název:");
        if (string.IsNullOrWhiteSpace(name)) return;

        try
        {
            await locationsService.CreateLocationAsync(
                new LocationCreateRequest(name, LocationType.Other, parent.Id, parent.Children.Count, null, session.SelectedHouseholdId.Value),
                CancellationToken.None);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Operace selhala", ex is ApiException apiEx ? errorLocalizer.GetString(apiEx.Type) : ex.Message);
        }
    }

    [RelayCommand]
    private async Task DeleteLocationNode(LocationNodeViewModel node)
    {
        if (!await dialogs.ShowConfirmationDialog("Smazat lokaci",
            $"Opravdu smazat '{node.Name}' včetně podlokací a položek?"))
            return;

        try
        {
            await locationsService.DeleteAsync(node.Id, CancellationToken.None);
            if (SelectedLocation == node) SelectedLocation = null;
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Operace selhala", ex is ApiException apiEx ? errorLocalizer.GetString(apiEx.Type) : ex.Message);
        }
    }

    [RelayCommand]
    private void StartMoveLocationNode(LocationNodeViewModel node)
    {
        SelectedLocation = node;
        _movingLocation = node;
        IsMovingLocation = true;
    }

    [RelayCommand]
    private void CancelMoveLocation()
    {
        _movingLocation = null;
        IsMovingLocation = false;
    }

    [RelayCommand]
    private async Task CompleteMoveLocation(LocationNodeViewModel? target)
    {
        if (_movingLocation is null) return;

        if (target is not null && _movingLocation.ContainsDescendant(target.Id))
        {
            dialogs.ShowError("Operace selhala", "Nelze přesunout lokaci do její vlastní podlokace.");
            _movingLocation = null;
            IsMovingLocation = false;
            return;
        }

        try
        {
            await locationsService.MoveAsync(_movingLocation.Id, target?.Id, CancellationToken.None);
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Operace selhala", ex is ApiException apiEx ? errorLocalizer.GetString(apiEx.Type) : ex.Message);
        }
        finally
        {
            _movingLocation = null;
            IsMovingLocation = false;
            await RefreshAsync();
        }
    }

    [RelayCommand]
    private async Task SwitchHousehold()
    {
        _loadedHouseholdId = null;
        await nav.NavigateTo<HouseholdsViewModel>();
    }

    [RelayCommand]
    private async Task Logout()
    {
        await auth.LogoutAsync();
        session.SelectedHouseholdId = null;
        _loadedHouseholdId = null;
        await nav.NavigateTo<LoginViewModel>();
    }

    private static async Task<string?> PromptNameAsync(string title, string message, string? defaultValue = null)
    {
        if (Shell.Current?.CurrentPage is Page page)
            return await page.DisplayPromptAsync(title, message, initialValue: defaultValue ?? "");
        return null;
    }
}
