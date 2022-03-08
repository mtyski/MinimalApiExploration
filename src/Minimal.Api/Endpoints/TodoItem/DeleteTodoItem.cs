using Microsoft.AspNetCore.Mvc;
using Minimal.Application.Handlers.TodoItems;

namespace Minimal.Api.Endpoints.TodoItem;

internal class DeleteTodoItem : Endpoint<Delete.Request, Result>
{
    public DeleteTodoItem()
    {
        Verb(Http.Delete);
        Route("/items/{id:long}");
        Handler(async ([FromRoute] long id, [FromServices] IMediator mediator, CancellationToken token) =>
            await Handle(
                new(id),
                mediator,
                token));
    }
}