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

    private protected HttpClient Client { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected Uri Uri(string route) => new(Client.BaseAddress!, route);

    protected virtual void Dispose(bool disposing) => todoWebApplicationFactory.Reset();
}
