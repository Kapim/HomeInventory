using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using HomeInventory.Mobile.Maui.Services.Navigation;
using System.Collections.ObjectModel;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class HouseholdsViewModel(
    IHouseholdsService householdsService,
    INavigationService nav,
    IAuthService auth,
    IDialogService dialogs,
    IErrorLocalizer errorLocalizer,
    ISessionState session) : ObservableObject, IAsyncInitializable
{
    public ObservableCollection<Household> Households { get; } = [];

    [ObservableProperty]
    private bool isBusy;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        IsBusy = true;
        try
        {
            var all = await householdsService.GetAllAsync(ct);
            Households.Clear();
            foreach (var h in all)
                Households.Add(h);

            if (Households.Count == 1)
            {
                session.SelectedHouseholdId = Households[0].Id;
                session.SelectedHouseholdName = Households[0].Name;
                await nav.NavigateTo<LocationsViewModel>();
            }
        }
        catch (ApiException ex)
        {
            var message = errorLocalizer.GetString(ex.Type);
            dialogs.ShowError("Operace selhala", message);
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

    [RelayCommand]
    private async Task SelectHousehold(Household household)
    {
        try
        {
            session.SelectedHouseholdId = household.Id;
            session.SelectedHouseholdName = household.Name;
            await nav.NavigateTo<LocationsViewModel>();
        }
        catch (Exception ex)
        {
            dialogs.ShowError("Navigační chyba", $"{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace?[..Math.Min(400, ex.StackTrace?.Length ?? 0)]}");
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        await auth.LogoutAsync();
        await nav.NavigateTo<LoginViewModel>();
    }
}
