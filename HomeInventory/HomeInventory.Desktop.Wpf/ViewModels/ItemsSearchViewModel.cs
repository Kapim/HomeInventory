using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Desktop.Wpf.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class ItemsSearchViewModel(
        INavigationService nav,
        IAuthService auth,
        IHouseholdsService households,
        IDialogService dialogs,
        IPendingNavigationService pending) : ObservableObject, IAsyncInitializable
    {
        private readonly INavigationService _nav = nav;
        private readonly IAuthService _auth = auth;
        private readonly IHouseholdsService _households = households;
        private readonly IDialogService _dialogs = dialogs;
        private readonly IPendingNavigationService _pending = pending;

        [ObservableProperty]
        private string _query = "";

        [ObservableProperty]
        private Household? _selectedHousehold;

        [ObservableProperty]
        private SearchResult? _selectedResult;

        public ObservableCollection<Household> Households { get; } = [];
        public ObservableCollection<SearchResult> Results { get; } = [];

        private IReadOnlyList<Item> _allItems = [];
        private IReadOnlyList<LocationListItem> _allLocations = [];
        private CancellationTokenSource? _debounceToken;

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            var all = await _households.GetAllAsync(ct);
            foreach (var h in all)
                Households.Add(h);
            SelectedHousehold = Households.FirstOrDefault();
        }

        partial void OnSelectedHouseholdChanged(Household? value)
        {
            if (value is not null)
                _ = LoadHouseholdDataAsync(value.Id, CancellationToken.None);
        }

        async partial void OnSelectedResultChanged(SearchResult? value)
        {
            if (value is null) return;
            _pending.PendingLocationId = value.LocationId;
            await _nav.NavigateTo<MainViewModel>();
        }

        internal async Task LoadHouseholdDataAsync(Guid householdId, CancellationToken ct)
        {
            try { _allItems = await _households.GetItemsAsync(householdId, ct); }
            catch { _allItems = []; }

            try { _allLocations = await _households.GetLocationsAsync(householdId, ct); }
            catch { _allLocations = []; }

            ApplyFilter();
        }

        partial void OnQueryChanged(string value)
        {
            _debounceToken?.Cancel();
            _debounceToken = new CancellationTokenSource();
            var token = _debounceToken.Token;
            _ = Task.Delay(300, token).ContinueWith(
                _ => Application.Current.Dispatcher.Invoke(ApplyFilter),
                token,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
        }

        [RelayCommand]
        internal void Search() => ApplyFilter();

        internal void ApplyFilter()
        {
            Results.Clear();
            SelectedResult = null;
            if (string.IsNullOrWhiteSpace(Query)) return;

            var q = Query.Trim().ToLowerInvariant();
            var locationMap = _allLocations.ToDictionary(l => l.Id);

            foreach (var item in _allItems.Where(i => i.Name.ToLowerInvariant().Contains(q)).Take(50))
                Results.Add(new SearchResult(item.Id, item.Name, SearchResultType.Item, BuildLocationPath(item.LocationId, locationMap), item.LocationId));

            foreach (var loc in _allLocations.Where(l => l.Name.ToLowerInvariant().Contains(q)).Take(50))
                Results.Add(new SearchResult(loc.Id, loc.Name, SearchResultType.Location, BuildLocationPath(loc.ParentLocationId, locationMap), loc.Id));
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

        [RelayCommand]
        private async Task GoBack()
        {
            await _nav.NavigateTo<MainViewModel>();
        }

        [RelayCommand]
        private async Task Logout()
        {
            await _auth.LogoutAsync();
            await _nav.NavigateTo<LoginViewModel>();
        }
    }
}
