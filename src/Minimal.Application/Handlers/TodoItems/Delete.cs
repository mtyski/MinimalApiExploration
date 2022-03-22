using Minimal.Application.Errors;
using Minimal.Db;
using Minimal.Model;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class Delete
{
    public record Request(long ItemId) : IRequest<Result>
    {
        internal class Validator : AbstractValidator<Request>
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

        internal class Handler : IValidatedRequestHandler<Request, ValidatedRequest, Result>
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
                var item = await Context.Items.FindAsync(new object?[] { request.ItemId }, cancellationToken);

                return Result.OkIf(
                        item is not null,
                        new NotFoundError($"Todo item with id: {request.ItemId} was not found!"))
                    .ToResult(new ValidatedRequest(item!));
            }

            public async Task<Result> HandleValidatedRequest(
                ValidatedRequest request,
                CancellationToken cancellationToken = default)
            {
                Context.Items.Remove(request.Item);
                await Context.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
        }
    }

    internal record ValidatedRequest(TodoItem Item);
}