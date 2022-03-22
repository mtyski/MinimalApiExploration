using Microsoft.AspNetCore.Mvc;
using Minimal.Application;
using Minimal.Application.Handlers.TodoItems;

namespace Minimal.Api.Endpoints.TodoItem;

internal class UpdateTodoItem : Endpoint<Put.Request, Result>
{
    public UpdateTodoItem()
    {
        Verb(Http.Put);
        Route("/items/{id:long}");
        Handler(async (
            [FromBody] TodoItemDto inputDto,
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            CancellationToken token) =>
                await Handle(
                    new(id, inputDto),
                    mediator,
                    token));
    }
}