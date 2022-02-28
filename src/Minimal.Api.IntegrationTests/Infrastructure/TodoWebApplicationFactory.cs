using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minimal.Db;

namespace Minimal.Api.IntegrationTests.Infrastructure
{
    public class TodoWebApplicationFactory : WebApplicationFactory<Program>
    {
        private const string TestEnvironmentName = "Test";

        private readonly IConfiguration configuration;

        private DbConnection connection;

        public TodoWebApplicationFactory()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{TestEnvironmentName}.json")
                .Build();
            connection = CreateAndOpenDatabaseConnection();
        }

        private TodoContext Context => Services.GetRequiredService<TodoContext>();

        internal void Reset()
        {
            connection.Dispose();
            connection = CreateAndOpenDatabaseConnection();
            Context.ChangeTracker.Clear();
            Context.Database.EnsureCreated();
        }

        protected override IHost CreateHost(IHostBuilder builder) =>
            base.CreateHost(
                builder.UseEnvironment(TestEnvironmentName)
                    .ConfigureServices(
                        services =>
                        {
                            using var provider = services.BuildServiceProvider();
                            using var scope = provider.CreateScope();
                            using var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
                            context.Database.EnsureCreated();
                        }));

        protected override void Dispose(bool disposing)
        {
            connection.Dispose();
            base.Dispose(disposing);
        }

        private DbConnection CreateAndOpenDatabaseConnection()
        {
            var connection = new SqliteConnection(
                new SqliteConnectionStringBuilder
                {
                    DataSource = configuration["Database:Source"],
                    Mode = Enum.Parse<SqliteOpenMode>(configuration["Database:Mode"]),
                    Cache = Enum.Parse<SqliteCacheMode>(configuration["Database:Cache"])
                }.ToString());

            connection.Open();

            return connection;
        }
    }
}
