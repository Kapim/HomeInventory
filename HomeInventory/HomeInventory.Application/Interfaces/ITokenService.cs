using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Interfaces
{
    public interface ITokenService
    {
        public string CreateAccessToken(User user);
    }
}
