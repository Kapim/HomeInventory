using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
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

    private CancellationTokenSource? _debounceToken;

    public Task InitializeAsync(CancellationToken ct = default) => Task.CompletedTask;

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
    private Task Search() => SearchAsync();

    // Search runs on the server (GET /households/{id}/search); the client no longer
    // downloads the whole inventory to filter locally.
    private async Task SearchAsync(CancellationToken token = default)
    {
        if (session.SelectedHouseholdId is null || string.IsNullOrWhiteSpace(Query))
        {
            MainThread.BeginInvokeOnMainThread(Results.Clear);
            return;
        }

        IReadOnlyList<SearchResultDto> dtos;
        IsBusy = true;
        try { dtos = await households.SearchAsync(session.SelectedHouseholdId.Value, Query.Trim(), token); }
        catch { return; }
        finally { IsBusy = false; }

        if (token.IsCancellationRequested) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Results.Clear();
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
}
