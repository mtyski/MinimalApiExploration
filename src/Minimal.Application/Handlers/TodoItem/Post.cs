using Minimal.Db;

namespace Minimal.Application.Handlers.TodoItem;

public abstract class Post
{
    public record Request(string Name) : IRequest<Result<TodoItemDto>>
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(static x => x.Name)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Result<TodoItemDto>>
        {
            public Handler(TodoContext context)
            {
                Context = context;
            }

            private TodoContext Context { get; }

            public async Task<Result<TodoItemDto>> Handle(Request request, CancellationToken cancellationToken)
            {
                var item = Model.TodoItem.Create(request.Name);
                Context.Items.Add(item);
                await Context.SaveChangesAsync(cancellationToken);

                return Result.Ok(TodoItemDto.FromTodoItem(item))
                    .WithSuccess(new CreatedAtSuccess(item.Id));
            }
        }
    }

    public class CreatedAtSuccess : Success
    {
        public CreatedAtSuccess(long itemId)
            : base($"/items/{itemId}")
        {
        }
    }
}
