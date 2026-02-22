using HomeInventory.Application.Exceptions;
using HomeInventory.Application.Interfaces;
using HomeInventory.Domain;

using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeInventory.Infrastructure.Repositories
{
    public class EfUserRepository(HomeInventoryDbContext db) : IUserRepository
    {
        readonly HomeInventoryDbContext _db = db;

        public async Task AddAsync(User user, CancellationToken ct)
        {
            if (await _db.Users.AnyAsync(u => u.UserName == user.UserName, ct))
                throw new DuplicateUserNameException();
            EntityEntry<User> newUser = await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
        }

        public Task<User?> FindByUsernameAsync(string username, CancellationToken ct)
        {
            return _db.Users.FirstOrDefaultAsync(u => username == u.UserName, ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }
    }
}
