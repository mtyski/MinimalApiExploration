using Minimal.Api.IntegrationTests.Infrastructure;
using Minimal.Application;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Update.Implementation;

public sealed class NonExistingItemIsUpdated : BaseIntegrationTest
{
    public NonExistingItemIsUpdated(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task NoItemIsUpdatedAsync()
    {
        var putResponse = await Client.PutAsJsonAsync(
            Uri("items/1"),
            new TodoItemDto(
                "Blow stuff up!",
                TodoItemDto.ItemStatus.Done));

        putResponse.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
}
