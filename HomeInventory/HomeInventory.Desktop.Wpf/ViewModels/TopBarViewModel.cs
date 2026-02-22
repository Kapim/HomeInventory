using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class TopBarViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Household> households = [];
        [ObservableProperty]
        public Household? selectedHousehold;
    }
}
