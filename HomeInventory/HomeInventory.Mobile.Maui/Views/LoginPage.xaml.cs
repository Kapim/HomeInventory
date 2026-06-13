using HomeInventory.Mobile.Maui.ViewModels;

namespace HomeInventory.Mobile.Maui.Views;

public partial class LoginPage : BasePage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<LoginViewModel>();
    }
}
