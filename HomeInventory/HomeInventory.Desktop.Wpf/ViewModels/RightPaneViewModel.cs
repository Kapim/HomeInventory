using CommunityToolkit.Mvvm.ComponentModel;


namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class RightPaneViewModel(ItemsListViewModel itemsListViewModel, LocationDetailViewModel locationDetailViewModel) : ObservableObject
    {
        public ItemsListViewModel ItemsList { get; } = itemsListViewModel;
        public LocationDetailViewModel LocationDetail { get; } = locationDetailViewModel;
    }
}
