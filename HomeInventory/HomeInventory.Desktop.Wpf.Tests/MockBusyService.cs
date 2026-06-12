using HomeInventory.Desktop.Wpf.Services;
using System.ComponentModel;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class MockBusyService : IBusyService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsBusy { get; private set; }

        public IDisposable Enter()
        {
            IsBusy = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
            return new BusyHandle(this);
        }

        public async Task Run(Func<Task> action)
        {
            using var _ = Enter();
            await action();
        }

        public async Task<T> Run<T>(Func<Task<T>> action)
        {
            using var _ = Enter();
            return await action();
        }

        private void Release()
        {
            IsBusy = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
        }

        private sealed class BusyHandle(MockBusyService owner) : IDisposable
        {
            public void Dispose() => owner.Release();
        }
    }
}
