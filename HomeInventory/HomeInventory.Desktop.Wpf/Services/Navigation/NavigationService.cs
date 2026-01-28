using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            CurrentViewModel = _provider.GetRequiredService<TViewModel>();
            CurrentViewModelChanged?.Invoke();
        }
    }
}
