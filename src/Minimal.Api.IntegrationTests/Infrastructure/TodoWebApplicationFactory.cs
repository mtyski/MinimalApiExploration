using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minimal.Db;

namespace Minimal.Api.IntegrationTests.Infrastructure;

public class TodoWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestEnvironmentName = "Test";

    private TodoContext Context => Services.GetRequiredService<TodoContext>();

    public TodoWebApplicationFactory()
    {
        Context.Database.Migrate();
    }

    internal void Reset()
    {
        Context.Database.EnsureDeleted();
        Context.ChangeTracker.Clear();
        Context.Database.Migrate();
    }

    protected override IHost CreateHost(IHostBuilder builder) =>
        base.CreateHost(
            builder.UseEnvironment(TestEnvironmentName));
}
