using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class User(string userName, string password, UserRole userRole)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string UserName { get; private set; } = userName;
        public string Password { get; private set; } = password;
        public UserRole UserRole { get; private set; } = userRole;

    }
}
