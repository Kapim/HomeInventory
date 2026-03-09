using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Models
{
    public class Item(Guid id, string name, int quantity, Guid locationId, Guid ownerId, string? placementNote, string? description)
    {
        public Guid Id = id;
        public string Name { get; private set; } = name;
        public int Quantity { get; set; } = quantity;
        public Guid LocationId { get; } = locationId;
        public Guid OwnerId { get; } = ownerId;
        public string? PlacementNote { get; set; } = placementNote;
        public string? Description { get; set; } = description;

        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name must be set");
            Name = newName.Trim();
        }
    }
}
