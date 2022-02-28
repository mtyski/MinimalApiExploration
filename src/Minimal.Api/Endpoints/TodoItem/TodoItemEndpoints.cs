using Microsoft.AspNetCore.Mvc;
using Minimal.Application;
using Minimal.Application.Handlers.TodoItems;

namespace Minimal.Api.Endpoints.TodoItem;

public abstract class TodoItemEndpoints
{
    internal class DeleteTodoItem : Endpoint<Delete.Request>
    {
        public DeleteTodoItem()
        {
            Verb(Http.Delete);
            Route("/items/{id:long}");
            Handler(async ([FromRoute] long id, [FromServices] IMediator mediator, CancellationToken token) =>
                await Handle(new Delete.Request(id), mediator, token));
        }
    }

    internal class UpdateTodoItem : Endpoint<Put.Request>
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
                    await Handle(new Put.Request(id, inputDto), mediator, token));
        }
    }
}
