using Minimal.Api.IntegrationTests.Infrastructure;

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
                TodoItemDto.ItemStatus.InProgress));

        putResponse.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
}
