using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class LocationTreeViewModel(IHouseholdsService householdsService, ILocationsService locationsService, IErrorLocalizer errorLocalizer, IDialogService dialogService) : ObservableObject
    {
        private readonly IHouseholdsService _households = householdsService;
        private readonly ILocationsService _locations = locationsService;
        private readonly IErrorLocalizer _errorLocalizer = errorLocalizer;
        private readonly IDialogService _dialogService = dialogService;
        [ObservableProperty]
        public ObservableCollection<LocationNodeViewModel> rootLocations = [];
        private Dictionary<Guid, LocationNodeViewModel> _byId = [];
        public IReadOnlyList<LocationNodeViewModel>? Locations { get; private set; }
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteLocationCommand))]
        [NotifyCanExecuteChangedFor(nameof(RenameLocationCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveLocationCommand))]
        private LocationNodeViewModel? selectedLocation;
        public EventHandler<LocationNodeViewModel?>? OnSelectedLocationChangedEvent;

        [ObservableProperty] 
        private LocationNodeViewModel? editingNode;
        private Guid selectedHouseholdId;
        private LocationNodeViewModel? movingLocation;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CancelMoveLocationCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveLocationToRootCommand))]
        private bool isMovingLocation;

        public event EventHandler<Household?>? SelectedHouseholdChangedEvent;
        [ObservableProperty]
        public ObservableCollection<Household> households = [];
        [ObservableProperty]
        public Household? selectedHousehold;

        private bool CanManipulateLocation => SelectedLocation != null && !SelectedLocation.IsNew;
        private bool CanFinishMove => IsMovingLocation && movingLocation is not null;

        public async Task LoadAsync(Guid householdId, CancellationToken ct)
        {
            RootLocations.Clear();
            try
            {
                var locations = await _households.GetLocationsAsync(householdId, ct);
                Locations = [.. locations.Select(x => new LocationNodeViewModel(x))];
                _byId = Locations.ToDictionary(x => x.Id);

                foreach (var location in Locations)
                {
                    if (location.ParentId is Guid parentId && _byId.TryGetValue(parentId, out var parent))
                    {
                        parent.Children.Add(location);
                    }
                }
                foreach (var location in Locations)
                    location.SortChildren();

                foreach (var root in Locations.Where(n => n.ParentId is null).OrderBy(n => n.SortOrder).ThenBy(n => n.Name))
                    RootLocations.Add(root);

                SelectedLocation = Locations.Any() ? Locations[0] : null;                   
                selectedHouseholdId = householdId;
            } catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogService.ShowError("Operace selhala", message);
            }
            
        }
        async partial void OnSelectedLocationChanged(LocationNodeViewModel? value)
        {
            if (IsMovingLocation)
            {
                await CompleteMoveAsync(value);
                return;
            }

            OnSelectedLocationChangedEvent?.Invoke(this, value);
        }
       

        [RelayCommand]
        public async Task CommitEdit()
        {
            var location = EditingNode;
            if (location is null) return;

            if (string.IsNullOrWhiteSpace(location.Name))
                return;

            if (location.IsNew)
            {
                try
                {
                    var created = await _locations.CreateLocationAsync(new LocationCreateRequest(location.Name, LocationType.Other, location.ParentId, 0, null, selectedHouseholdId), new CancellationTokenSource().Token);
                    location.SetLocation(LocationMapping.MapToListItem(created));
                    location.IsEditing = false;
                    location.ShouldFocusName = false;
                    EditingNode = null;
                } catch (ApiException ex)
                {
                    var message = _errorLocalizer.GetString(ex.Type);
                    _dialogService.ShowError("Operace selhala", message);
                }
            } else
            {
                try
                {
                    var newLocation = await _locations.RenameAsync(location.Id, location.Name, new CancellationTokenSource().Token);
                    location.SetLocation(LocationMapping.MapToListItem(newLocation));
                    location.IsEditing = false;
                    EditingNode = null;
                }
                catch (ApiException ex)
                {
                    var message = _errorLocalizer.GetString(ex.Type);
                    _dialogService.ShowError("Operace selhala", message);
                    location.Name = location.Location!.Name;
                }
            }
       
        }

        [RelayCommand]
        public void CancelEdit()
        {
            var location = EditingNode;
            if (EditingNode is null) return;

            if (location != null && location.IsNew)
            {
                RemoveLocation(location);
                location.IsEditing = false;
            }
                
            EditingNode = null;
                
        }

        private void RemoveLocation(LocationNodeViewModel location)
        {
            if (location.ParentId is null)
            {
                RootLocations.Remove(location);
                return;
            }
            if (_byId.TryGetValue(location.ParentId.Value, out var parent))
            {
                parent?.Children.Remove(location);
            }
        }

        partial void OnSelectedHouseholdChanged(Household? value)
        {
            SelectedHouseholdChangedEvent?.Invoke(this, value);
        }


        [RelayCommand]
        private void AddLocation()
        {
            var parent = SelectedLocation;

            if (EditingNode is not null) return;

            var sort = parent?.Children.Count ?? RootLocations.Count;
            var draft = LocationNodeViewModel.CreateDraft(parent?.Id, sort);

            if (parent is null)
                RootLocations.Add(draft);
            else
            {
                parent.IsExpanded = true;
                parent.Children.Add(draft);
            }

            EditingNode = draft;
            SelectedLocation = draft;
        }

        [RelayCommand(CanExecute = nameof(CanManipulateLocation))]
        private async Task DeleteLocation()
        {
            var result = _dialogService.ShowConfirmationDialog("Delete location", $"Are you sure you want to delete location {SelectedLocation!.Name}, all its sub-locations and all items?");
            if (result == Enums.DialogResult.Yes)
            {
                try
                {
                    await _locations.DeleteAsync(SelectedLocation!.Id, new CancellationTokenSource().Token);
                } catch (ApiException ex)
                {
                    var message = _errorLocalizer.GetString(ex.Type);
                    _dialogService.ShowError("Operace selhala", message);
                } finally
                {
                    await LoadAsync(selectedHouseholdId, new CancellationTokenSource().Token);
                }
            }
                
        }

        [RelayCommand(CanExecute = nameof(CanManipulateLocation))]
        private async Task RenameLocation()
        {
            SelectedLocation!.IsEditing = true;
            EditingNode = SelectedLocation;
        }

        [RelayCommand(CanExecute = nameof(CanManipulateLocation))]
        private void MoveLocation()
        {
            movingLocation = SelectedLocation;
            IsMovingLocation = true;
        }

        [RelayCommand(CanExecute = nameof(CanFinishMove))]
        private void CancelMoveLocation()
        {
            movingLocation = null;
            IsMovingLocation = false;
        }

        [RelayCommand(CanExecute = nameof(CanFinishMove))]
        private async Task MoveLocationToRoot()
            => await MoveLocationToParentAsync(null);

        private async Task CompleteMoveAsync(LocationNodeViewModel? target)
        {
            if (target is null || movingLocation is null)
            {
                CancelMoveLocation();
                return;
            }

            if (target.Id == movingLocation.Id)
            {
                CancelMoveLocation();
                OnSelectedLocationChangedEvent?.Invoke(this, target);
                return;
            }

            if (movingLocation.ContainsDescendant(target.Id))
            {
                _dialogService.ShowError("Operace selhala", "Lokaci nelze přesunout do její vlastní podlokace.");
                CancelMoveLocation();
                OnSelectedLocationChangedEvent?.Invoke(this, movingLocation);
                return;
            }

            await MoveLocationToParentAsync(target.Id);
        }

        private async Task MoveLocationToParentAsync(Guid? newParentId)
        {
            if (movingLocation is null)
                return;

            var movedLocationId = movingLocation.Id;
            try
            {
                await _locations.MoveAsync(movedLocationId, newParentId, new CancellationTokenSource().Token);
                movingLocation = null;
                IsMovingLocation = false;
                await LoadAsync(selectedHouseholdId, new CancellationTokenSource().Token);
                if (_byId.TryGetValue(movedLocationId, out var moved))
                {
                    ExpandAncestors(moved);
                    SelectedLocation = moved;
                }
            }
            catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogService.ShowError("Operace selhala", message);
                CancelMoveLocation();
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError("Operace selhala", ex.Message);
                CancelMoveLocation();
            }
        }

        private void ExpandAncestors(LocationNodeViewModel location)
        {
            var parentId = location.ParentId;
            while (parentId is Guid id && _byId.TryGetValue(id, out var parent))
            {
                parent.IsExpanded = true;
                parentId = parent.ParentId;
            }
        }
    }
}
