using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Desktop.Wpf.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class MainViewModel : ObservableObject, IAsyncInitializable
    {
        public LocationTreeViewModel LocationTree { get; }
        public RightPaneViewModel RightPane { get; }

        private readonly IHouseholdsService _householdsService;
        private readonly IErrorLocalizer _errorLocalizer;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _nav;
        private readonly IPendingNavigationService _pending;
        public ObservableCollection<Household> Households = [];
        private bool isSelectingNewLocationForItem = false;

        private readonly IBusyService _busy;
        public bool IsBusy => _busy.IsBusy;

        private readonly INotificationsService _notifications;
        public ISnackbarMessageQueue SnackbarMessageQueue => _notifications.SnackbarMessageQueue;

        public MainViewModel(
            LocationTreeViewModel locationTree,
            RightPaneViewModel rightPaneViewModel,
            IHouseholdsService householdsService,
            IErrorLocalizer errorLocalizer,
            IDialogService dialogService,
            INotificationsService notifications,
            IBusyService busyService,
            INavigationService nav,
            IPendingNavigationService pending)
        {
            LocationTree = locationTree;
            RightPane = rightPaneViewModel;
            _householdsService = householdsService;
            _errorLocalizer = errorLocalizer;
            _dialogService = dialogService;
            _notifications = notifications;
            _busy = busyService;
            _nav = nav;
            _pending = pending;
            _busy.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IBusyService.IsBusy))
                    OnPropertyChanged(nameof(IsBusy));
            };
        

            LocationTree.SelectedHouseholdChangedEvent += SetActiveHouseholdAsync;
            LocationTree.OnSelectedLocationChangedEvent += LocationTree_OnSelectedLocationChangeEvent;
            RightPane.ItemsList.SelectNewLocationForItemsEvent += RightPaneViewModel_SelectNewLocationForItemsEvent;

        }

        private void RightPaneViewModel_SelectNewLocationForItemsEvent(object? sender, EventArgs e)
        {
            isSelectingNewLocationForItem = true;
        }

     
        private async void LocationTree_OnSelectedLocationChangeEvent(object? sender, LocationNodeViewModel? location)
        {
            if (location != null)
            {
                var householdId = LocationTree.SelectedHousehold?.Id ?? Guid.Empty;
                if (isSelectingNewLocationForItem)
                {
                    await RightPane.ItemsList.MoveSelectedItemsToLocation(location);
                    await RightPane.LocationDetail.LoadAsync(location, new CancellationTokenSource().Token);
                    isSelectingNewLocationForItem = false;
                }
                else
                {
                    await RightPane.LocationDetail.LoadAsync(location, new CancellationTokenSource().Token);
                    await RightPane.ItemsList.LoadAsync(location, householdId, new CancellationTokenSource().Token);
                }
            }
            else
            {
                RightPane.LocationDetail.Clear();
                RightPane.ItemsList.Clear();
            }
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                var h = await _householdsService.GetAllAsync(new CancellationTokenSource().Token);
                Households = new ObservableCollection<Household>(h);
                LocationTree.Households = Households;
                LocationTree.SelectedHousehold = Households.FirstOrDefault();
            } catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogService.ShowError("Operace selhala", message);
            }
            
        }

        private async void SetActiveHouseholdAsync(object? sender, Household? h)
        {
            if (h != null)
            {
                await LocationTree.LoadAsync(h.Id, new CancellationTokenSource().Token);
                if (_pending.PendingLocationId is Guid targetId)
                {
                    _pending.PendingLocationId = null;
                    LocationTree.SelectById(targetId);
                }
            }
        }

        [RelayCommand]
        private async Task GoToSearch()
        {
            await _nav.NavigateTo<ItemsSearchViewModel>();
        }
    }
}
