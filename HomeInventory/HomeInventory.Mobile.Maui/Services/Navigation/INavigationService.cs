namespace HomeInventory.Mobile.Maui.Services.Navigation;

public interface INavigationService
{
    Task NavigateTo<TViewModel>() where TViewModel : class;
    Task NavigateBack();
}
