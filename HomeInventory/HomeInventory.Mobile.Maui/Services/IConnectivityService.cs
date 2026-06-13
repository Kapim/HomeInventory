using System.ComponentModel;

namespace HomeInventory.Mobile.Maui.Services;

public interface IConnectivityService : INotifyPropertyChanged
{
    bool IsOnline { get; }
}
