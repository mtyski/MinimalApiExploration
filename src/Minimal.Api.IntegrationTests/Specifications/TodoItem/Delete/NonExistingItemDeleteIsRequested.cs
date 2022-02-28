using Minimal.Api.IntegrationTests.Infrastructure;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Delete;

public class NonExistingItemDeleteIsRequested : BaseIntegrationTest
{
    public NonExistingItemDeleteIsRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task NoItemIsDeletedAsync()
    {
        var deleteResponse = await Client.DeleteAsync(Uri("items/1"));

        deleteResponse.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
}
