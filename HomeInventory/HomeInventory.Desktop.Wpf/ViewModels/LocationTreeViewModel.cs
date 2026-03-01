using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Desktop.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class LocationTreeViewModel(IHouseholdsService householdsService) : ObservableObject
    {
        private readonly IHouseholdsService _households = householdsService;
        [ObservableProperty]
        public ObservableCollection<LocationNodeViewModel> rootLocations = [];
        private Dictionary<Guid, LocationNodeViewModel> _byId = [];
        public IReadOnlyList<LocationNodeViewModel>? Locations { get; private set; }
        [ObservableProperty]
        private LocationNodeViewModel? selectedLocation;
        public EventHandler<LocationNodeViewModel?>? OnSelectedLocationChangedEvent;

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
        }

        partial void OnSelectedLocationChanged(LocationNodeViewModel? value)
        {
            OnSelectedLocationChangedEvent?.Invoke(this, value);
        }
    }
}
