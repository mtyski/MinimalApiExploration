using Microsoft.AspNetCore.Mvc;
using Minimal.Application;
using Minimal.Application.Handlers.TodoItem;

namespace Minimal.Api.Endpoints.TodoItem;

internal class GetAllTodoItems : Endpoint<GetAll.Request, Result<IEnumerable<TodoItemDto>>, IEnumerable<TodoItemDto>>
{
    public GetAllTodoItems()
    {
        Verb(Http.Get);
        Route("/items");
        Handler(async (
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
                await Handle(
                    new(),
                    mediator,
                    cancellationToken));
    }
}