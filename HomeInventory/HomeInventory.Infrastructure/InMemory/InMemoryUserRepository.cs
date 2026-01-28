using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using HomeInventory.Application.Interfaces;

namespace HomeInventory.Infrastructure.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {

        public InMemoryUserRepository()
        {
            // temporary seed
            foreach (var user in InMemoryDataSeed.GetUsers())
            {
                Task.WaitAll(AddUser(user));
            }
        }

        private readonly Dictionary<string, User> _users = [];
        public Task<User?> FindByUsernameAsync(string username, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            _users.TryGetValue(username, out var user);

            return Task.FromResult(user);
        }

        public async Task<bool> AddUser(User user)
        {
            if (_users.ContainsKey(user.UserName))
                return false;
            _users.Add(user.UserName, user);
            return true;
        }

    }
}
