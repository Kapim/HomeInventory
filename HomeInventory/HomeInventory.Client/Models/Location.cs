

namespace HomeInventory.Client.Models
{
    public class Location(Guid id, string name, LocationType locationType, Guid? parentLocationId, Guid householdId, Guid ownerUserId, int sortOrder, string? description)
    {
        public Guid Id = id;
        public string Name { get; } = name;
        public LocationType LocationType { get; } = locationType;
        public Guid? ParentLocationId { get; } = parentLocationId;
        public int SortOrder { get; } = sortOrder;
        public string? Description { get; } = description;
        public Guid HouseholdId { get; } = householdId;
        public Guid OwnerUserId { get; } = ownerUserId;
    }

}
