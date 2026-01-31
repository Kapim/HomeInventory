using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class Item
    {

        public Item(string name)
        {
            Rename(name);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must be set");
            }
            _name = name.Trim();
        }
        public string Name => _name;
        private string _name = null!;
        public Guid CategoryId;
        public int Quantity;
        public Guid? LocationId { get; private set; }
        public string? PlacementNote;

        public void MoveToLocation(Guid? newLocationId)
        {
            LocationId = newLocationId;
        }
    }
}
