﻿using Microsoft.AspNetCore.Mvc;
using Minimal.Application;
using Minimal.Application.Handlers.TodoItem;

namespace Minimal.Api.Endpoints.TodoItem;

internal class GetTodoItemById : Endpoint<GetById.Request, Result<TodoItemDto>, TodoItemDto>
{
    public GetTodoItemById()
    {
        Verb(Http.Get);
        Route("/items/{id:long}");
        Handler(async (
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            CancellationToken token) =>
                await Handle(
                    new(id),
                    mediator,
                    token));
    }
}