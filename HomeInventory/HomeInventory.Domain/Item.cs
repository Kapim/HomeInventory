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
            Quantity = 1;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must be set");
            } else if (name.Length > 20)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Name must not be longer than 20 characters");
            }
            _name = name.Trim();
        }

        public void SetQuantity(int quantity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(quantity);
            Quantity = quantity;
        }

        public void SetDescription(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                ArgumentOutOfRangeException.ThrowIfGreaterThan(100, description.Length, nameof(description));
            Description = description;
        }

        public void SetPlacementNote(string? placementNote)
        {
            if (!string.IsNullOrWhiteSpace(placementNote))
                ArgumentOutOfRangeException.ThrowIfGreaterThan(100, placementNote.Length, nameof(placementNote));
            PlacementNote = placementNote;
        }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name => _name;
        private string _name = null!;
        public int Quantity { get; private set; }
        public Guid LocationId { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public string? PlacementNote { get; private set; }
        public string? Description { get; private set; }
        public string NormalizedName { get; private set; }

        public void MoveToLocation(Guid newLocationId)
        {
            LocationId = newLocationId;
        }
    }
}
