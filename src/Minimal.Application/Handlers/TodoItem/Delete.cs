using Minimal.Application.Errors;
using Minimal.Db;
using ItemDeletedEvent = Minimal.Model.TodoItem.DomainEvents.ItemDeleted;

namespace Minimal.Application.Handlers.TodoItem;

public abstract class Delete
{
    public record Request(long ItemId) : IRequest<Result>
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator(TodoContext context)
            {
                RuleFor(static r => r.ItemId)
                    .MustAsync(
                        async (itemId, cancellationToken) =>
                            await context.Items.FindAsync(new object[] { itemId }, cancellationToken) is not null)
                    .WithErrorCode(NotFoundError.ErrorCode)
                    .WithMessage(static r => $"Todo item with id: {r.ItemId} was not found!");
            }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            public Handler(TodoContext context)
            {
                Context = context;
            }

            public TodoContext Context { get; }

            /// <inheritdoc />
            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var item = await Context.Items.FindAsync(
                    new object?[] { request.ItemId },
                    cancellationToken);

                item!.RegisterEvent(new ItemDeletedEvent(item.Id));

                Context.Items.Remove(item);

                await Context.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
        }
    }
}