using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using HomeInventory.Mobile.Maui.Services.Navigation;
using System.Collections.ObjectModel;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class ItemsSearchViewModel(
    INavigationService nav,
    IAuthService auth,
    IHouseholdsService households,
    ISessionState session) : ObservableObject, IAsyncInitializable
{
    [ObservableProperty]
    private string query = "";

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<SearchResult> Results { get; } = [];

    private IReadOnlyList<Item> _allItems = [];
    private IReadOnlyList<LocationListItem> _allLocations = [];
    private CancellationTokenSource? _debounceToken;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (session.SelectedHouseholdId is null) return;
        await LoadHouseholdDataAsync(session.SelectedHouseholdId.Value, ct);
    }

    private async Task LoadHouseholdDataAsync(Guid householdId, CancellationToken ct)
    {
        IsBusy = true;
        try
        {
            try { _allItems = await households.GetItemsAsync(householdId, ct); }
            catch { _allItems = []; }

            try { _allLocations = await households.GetLocationsAsync(householdId, ct); }
            catch { _allLocations = []; }

            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnQueryChanged(string value)
    {
        _debounceToken?.Cancel();
        _debounceToken = new CancellationTokenSource();
        var token = _debounceToken.Token;
        _ = Task.Delay(300, token).ContinueWith(
            _ => MainThread.BeginInvokeOnMainThread(ApplyFilter),
            token,
            TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Default);
    }

    [RelayCommand]
    private void Search() => ApplyFilter();

    private void ApplyFilter()
    {
        Results.Clear();
        if (string.IsNullOrWhiteSpace(Query)) return;

        var q = Query.Trim().ToLowerInvariant();
        var locationMap = _allLocations.ToDictionary(l => l.Id);
        var addedItemIds = new HashSet<Guid>();

        foreach (var item in _allItems.Where(i => i.Name.ToLowerInvariant().Contains(q)).Take(50))
        {
            addedItemIds.Add(item.Id);
            Results.Add(new SearchResult(item.Id, item.Name, SearchResultType.Item,
                BuildLocationPath(item.LocationId, locationMap), item.LocationId));
        }

        foreach (var item in _allItems.Where(i => !addedItemIds.Contains(i.Id)))
        {
            var matchedTag = item.Tags.FirstOrDefault(t => t.Name.ToLowerInvariant().Contains(q));
            if (matchedTag is null) continue;
            addedItemIds.Add(item.Id);
            Results.Add(new SearchResult(item.Id, item.Name, SearchResultType.Item,
                BuildLocationPath(item.LocationId, locationMap), item.LocationId)
                { TagMatch = matchedTag.Name });
            if (Results.Count >= 100) break;
        }

        foreach (var loc in _allLocations.Where(l => l.Name.ToLowerInvariant().Contains(q)).Take(50))
            Results.Add(new SearchResult(loc.Id, loc.Name, SearchResultType.Location,
                BuildLocationPath(loc.ParentLocationId, locationMap), loc.Id));
    }

    [RelayCommand]
    private async Task SelectResult(SearchResult result)
    {
        session.PendingLocationId = result.LocationId;
        await nav.NavigateTo<LocationsViewModel>();
    }

    [RelayCommand]
    private async Task GoBack()
        => await nav.NavigateTo<LocationsViewModel>();

    [RelayCommand]
    private async Task Logout()
    {
        await auth.LogoutAsync();
        session.SelectedHouseholdId = null;
        await nav.NavigateTo<LoginViewModel>();
    }

    private static string BuildLocationPath(Guid? locationId, Dictionary<Guid, LocationListItem> locationMap)
    {
        var parts = new List<string>();
        var current = locationId;
        while (current.HasValue && locationMap.TryGetValue(current.Value, out var loc))
        {
            parts.Insert(0, loc.Name);
            current = loc.ParentLocationId;
        }
        return string.Join(" › ", parts);
    }
}
