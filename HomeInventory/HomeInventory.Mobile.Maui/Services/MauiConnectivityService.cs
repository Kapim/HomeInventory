using CommunityToolkit.Mvvm.ComponentModel;

namespace HomeInventory.Mobile.Maui.Services;

public partial class MauiConnectivityService : ObservableObject, IConnectivityService
{
    public MauiConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    public bool IsOnline => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        => OnPropertyChanged(nameof(IsOnline));
}
