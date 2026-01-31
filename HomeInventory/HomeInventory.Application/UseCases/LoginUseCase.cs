using HomeInventory.Application.Interfaces;
using System.Security.Authentication;
using HomeInventory.Application.Models;

namespace HomeInventory.Application.UseCases
{ 

    public class LoginUseCase(IUserRepository users, ITokenService tokens) : ILoginUseCase
    {
        private readonly IUserRepository _users = users;
        private readonly ITokenService _tokens = tokens;

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _users.FindByUsernameAsync(username, new CancellationTokenSource().Token);
            if (user?.Password != password)
            {
                throw new InvalidCredentialException();
            }
            return new LoginResult(_tokens.CreateAccessToken(user), user.UserRole);
        }
    }
}
