using HomeInventory.Mobile.Maui.ViewModels;

namespace HomeInventory.Mobile.Maui.Views;

public partial class ItemsPage : BasePage, IQueryAttributable
{
    private Guid _locationId;
    private string _locationName = "";

    public ItemsPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<ItemsViewModel>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("locationId", out var id) && id is not null)
            _locationId = Guid.Parse(id.ToString()!);
        if (query.TryGetValue("locationName", out var name) && name is not null)
            _locationName = Uri.UnescapeDataString(name.ToString()!);
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        // Don't call base — we load with location ID instead of IAsyncInitializable
        if (_locationId != Guid.Empty && BindingContext is ItemsViewModel vm)
            await vm.LoadByIdAsync(_locationId, _locationName);
    }
}
