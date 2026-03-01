using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public class LocationNodeViewModel(Guid id, string name, Guid? parentId, int sortOrder) : ObservableObject
    {
        public Guid Id { get; } = id;
        public string Name { get; } = name;
        public Guid? ParentId { get; } = parentId;
        public int SortOrder { get; } = sortOrder;


        public ObservableCollection<LocationNodeViewModel> Children { get; } = [];
    }
}
