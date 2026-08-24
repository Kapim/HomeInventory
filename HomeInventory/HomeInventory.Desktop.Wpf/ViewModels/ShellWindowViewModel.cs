using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Auth;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    partial class ShellWindowViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly IAuthService _auth;

        [ObservableProperty]
        private object? currentViewModel;

        [ObservableProperty]
        private string title = "Shell Window";

        public ShellWindowViewModel(INavigationService nav, IAuthService auth)
        {
            _nav = nav;
            _auth = auth;

            _nav.CurrentViewModelChanged += () => CurrentViewModel = _nav.CurrentViewModel;

            _ = StartAsync();
        }

        // "Stay logged in": reuse a persisted, non-expired token and skip the login screen.
        private async Task StartAsync()
        {
            var token = await _auth.GetTokenAsync();
            if (JwtHelper.IsValid(token))
                await _nav.NavigateTo<MainViewModel>();
            else
                await _nav.NavigateTo<LoginViewModel>();
        }
    }
}
