using Microsoft.AspNetCore.Mvc;
using Minimal.Application;
using Minimal.Application.Handlers.TodoItems;

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
                    new GetById.Request(id),
                    mediator,
                    token));
    }
}