using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client.Errors;
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
        private LocationNodeViewModel? selectedLocation;
        public EventHandler<LocationNodeViewModel?>? OnSelectedLocationChangedEvent;
        [ObservableProperty] 
        private LocationNodeViewModel? editingNode;
        private Guid selectedHouseholdId;

        public async Task LoadAsync(Guid householdId, CancellationToken ct)
        {
            RootLocations.Clear();
            try
            {
                var locations = await _households.GetLocationsAsync(householdId, ct);
                Locations = [.. locations.Select(x => new LocationNodeViewModel(x.Id, x.Name, x.ParentLocationId, x.SortOrder))];
                _byId = Locations.ToDictionary(x => x.Id);

                foreach (var location in Locations)
                {
                    if (location.ParentId is Guid parentId && _byId.TryGetValue(parentId, out var parent))
                    {
                        parent.Children.Add(location);
                    }
                }
                foreach (var root in Locations.Where(n => n.ParentId is null).OrderBy(n => n.SortOrder))
                    RootLocations.Add(root);

                SelectedLocation = Locations.Any() ? Locations[0] : null;                   
                selectedHouseholdId = householdId;
            } catch (ApiException ex)
            {
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogService.ShowError("Operace selhala", message);
            }
            
        }

        partial void OnSelectedLocationChanged(LocationNodeViewModel? value)
        {
            OnSelectedLocationChangedEvent?.Invoke(this, value);
        }

        public void AddLocation()
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
                    location.FillData(created);
                    location.IsNew = false;

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
                    await _locations.RenameAsync(location.Id, location.Name, new CancellationTokenSource().Token);
                }
                catch (ApiException ex)
                {
                    var message = _errorLocalizer.GetString(ex.Type);
                    _dialogService.ShowError("Operace selhala", message);
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
    }
}
