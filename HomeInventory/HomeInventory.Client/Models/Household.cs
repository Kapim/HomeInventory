using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Models
{
    public class Household(Guid id, string name)
    {
        public Guid Id = id;
        public string Name { get; } = name;
    }
}
