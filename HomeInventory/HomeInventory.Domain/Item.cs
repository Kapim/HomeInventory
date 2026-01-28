using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class Item(string name)
    {
        public string Name { get; private set; } = name;
    }
}
