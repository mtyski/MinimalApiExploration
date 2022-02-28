using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimal.Api.IntegrationTests.Infrastructure;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Delete;

public class ExistingItemDeleteIsRequested : BaseIntegrationTest
{
    public ExistingItemDeleteIsRequested(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
        var response = Client.PostAsJsonAsync(Uri("items"), new NewTodoItemDto("Learn to delete items from database and test the endpoint")).Result;
        ItemUri = response.Headers.Location
            ?? throw new NullReferenceException($"POST item response header {nameof(response.Headers.Location)} is null!");
    }

    private Uri ItemUri { get; }

    [Fact]
    public async Task ItemIsDeletedAsync()
    {
        var deleteResponse = await Client.DeleteAsync(ItemUri);

        var getResponse = await Client.GetAsync(ItemUri);

        new[]
        {
            deleteResponse,
            getResponse
        }.Select(r => r.StatusCode)
        .Should()
        .ContainInOrder(
            HttpStatusCode.NoContent,
            HttpStatusCode.NotFound);
    }
}
