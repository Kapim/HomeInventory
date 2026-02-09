using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace HomeInventory.Infrastructure.Tests
{
    public sealed class PostgresFixture : IAsyncLifetime
    {
        private PostgreSqlContainer _container = default!;

        public string ConnectionString => _container.GetConnectionString();

        public async Task InitializeAsync()
        {
            _container = new PostgreSqlBuilder("postgres:16")
                .WithDatabase("homeinventory_test")
                .WithUsername("user")
                .WithPassword("pass")
                .Build();

            await _container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [CollectionDefinition(nameof(PostgresCollection))]
    public sealed class PostgresCollection : ICollectionFixture<PostgresFixture> { }
}
