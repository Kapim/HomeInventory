using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public partial class LocationNodeViewModel(Guid id, string name, Guid? parentId, int sortOrder) : ObservableObject
    {
        public Guid Id { get; private set; } = id;
        [ObservableProperty]
        private string name = name;
        public Guid? ParentId { get; private set; } = parentId;
        public int SortOrder { get; private set; } = sortOrder;
        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private bool isNew = false;
        [ObservableProperty]
        private bool shouldFocusName;
        [ObservableProperty]
        private bool isExpanded;


        public ObservableCollection<LocationNodeViewModel> Children { get; } = [];

        public static LocationNodeViewModel CreateDraft(Guid? parentId, int sortOrder)
            => new(Guid.Empty, "", parentId, sortOrder) { IsNew = true, IsEditing = true, ShouldFocusName = true };

        public void FillData(Location location)
        {
            Id = location.Id;
            Name = location.Name;
            SortOrder = location.SortOrder;
        }
    }
}
