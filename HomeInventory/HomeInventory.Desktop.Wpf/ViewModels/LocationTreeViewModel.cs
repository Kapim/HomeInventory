using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class LocationTreeViewModel(IHouseholdsService householdsService, ILocationsService locationsService) : ObservableObject
    {
        private readonly IHouseholdsService _households = householdsService;
        private readonly ILocationsService _locations = locationsService;
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
            RootLocations.Clear();
            foreach (var root in Locations.Where(n => n.ParentId is null).OrderBy(n => n.SortOrder))
                RootLocations.Add(root);

            if (Locations.Any())
                SelectedLocation = Locations[0];
            selectedHouseholdId = householdId;  
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
                var created = await _locations.CreateLocationAsync(new LocationCreateRequest(location.Name, LocationType.Other, location.ParentId, 0, null, selectedHouseholdId), new CancellationTokenSource().Token);
                location.FillData(created);
                location.IsNew = false;
 
                location.IsEditing = false;
                location.ShouldFocusName = false;
                EditingNode = null;
            } else
            {
                await _locations.RenameAsync(location.Id, location.Name, new CancellationTokenSource().Token);
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
