using Minimal.Application.Errors;
using Minimal.Db;
using Minimal.Model;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class Put
{
    public record Request(long ItemId, TodoItemDto TodoItemDto) : IRequest<Result>
    {
        public string GetNewTodoItemName() =>
            string.IsNullOrWhiteSpace(TodoItemDto.Name) ? "empty name" : TodoItemDto.Name;

        public class Validator : AbstractValidator<Request>
        {
            public Validator(TodoContext context)
            {
                CascadeMode = CascadeMode.Stop;
                RuleFor(static r => r.ItemId)
                    .MustAsync(
                        async (id, token) =>
                            await FindItemByIdAsync(id, context, token) is not null)
                    .WithErrorCode(NotFoundError.ErrorCode)
                    .WithMessage(static r => $"Todo item with id: {r.ItemId} was not found!");

                RuleFor(static r => r)
                    .MustAsync(
                        async (request, cancellationToken) =>
                        {
                            var (itemId, (name, _)) = request;
                            var item = await FindItemByIdAsync(itemId, context, cancellationToken);
                            return item!.CanBeRenamedTo(name);
                        })
                    .WithMessage(static r => $"Item with id {r.ItemId} cannot be renamed to {r.GetNewTodoItemName()}!");

                RuleFor(static r => r)
                    .MustAsync(
                        async (request, cancellationToken) =>
                        {
                            var (itemId, (_, status)) = request;
                            var item = await FindItemByIdAsync(itemId, context, cancellationToken);
                            return item!.CanHaveSetStateTo((TodoItem.State)status);
                        })
                    .WithMessage(static r => $"Item with id {r.ItemId} cannot have state set to to {r.TodoItemDto.Status}!");
            }

            private static async Task<TodoItem?> FindItemByIdAsync(long id, TodoContext context, CancellationToken token) =>
                await context.Items.FindAsync(new object[] { id }, token);
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            public Handler(TodoContext context)
            {
                Context = context;
            }

            private TodoContext Context { get; }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var item = await Context.Items.FindAsync(
                    new object[] { request.ItemId },
                    cancellationToken);

                item!.Rename(request.GetNewTodoItemName());
                item.SetState((TodoItem.State)request.TodoItemDto.Status);

                Context.Update(item);
                await Context.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
        }
    }
}
