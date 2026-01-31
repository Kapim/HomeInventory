using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace HomeInventory.Infrastructure.InMemory
{
    public class InMemoryDataSeed
    {
        public static List<User> GetUsers()
        {
            return
            [
                new("kapi", "heslo", UserRole.Parent),
                new("darca", "heslo", UserRole.Parent),
                new("sima", "heslo", UserRole.Child)
            ];
        }
    }
}
