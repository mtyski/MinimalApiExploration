using Minimal.Application.Errors;
using Minimal.Db;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class GetById
{
    public record Request(long Id) : IRequest<Result<TodoItemDto>>
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator(TodoContext context)
            {
                RuleFor(static x => x.Id)
                    .MustAsync(async (id, cancellationToken) =>
                        await context.Items.FindAsync(new object[] { id }, cancellationToken) is not null)
                    .WithErrorCode(NotFoundError.ErrorCode)
                    .WithMessage(static request => $"Todo item with Id: {request.Id} was not found!");
            }
        }

        public class Handler : IRequestHandler<Request, Result<TodoItemDto>>
        {
            public Handler(TodoContext context)
            {
                Context = context;
            }

            private TodoContext Context { get; }

            public async Task<Result<TodoItemDto>> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var item = await Context.Items.FindAsync(new object[] { request.Id }, cancellationToken);
                return Result.Ok(TodoItemDto.FromTodoItem(item!));
            }
        }
    }
}
