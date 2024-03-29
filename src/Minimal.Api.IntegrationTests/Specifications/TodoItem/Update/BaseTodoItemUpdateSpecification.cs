﻿using Minimal.Api.IntegrationTests.Infrastructure;
using Minimal.Application;

namespace Minimal.Api.IntegrationTests.Specifications.TodoItem.Update;

public class BaseTodoItemUpdateSpecification : BaseIntegrationTest
{
    public BaseTodoItemUpdateSpecification(TodoWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
        var response = Client.PostAsJsonAsync(
            Uri("items"),
            new NewTodoItemDto("Learn to test update logic")
            ).Result;

        ItemToUpdate = response.Content
            .ReadFromJsonAsync<TodoItemDto>(SerializerOptions)
            .Result!;

        ItemUri = response.Headers.Location ??
            throw new NullReferenceException($"POST item response header {nameof(response.Headers.Location)} is null!");
    }

    protected TodoItemDto ItemToUpdate { get; }

    protected Uri ItemUri { get; }
}
