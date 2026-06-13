using HomeInventory.Mobile.Maui.Services;
using System.Diagnostics;

namespace HomeInventory.Mobile.Maui.Views;

public abstract class BasePage : ContentPage
{
    // OnAppearing is more reliable than OnNavigatedTo for tab pages returning from push routes
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IAsyncInitializable init)
        {
            try { await init.InitializeAsync(); }
            catch (Exception ex) { Debug.WriteLine($"[BasePage] InitializeAsync failed: {ex}"); }
        }
    }
}
