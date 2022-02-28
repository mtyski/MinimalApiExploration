using System.Net.Http;
using Minimal.Api.IntegrationTests.Infrastructure;

namespace Minimal.Api.IntegrationTests.Specifications
{
    public abstract class BaseIntegrationTest : IClassFixture<TodoWebApplicationFactory>, IDisposable
    {
        public BaseIntegrationTest(TodoWebApplicationFactory webApplicationFactory)
        {
            todoWebApplicationFactory = webApplicationFactory;
            Client = webApplicationFactory.CreateClient();
        }

        private readonly TodoWebApplicationFactory todoWebApplicationFactory;

        private protected HttpClient Client { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => todoWebApplicationFactory.Dispose();
    }
}
