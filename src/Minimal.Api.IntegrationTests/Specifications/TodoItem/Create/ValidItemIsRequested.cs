using Minimal.Api.IntegrationTests.Infrastructure;
using Minimal.Application;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Create;

public class ValidItemIsRequested : BaseIntegrationTest
{
    public ValidItemIsRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task NewItemIsCreatedAsync()
    {
        const string todoItemName = "Learn integration testing with minimal API!";

        var postResponse = await Client.PostAsJsonAsync(
            Uri("items"),
            new NewTodoItemDto(todoItemName));

        var getResponse = await Client.GetAsync(postResponse.Headers.Location);

        new[]
        {
            postResponse,
            getResponse
        }.ForEach(static response => response.EnsureSuccessStatusCode());

        var responseContent = await getResponse.Content.ReadAsAsync<TodoItemDto>();

        responseContent.Should()
            .BeEquivalentTo(
                new TodoItemDto(
                    todoItemName,
                    TodoItemDto.ItemStatus.Created));
    }
}