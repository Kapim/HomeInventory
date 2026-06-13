using HomeInventory.Mobile.Maui.ViewModels;
using System.Diagnostics;

namespace HomeInventory.Mobile.Maui.Views;

public partial class ItemsSearchPage : BasePage
{
    public ItemsSearchPage()
    {
        try
        {
            InitializeComponent();
            BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<ItemsSearchViewModel>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ItemsSearchPage] Constructor failed: {ex}");
            // Show error on main thread after page is added to visual tree
            Dispatcher.Dispatch(async () =>
            {
                await DisplayAlertAsync("Chyba inicializace", ex.Message, "OK");
            });
        }
    }
}
