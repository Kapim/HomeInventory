using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Interfaces
{
    public interface IPasswordHasher
    {
        public string Hash(string plaintextPassword, User user);
        public bool Verify(string plaintextPassword, string passwordHash, User user);
    }
}
