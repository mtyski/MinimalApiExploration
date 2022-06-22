using System.Text.Json;
using System.Text.Json.Serialization;
using Minimal.Api.IntegrationTests.Infrastructure;

namespace Minimal.Api.IntegrationTests.Specifications;

public abstract class BaseIntegrationTest : IClassFixture<TodoWebApplicationFactory>, IDisposable
{
    private readonly TodoWebApplicationFactory todoWebApplicationFactory;

    protected BaseIntegrationTest(TodoWebApplicationFactory webApplicationFactory)
    {
        todoWebApplicationFactory = webApplicationFactory;
        Client = webApplicationFactory.CreateClient();
    }

    private protected static JsonSerializerOptions SerializerOptions => BuildJsonSerializerOptions();

    private protected HttpClient Client { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected Uri Uri(string route) => new(Client.BaseAddress!, route);

    protected virtual void Dispose(bool disposing) => todoWebApplicationFactory.Reset();

    private static JsonSerializerOptions BuildJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.PropertyNameCaseInsensitive = true;
        return options;
    }
}