using Minimal.Api.IntegrationTests.Infrastructure;
using Minimal.Application;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Update.Implementation;

public sealed class InvalidUpdateIsRequested : BaseTodoItemUpdateSpecification
{
    public static readonly TheoryData<string?, TodoItemDto.ItemStatus> InvalidUpates = new()
    {
        { null, TodoItemDto.ItemStatus.Created },
        { string.Empty, TodoItemDto.ItemStatus.Created },
        { " ", TodoItemDto.ItemStatus.Created },
        { "Provide out-of-enumeration item state", (TodoItemDto.ItemStatus)128 }
    };

    public InvalidUpdateIsRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
    }

    [Theory]
    [MemberData(nameof(InvalidUpates))]
    public async Task NoItemIsUpdatedAsync(
        string? itemName,
        TodoItemDto.ItemStatus status)
    {
        var putResponse = await Client.PutAsJsonAsync(
            ItemUri,
            new TodoItemDto(itemName!, status));

        putResponse.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }
}
