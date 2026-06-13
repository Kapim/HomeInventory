using HomeInventory.Mobile.Maui.ViewModels;

namespace HomeInventory.Mobile.Maui.Views;

public partial class HouseholdsPage : BasePage
{
    public HouseholdsPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<HouseholdsViewModel>();
    }
}
