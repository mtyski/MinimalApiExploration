using Microsoft.EntityFrameworkCore;
using Minimal.Db;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class GetAll
{
    public record Request : IRequest<Result<IEnumerable<TodoItemDto>>>
    {
        internal class Handler : IRequestHandler<Request, Result<IEnumerable<TodoItemDto>>>
        {
            public Handler(TodoContext todoContext)
            {
                Context = todoContext;
            }

            private TodoContext Context { get; }

            public async Task<Result<IEnumerable<TodoItemDto>>> Handle(Request request, CancellationToken cancellationToken)
            {
                var items = await Context.Items.ToListAsync(cancellationToken);
                return items.Select(TodoItemDto.FromTodoItem).ToResult();
            }
        }
    }
}
