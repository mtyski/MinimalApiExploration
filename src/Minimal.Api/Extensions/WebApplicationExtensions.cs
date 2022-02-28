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
}
