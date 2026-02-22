using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class Household
    {

        public Household(string name)
        {
            Rename(name);
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
        public string NormalizedName { get; private set; }
    
    }
}
