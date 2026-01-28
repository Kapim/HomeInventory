using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    partial class ShellWindowViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        
        [ObservableProperty]
        private object? currentViewModel;

        [ObservableProperty]
        private string title = "Shell Window";

        public ShellWindowViewModel(INavigationService nav)
        {
            _nav = nav;

            _nav.CurrentViewModelChanged += () => CurrentViewModel = _nav.CurrentViewModel;

            _nav.NavigateTo<LoginViewModel>();
        }
    }
}
