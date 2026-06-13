using HomeInventory.Mobile.Maui.ViewModels;

namespace HomeInventory.Mobile.Maui.Views;

public partial class LocationsPage : BasePage
{
    public LocationsPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<LocationsViewModel>();
    }
}
