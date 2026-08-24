using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
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

        private CancellationTokenSource? _debounceToken;

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            var all = await _households.GetAllAsync(ct);
            foreach (var h in all)
                Households.Add(h);
            SelectedHousehold = Households.FirstOrDefault();
        }

        partial void OnSelectedHouseholdChanged(Household? value) => _ = SearchAsync();

        async partial void OnSelectedResultChanged(SearchResult? value)
        {
            if (value is null) return;
            _pending.PendingLocationId = value.LocationId;
            await _nav.NavigateTo<MainViewModel>();
        }

        partial void OnQueryChanged(string value)
        {
            _debounceToken?.Cancel();
            _debounceToken = new CancellationTokenSource();
            _ = DebouncedSearchAsync(_debounceToken.Token);
        }

        private async Task DebouncedSearchAsync(CancellationToken token)
        {
            try { await Task.Delay(300, token); }
            catch (TaskCanceledException) { return; }
            await SearchAsync(token);
        }

        [RelayCommand]
        internal void Search() => _ = SearchAsync();

        // Search now runs on the server (GET /households/{id}/search); the client no longer
        // downloads the whole inventory to filter locally.
        private async Task SearchAsync(CancellationToken token = default)
        {
            var household = SelectedHousehold;
            if (household is null || string.IsNullOrWhiteSpace(Query))
            {
                Application.Current.Dispatcher.Invoke(() => { Results.Clear(); SelectedResult = null; });
                return;
            }

            IReadOnlyList<SearchResultDto> dtos;
            try { dtos = await _households.SearchAsync(household.Id, Query.Trim(), token); }
            catch { return; }

            if (token.IsCancellationRequested) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Results.Clear();
                SelectedResult = null;
                foreach (var d in dtos)
                    Results.Add(Map(d));
            });
        }

        private static SearchResult Map(SearchResultDto d) => new(
            d.Id, d.Name,
            d.Kind == SearchResultKindDto.Location ? SearchResultType.Location : SearchResultType.Item,
            d.LocationPath, d.LocationId)
        {
            TagMatch = d.TagMatch,
            Description = d.Description
        };

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
