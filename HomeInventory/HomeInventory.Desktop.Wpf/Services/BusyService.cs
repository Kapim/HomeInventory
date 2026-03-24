using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HomeInventory.Desktop.Wpf.Services
{
    public class BusyService : IBusyService
    {
        int _counter = 0;

        public bool IsBusy => _counter > 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public IDisposable Enter()
        {
            var newVal = Interlocked.Increment(ref _counter);
            if (newVal == 1) OnPropertyChanged(nameof(IsBusy));

            return new Scope(this);
        }

        public async Task Run(Func<Task> action)
        {
            using (Enter())
                await action();
        }

        public async Task<T> Run<T>(Func<Task<T>> action)
        {
            using (Enter())
                return await action();
        }
        private void Exit()
        {
            var newVal = Interlocked.Decrement(ref _counter);
            if (newVal == 0) OnPropertyChanged(nameof(IsBusy));
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private sealed class Scope(BusyService owner) : IDisposable
        {
            private BusyService? _owner = owner;

            public void Dispose()
            {
                _owner?.Exit();
                _owner = null;
            }
        }

    }
}
