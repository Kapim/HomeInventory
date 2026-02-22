using HomeInventory.Application.Exceptions;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Repositories;
using System.Data;
using Xunit;

namespace HomeInventory.Infrastructure.Tests
{
    [Collection(nameof(PostgresCollection))]
    public class EfUserRepositoryTests(PostgresFixture fx)
    {
        private readonly PostgresFixture _fx = fx;

        private (HomeInventoryDbContext ctx, EfUserRepository repo) CreateSut()
        {
            var ctx = DbContextFactory.Create(_fx.ConnectionString);

            // Clean DB between tests (simple approach)
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();

            var repo = new EfUserRepository(ctx);
            return (ctx, repo);
        }

        private User CreateUser(string userName, string passwordHash, UserRole userRole)
            => new(userName, passwordHash, userRole);

       
        [Fact]
        public async Task AddAsync_persists_user_and_FindByNameAsync_returns_it()
        {
            var (ctx, repo) = CreateSut();

            var user = CreateUser("Kapi", "hash", UserRole.Parent);

            await repo.AddAsync(user, CancellationToken.None);
            await ctx.SaveChangesAsync();

            ctx.ChangeTracker.Clear();

            var loaded = await repo.FindByUsernameAsync("Kapi", CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal(user.Id, loaded!.Id);
            Assert.Equal("Kapi", loaded.UserName);
        }

        [Fact]
        public async Task AddAsync_add_duplicate_user_Throw_exepction()
        {
            var (ctx, repo) = CreateSut();

            var user = CreateUser("Kapi", "hash", UserRole.Parent);
            var user2 = CreateUser("Kapi", "hashasdf", UserRole.Child);
            await repo.AddAsync(user, CancellationToken.None);
            await Assert.ThrowsAsync<DuplicateUserNameException>(() => repo.AddAsync(user2, CancellationToken.None));
        }

        [Fact]
        public async Task GetByIdAsync_returns_null_when_not_found()
        {
            var (_, repo) = CreateSut();

            var loaded = await repo.FindByUsernameAsync("blablabla", CancellationToken.None);

            Assert.Null(loaded);
        }

        [Fact]
        public async Task UpdateAsync_updates_user()
        {
            var (_, repo) = CreateSut();

            var user = CreateUser("Kapi", "hash", UserRole.Parent);
            await repo.AddAsync(user, CancellationToken.None);
            user.Rename("UpdatedKapi");
            user.ChangePassword("UpdatedHash");
            user.ChangeRole(UserRole.Child);

            await repo.UpdateAsync(user, CancellationToken.None);

            var updatedUser = await repo.FindByUsernameAsync("UpdatedKapi", CancellationToken.None);

            Assert.NotNull(updatedUser);
            Assert.Equal(user, updatedUser);
            Assert.Equal("UpdatedKapi", updatedUser.UserName);
            Assert.Equal("UpdatedHash", updatedUser.PasswordHash);
            Assert.Equal(UserRole.Child, updatedUser.UserRole);
        }

       
    }
}