using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeInventory.Desktop.Wpf.Services.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        private readonly IServiceProvider _provider;

        public NavigationService(IServiceProvider provider)
        {
            _provider = provider;
        }
        public object? CurrentViewModel { get; private set; }

        public event Action? CurrentViewModelChanged;

        public async Task NavigateTo<TViewModel>() where TViewModel : class
        {
            CurrentViewModel = _provider.GetRequiredService<TViewModel>();
            if (CurrentViewModel is IAsyncInitializable init)
                await init.InitializeAsync();
            CurrentViewModelChanged?.Invoke();
        }

    }
}
