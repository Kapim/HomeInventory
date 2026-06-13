using System.ComponentModel;

namespace HomeInventory.Mobile.Maui.Services;

public interface IBusyService : INotifyPropertyChanged
{
    bool IsBusy { get; }
    IDisposable Enter();
    Task Run(Func<Task> action);
    Task<T> Run<T>(Func<Task<T>> action);
}
