using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;
using Microsoft.AspNetCore.Identity;

namespace HomeInventory.Infrastructure.Services
{
    public sealed class AspNetPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher = new();
        public string Hash(string plaintextPassword, User user)
        {
            return _hasher.HashPassword(user, plaintextPassword);
        }

        public bool Verify(string plaintextPassword, string passwordHash, User user)
        {
            var result = _hasher.VerifyHashedPassword(user, passwordHash, plaintextPassword);
            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
