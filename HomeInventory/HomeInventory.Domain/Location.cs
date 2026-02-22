using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class Location
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid HouseholdId { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public string Name => _name;  // must be unique within one Parent
        private string _name = null!;

        public string NormalizedName { get; private set; } = null!;
        public LocationType LocationType { get; set; }

        public Guid? ParentLocationId { get; private set; } // no cycles allowed, null means root (rooms typically)
        public int SortOrder { get; private set; } // ordering of the nodes in the GUI
        public string? Description { get; private set; } // user description of the location (e.g. "black box on the top shelf")

        private Location() { }

        public Location(string name, Guid houseHoldId, Guid ownerUserId, Guid? parentLocationId = null, LocationType locationType = LocationType.Other)
        {
            Rename(name);
            HouseholdId = houseHoldId;
            OwnerUserId = ownerUserId;
            ParentLocationId = parentLocationId;
            LocationType = locationType;
        }

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Name must be set");
            }
            _name = newName.Trim();
            NormalizedName = _name.ToLowerInvariant();
        }

        public void SetDescription(string? newDescription)
        {
            if (newDescription is null)
                Description = null;
            else
            {
                if (string.IsNullOrWhiteSpace(newDescription))
                {
                    throw new ArgumentException("Description cannot be empty");
                }
                Description = newDescription.Trim();

            }
        }

        public void MoveTo(Guid? newParentId)
        {
            if (Id == newParentId)
                throw new ArgumentException("Cannot move location to the itself");
            ParentLocationId = newParentId;
        }
    }
}
