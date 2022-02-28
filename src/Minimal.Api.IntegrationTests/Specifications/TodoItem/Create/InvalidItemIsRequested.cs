using Minimal.Api.IntegrationTests.Infrastructure;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Create;

public sealed class InvalidItemIsRequested : BaseIntegrationTest
{
    public InvalidItemIsRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task NoItemIsCreatedAsync(string? itemName)
    {
        var response = await Client.PostAsJsonAsync(
            Uri("items"),
            new NewTodoItemDto(itemName!));

        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
}
