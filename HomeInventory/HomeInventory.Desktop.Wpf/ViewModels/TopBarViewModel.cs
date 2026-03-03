using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class TopBarViewModel : ObservableObject
    {
        public event EventHandler<Household?>? SelectedHouseholdChangedEvent;
        public event EventHandler? AddItemEvent, AddLocationEvent;
        [ObservableProperty]
        public ObservableCollection<Household> households = [];
        [ObservableProperty]
        public Household? selectedHousehold;

        partial void OnSelectedHouseholdChanged(Household? value)
        {
            SelectedHouseholdChangedEvent?.Invoke(this, value);
        }

        [RelayCommand]
        private void AddItem()
        {
            AddItemEvent?.Invoke(this, new());
        }

        [RelayCommand]
        private void AddLocation()
        {
            AddLocationEvent?.Invoke(this, new());
        }
    }
}
