using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Services.Navigation
{
    public interface INavigationService
    {
        object? CurrentViewModel { get; }
        event Action? CurrentViewModelChanged;

        void NavigateTo<TViewModel>() where TViewModel : class;
    }
}
