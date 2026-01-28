using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Services;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Desktop.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class ItemsSearchViewModel : ObservableObject
    {
        private INavigationService _nav { get; }
        private IAuthService _auth { get; }
        private IDialogService _dialogs { get; }
        
        public ItemsSearchViewModel(INavigationService nav, IAuthService auth, IDialogService dialogs)
        {
            _nav = nav;
            _auth = auth;
            _dialogs = dialogs;
        }

        [RelayCommand]
        public async Task Logout()
        {
            if (await _auth.LogoutAsync())
                _nav.NavigateTo<LoginViewModel>();
            else
                _dialogs.ShowInfo("Logout failed", "Could not logout");

        }

    }
}
