using HomeInventory.Application.Interfaces;
using System.Security.Authentication;
using HomeInventory.Application.Models;
using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{ 

    public class LoginUseCase(IUserRepository users, ITokenService tokens, IPasswordHasher passwordHasher) : ILoginUseCase
    {
        private readonly IUserRepository _users = users;
        private readonly ITokenService _tokens = tokens;
        private readonly IPasswordHasher _hasher = passwordHasher;

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _users.FindByUsernameAsync(username, new CancellationTokenSource().Token);
            if (user is not null && _hasher.Verify(password, user.PasswordHash, user))
            {
                return new LoginResult(_tokens.CreateAccessToken(user), user.UserRole);
            }
            throw new InvalidCredentialException();
        }
    }
}
