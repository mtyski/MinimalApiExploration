using Minimal.Api.IntegrationTests.Infrastructure;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Create
{
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
            var newItem = new NewTodoItemDto(todoItemName);

            var postResponse = await Client.PostAsJsonAsync("/items", newItem);

            postResponse.EnsureSuccessStatusCode();

            var getResponse = await Client.GetAsync(postResponse.Headers.Location);

            getResponse.EnsureSuccessStatusCode();

            var responseContent = await getResponse.Content.ReadAsAsync<TodoItemDto>();

            responseContent.Should().BeEquivalentTo(new TodoItemDto(todoItemName, TodoItemDto.ItemStatus.Created));
        }
    }
}
