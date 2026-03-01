

namespace HomeInventory.Client.Models
{
    public class LocationListItem(Guid id, string name, LocationType locationType, Guid? parentLocationId, int sortOrder)
    {
        public Guid Id = id;
        public string Name { get; } = name;
        public LocationType LocationType { get; } = locationType;
        public Guid? ParentLocationId { get; } = parentLocationId;
        public int SortOrder { get; } = sortOrder;
       
    }

}
