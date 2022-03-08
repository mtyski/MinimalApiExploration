using Minimal.Api.IntegrationTests.Infrastructure;
using Minimal.Application;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Get_All;

public class TodoItemsAreRequested : BaseIntegrationTest
{
    private static readonly List<string> ItemNames = new()
    {
        "Learn to write integration tests",
        "Test API integration",
        "Go for a good lunch"
    };

    public TodoItemsAreRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
        Task.WaitAll(
            ItemNames.Select(async name => await Client.PostAsJsonAsync(Uri("items"), new NewTodoItemDto(name)))
                .ToArray<Task>());
    }

    [Fact]
    public async Task ItemsAreReturnedAsync()
    {
        var getResponse = await Client.GetAsync(Uri("items"));

        getResponse.EnsureSuccessStatusCode();

        var content = await getResponse.Content.ReadAsAsync<List<TodoItemDto>>();

        content.Should()
            .BeEquivalentTo(ItemNames.Select(static name => new TodoItemDto(name, TodoItemDto.ItemStatus.Created)));
    }
}