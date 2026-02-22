using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Repositories;
using Xunit;

namespace HomeInventory.Infrastructure.Tests
{
    [Collection(nameof(PostgresCollection))]
    public class EfHouseholdRepositoryTests(PostgresFixture fx)
    {
        private readonly PostgresFixture _fx = fx;
        private readonly Guid _ownerId = Guid.NewGuid();

        private (HomeInventoryDbContext ctx, EfHouseHoldRepository repo) CreateSut()
        {
            var ctx = DbContextFactory.Create(_fx.ConnectionString);

            // Clean DB between tests (simple approach)
            ctx.Households.RemoveRange(ctx.Households);
            ctx.SaveChanges();

            var repo = new EfHouseHoldRepository(ctx);
            return (ctx, repo);
        }

        private static Household CreateHousehold(string name)
        {
            return new Household(name);
        }

     
        [Fact]
        public async Task Add_Then_GetById_ReturnsEntity()
        {
            var (_, repo) = CreateSut();
            var house = CreateHousehold("House");

            await repo.AddAsync(house);

            var loaded = await repo.GetByIdAsync(house.Id);

            Assert.Equal(house.Id, loaded.Id);
            Assert.Equal("House", loaded.Name);
        }

        [Fact]
        public async Task GetById_UnknownId_ThrowsKeyNotFoundException()
        {
            var (_, repo) = CreateSut();

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(Guid.NewGuid()));
        }

       
        [Fact]
        public async Task Update_PersistsChanges()
        {
            var (_, repo) = CreateSut();

            var house = CreateHousehold("Old");
            await repo.AddAsync(house);

            house.Rename("New");
            await repo.UpdateAsync(house);

            var loaded = await repo.GetByIdAsync(house.Id);
            Assert.Equal("New", loaded.Name);
        }

        [Fact]
        public async Task Remove_Then_GetById_Throws()
        {
            var (_, repo) = CreateSut();

            var house = CreateHousehold("ToRemove");
            await repo.AddAsync(house);

            await repo.RemoveAsync(house);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetByIdAsync(house.Id));
        }

    

       
    }
}
