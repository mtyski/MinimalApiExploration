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

        private readonly DbConnection connection;

        public TodoWebApplicationFactory()
        {
            connection = CreateAndOpenDatabaseConnection();
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

        private static DbConnection CreateAndOpenDatabaseConnection()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{TestEnvironmentName}.json")
                .Build();

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
