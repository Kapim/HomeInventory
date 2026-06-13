using CommunityToolkit.Mvvm.ComponentModel;
using HomeInventory.Client.Models;
using System.Collections.ObjectModel;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class LocationNodeViewModel : ObservableObject
{
    public LocationListItem? Location { get; private set; }
    public Guid Id { get; private set; }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChevronText))]
    private bool isExpanded = false;

    public Guid? ParentId { get; private set; }
    public int? SortOrder { get; private set; }
    public int Depth { get; set; }
    public bool IsNew => Location is null;
    public bool HasChildren => Children.Count > 0;
    public string ChevronText => IsExpanded ? "▾" : "▸";

    public ObservableCollection<LocationNodeViewModel> Children { get; } = [];

    public LocationNodeViewModel(LocationListItem location)
    {
        Location = location;
        Id = location.Id;
        name = location.Name;
        ParentId = location.ParentLocationId;
        SortOrder = location.SortOrder;
    }

    private LocationNodeViewModel()
    {
        Id = Guid.Empty;
    }

    public static LocationNodeViewModel CreateDraft(Guid? parentId, int sortOrder)
        => new() { ParentId = parentId, SortOrder = sortOrder };

    public void SetLocation(LocationListItem location)
    {
        Location = location;
        Id = location.Id;
        Name = location.Name;
        ParentId = location.ParentLocationId;
        SortOrder = location.SortOrder;
    }

    public bool ContainsDescendant(Guid locationId)
        => Children.Any(c => c.Id == locationId || c.ContainsDescendant(locationId));

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

    public string PathLabel => Depth == 0 ? "" : new string(' ', Depth * 2) + "└ ";
}
