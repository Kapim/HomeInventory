using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByUsernameAsync(string username, CancellationToken ct);
        public Task<bool> AddUser(User user);
    }
}
