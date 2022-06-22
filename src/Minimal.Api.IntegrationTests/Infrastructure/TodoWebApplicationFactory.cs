using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minimal.Db;
using Minimal.Model;
using Minimal.Model.Base;
using StackExchange.Redis;

namespace Minimal.Api.IntegrationTests.Infrastructure;

public class TodoWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestEnvironmentName = "Test";

    public TodoWebApplicationFactory()
    {
        Context.Database.Migrate();
    }

    private TodoContext Context => Services.GetRequiredService<TodoContext>();

    private IConnectionMultiplexer Multiplexer => Services.GetRequiredService<IConnectionMultiplexer>();

    internal void Reset()
    {
        ResetRelationalDb();
        ResetRedis();

        void ResetRelationalDb()
        {
            Context.Database.EnsureDeleted();
            Context.ChangeTracker.Clear();
            Context.Database.Migrate();
        }

        void ResetRedis()
        {
            var db = Multiplexer.GetDatabase();
            typeof(TodoItem).Assembly.GetTypes()
                .Where(static t => !t.IsAbstract && t.IsAssignableTo(typeof(BaseEntity)))
                .Select(static t => t.Name)
                .ForEach(typeName => db.KeyDelete(typeName));
        }
    }

    protected override IHost CreateHost(IHostBuilder builder) =>
        base.CreateHost(builder.UseEnvironment(TestEnvironmentName));
}