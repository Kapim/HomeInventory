using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class Item
    {

        public Item(string name, Guid ownerUserId, Guid locationId)
        {
            Rename(name);
            OwnerUserId = ownerUserId;
            LocationId = locationId;
            NormalizedName = Name.ToLowerInvariant();
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must be set");
            }
            _name = name.Trim();
        }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name => _name;
        private string _name = null!;
        public int Quantity;
        public Guid LocationId { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public string? PlacementNote;
        public string? Description;
        public string NormalizedName { get; private set; }

        public void MoveToLocation(Guid newLocationId)
        {
            LocationId = newLocationId;
        }
    }
}
