using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Auth;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Desktop.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class ItemsSearchViewModel(INavigationService nav, IAuthService auth, IDialogService dialogs) : ObservableObject
    {
        private INavigationService Nav { get; } = nav;
        private IAuthService Auth { get; } = auth;
        private IDialogService Dialogs { get; } = dialogs;

        [RelayCommand]
        public async Task Logout()
        {
            await Auth.LogoutAsync();
            await Nav.NavigateTo<LoginViewModel>();
        }

    }
}
