using HomeInventory.Mobile.Maui.ViewModels;

namespace HomeInventory.Mobile.Maui.Services.Navigation;

public class MauiNavigationService : INavigationService
{
    private static readonly Dictionary<Type, string> Routes = new()
    {
        [typeof(LoginViewModel)] = "login",
        [typeof(HouseholdsViewModel)] = "households",
        [typeof(LocationsViewModel)] = "//main/lokace/locations",
        [typeof(ItemsSearchViewModel)] = "//main/hledat/search",
    };

    public async Task NavigateTo<TViewModel>() where TViewModel : class
    {
        if (Routes.TryGetValue(typeof(TViewModel), out var route))
            await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateBack()
        => await Shell.Current.GoToAsync("..");
}
