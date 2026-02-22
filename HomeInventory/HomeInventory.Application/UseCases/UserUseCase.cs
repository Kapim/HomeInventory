using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using System.Security.Authentication;

namespace HomeInventory.Application.UseCases
{
    public class UserUseCase(IUserRepository repo) : IUserUserCase
    {
        private readonly IUserRepository _users = repo;
        public async Task AddUserAsync(string userName, string password, UserRole userRole)
        {
            User user = new(userName, password.ToString()!, userRole);
            try {
                await _users.AddAsync(user, new CancellationTokenSource().Token);
            } catch (ArgumentException)
            {
                throw;
            } 
        }

        public async Task ChangePasswordAsync(Guid id, string newPassword)
        {
            var user = await _users.GetByIdAsync(id, new CancellationTokenSource().Token) ?? throw new KeyNotFoundException(nameof(User));
            user.ChangePassword(newPassword.ToString());
            return;
        }

        public Task ChangeRole(Guid id, UserRole newUserRole)
        {
            throw new NotImplementedException();
        }

        public Task<Household> FindUserByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task RenameUserAsync(Guid id, string newUserName)
        {
            throw new NotImplementedException();
        }
    }
}
