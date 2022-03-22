using Minimal.Application.Errors;
using Minimal.Application.Extensions;
using Minimal.Db;
using Minimal.Model;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class Put
{
    public record Request(long ItemId, TodoItemDto TodoItemDto) : IRequest<Result>
    {
        public string GetNewTodoItemName() =>
            string.IsNullOrWhiteSpace(TodoItemDto.Name) ? "empty name" : TodoItemDto.Name;

        internal class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(static x => x.TodoItemDto.Name)
                    .NotEmpty()
                    .WithMessage(static x => $"Item with id {x.ItemId} cannot be renamed to {x.GetNewTodoItemName()}!");

                RuleFor(static x => x.TodoItemDto.Status)
                    .IsInEnum()
                    .WithMessage(static x => $"Item with id {x.ItemId} cannot have state set to to {x.TodoItemDto.Status}!");
            }
        }

        internal class Handler : IValidatedRequestHandler<Request, ValidRequest, Result>
        {
            public Handler(TodoContext context)
            {
                Context = context;
            }

            private TodoContext Context { get; }

            public async Task<Result<ValidRequest>> Parse(Request request, CancellationToken cancellationToken = default)
            {
                var (itemId, (newName, newState)) = request;
                var itemNullable = await Context.Items.FindAsync(new object[] { itemId }, cancellationToken);
                var status = (TodoItem.State)newState;

                return Result.FailIf(
                        itemNullable is null,
                        new NotFoundError($"Todo item with id: {itemId} was not found!"))
                    .ToResult(itemNullable!)
                    .Bind(
                        item => Result.Merge(
                                Result.OkIf(
                                    item.CanBeRenamedTo(newName),
                                    new BadRequestError(
                                        $"Item with id {itemId} cannot be renamed to {request.GetNewTodoItemName()}!")),
                                Result.OkIf(
                                    item.CanHaveSetStateTo(status),
                                    new BadRequestError(
                                        $"Item with id {itemId} cannot have state set to to {status}!")))
                            .ToResult(
                                new ValidRequest(
                                    item,
                                    status,
                                    newName)));
            }

            public async Task<Result> HandleValidatedRequest(ValidRequest request, CancellationToken cancellationToken = default)
            {
                var (todoItem, status, itemName) = request;
                todoItem.Rename(itemName);
                todoItem.SetState(status);
                Context.Update(todoItem);
                await Context.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
        }
    }

    internal record ValidRequest(TodoItem Item, TodoItem.State Status, string ItemName);
}