using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class LocationNodeViewModel : ObservableObject
    {
        public LocationListItem? Location { get; private set; }
        public Guid Id { get; private set; }
        [ObservableProperty]
        private string? name;
        public Guid? ParentId { get; private set; }
        public int? SortOrder { get; private set; }
        [ObservableProperty]
        private bool isEditing;

        public bool IsNew => Location == null;
        [ObservableProperty]
        private bool shouldFocusName;
        [ObservableProperty]
        private bool isExpanded;

        public LocationNodeViewModel(LocationListItem? location)
        {
            Location = location;
            if (location != null)
            {
                Id = location.Id;
                name = location.Name;
                ParentId = location.ParentLocationId;
                SortOrder = location.SortOrder;
            } else
            {
                Id = Guid.Empty;
            }
        }

        public ObservableCollection<LocationNodeViewModel> Children { get; } = [];

        public static LocationNodeViewModel CreateDraft(Guid? parentId, int sortOrder)
            => new(null) { ParentId = parentId, SortOrder = sortOrder, IsEditing = true, ShouldFocusName = true };

        public void SetLocation(LocationListItem location)
        {
            Location = location;
            Id = location.Id;
            Name = location.Name;
            ParentId = location.ParentLocationId;
            SortOrder = location.SortOrder;
        }

        public bool ContainsDescendant(Guid locationId)
            => Children.Any(child => child.Id == locationId || child.ContainsDescendant(locationId));

        public void SortChildren()
        {
            var ordered = Children.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            Children.Clear();
            foreach (var child in ordered)
            {
                child.SortChildren();
                Children.Add(child);
            }
        }
    }
}
