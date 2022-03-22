using Minimal.Application.Errors;
using Minimal.Db;
using Minimal.Model;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class GetById
{
    public record Request(long Id) : IRequest<Result<TodoItemDto>>
    {
        internal class Handler : IValidatedRequestHandler<Request, ValidatedRequest, Result<TodoItemDto>>
        {
            public Handler(TodoContext context)
            {
                Context = context;
            }

            private TodoContext Context { get; }

            public async Task<Result<ValidatedRequest>> Parse(
                Request request,
                CancellationToken cancellationToken = default)
            {
                var item = await Context.Items.FindAsync(new object?[] { request.Id }, cancellationToken);
                
                return Result.OkIf(
                        item is not null,
                        new NotFoundError($"Todo item with Id: {request.Id} was not found!"))
                    .ToResult(new ValidatedRequest(item!));
            }

            public Task<Result<TodoItemDto>> HandleValidatedRequest(
                ValidatedRequest request,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(TodoItemDto.FromTodoItem(request.Item).ToResult());
        }
    }

    internal record ValidatedRequest(TodoItem Item);
}