using HomeInventory.Mobile.Maui.ViewModels;

namespace HomeInventory.Mobile.Maui.Views;

public partial class LocationDetailPage : BasePage, IQueryAttributable
{
    private LocationNodeViewModel? _node;

    public LocationDetailPage()
    {
        InitializeComponent();
        BindingContext = IPlatformApplication.Current!.Services.GetRequiredService<LocationDetailViewModel>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("node", out var obj) && obj is LocationNodeViewModel node)
            _node = node;
    }

    protected override async void OnAppearing()
    {
        // Don't call base — we load with the node reference instead of IAsyncInitializable
        if (_node is not null && BindingContext is LocationDetailViewModel vm)
            await vm.LoadAsync(_node);
    }
}
