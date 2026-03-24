using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Services
{
    public interface IBusyService : INotifyPropertyChanged
    {
        bool IsBusy { get; }
        IDisposable Enter();
        Task Run(Func<Task> action);
        Task<T> Run<T>(Func<Task<T>> action);
    }
}
