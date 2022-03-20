using Microsoft.EntityFrameworkCore;

namespace Minimal.Api.Extensions;

internal static class WebApplicationExtensions
{
    public static WebApplication AddEndpoints(this WebApplication app)
    {
        typeof(Program).Assembly.GetTypes()
            .Where(
                static t =>
                    !t.IsAbstract &&
                    t.IsAssignableTo(typeof(Endpoints.Endpoint)))
            .Select(Activator.CreateInstance)
            .Cast<Endpoints.Endpoint>()
            .ForEach(e => e.Build(app));

        return app;
    }

    public static WebApplication Migrate<TContext>(this WebApplication app)
        where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<TContext>();
        context.Database.Migrate();
        return app;
    }
}
