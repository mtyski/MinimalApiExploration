using Minimal.Api.IntegrationTests.Infrastructure;
using Minimal.Application;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Update.Implementation;

public class ValidUpdateIsRequested : BaseTodoItemUpdateSpecification
{
    public ValidUpdateIsRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task ItemIsUpdatedAsync()
    {
        var expectedItemDto = new TodoItemDto(
            "Learn to validate requests",
            TodoItemDto.ItemStatus.Done);

        var putResponse = await Client.PutAsJsonAsync(ItemUri, expectedItemDto);

        var getResponse = await Client.GetAsync(ItemUri);

        new[]
        {
            putResponse,
            getResponse
        }.ForEach(static response => response.EnsureSuccessStatusCode());

        var content = await getResponse.Content.ReadAsAsync<TodoItemDto>();

        content.Should().BeEquivalentTo(expectedItemDto)
            .And.NotBeEquivalentTo(ItemToUpdate);
    }
}
