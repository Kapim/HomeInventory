using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class MainViewModel(TopBarViewModel topBar, IHouseholdsService householdsService) : ObservableObject, IAsyncInitializable 
    {
        public TopBarViewModel TopBar { get; } = topBar;

        private readonly IHouseholdsService _householdService = householdsService;
        public ObservableCollection<Household> Households = [];
        public Household? SelectedHousehold;


        public async Task InitializeAsync(CancellationToken ct = default)
        {
            var h = await _householdService.GetAllAsync(new CancellationTokenSource().Token);
            Households = new ObservableCollection<Household>(h);
            SelectedHousehold = Households.FirstOrDefault();
            TopBar.Households = Households;
            TopBar.SelectedHousehold = SelectedHousehold;
        } 
    }
}
