using HomeInventory.Application.Exceptions;
using HomeInventory.Application.Interfaces;
using HomeInventory.Application.Models;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.UseCases
{
    public class RegisterUseCase(IPasswordHasher passwordHasher, IUserRepository repo) : IRegisterUseCase
    {
        private readonly IPasswordHasher _hasher = passwordHasher;
        private readonly IUserRepository _repo = repo;
        public async Task RegisterAsync(string username, string password, UserRole userRole)
        {
            User user = new(username, _hasher.Hash(password, new User()), userRole);
            try
            {
                await _repo.AddAsync(user, new CancellationTokenSource().Token);
            } catch (DuplicateUserNameException)
            {
                throw;
            }
        }
        
    }
}
