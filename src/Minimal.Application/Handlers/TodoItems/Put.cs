using Minimal.Application.Errors;
using Minimal.Application.Extensions;
using Minimal.Db;
using Minimal.Model;

namespace Minimal.Application.Handlers.TodoItems;

public abstract class Put
{
    public record Request(long ItemId, TodoItemDto TodoItemDto) : IRequest<Result<Unit>>
    {
        public string GetNewTodoItemName() =>
            string.IsNullOrWhiteSpace(TodoItemDto.Name) ? "empty name" : TodoItemDto.Name;
    }

    public record ValidRequest(TodoItem Item, TodoItem.State Status, Request Request);

    public class Handler : IValidRequestHandler<Request, ValidRequest, Unit>
    {
        public Handler(TodoContext context)
        {
            Context = context;
        }

        private TodoContext Context { get; }

        public async Task<Result<ValidRequest>> Parse(Request request, CancellationToken cancellationToken)
        {
            var itemNullable = await Context.Items.FindAsync(new object[] {request.ItemId}, cancellationToken);
            var status = (TodoItem.State)request.TodoItemDto.Status;
            return itemNullable.ToResultFromNull(
                    new NotFoundError($"Todo item with id: {request.ItemId} was not found!"))
                .Bind(item => Result.Merge(
                        Result.OkIf(item.CanBeRenamedTo(request.TodoItemDto.Name),
                            $"Item with id {request.ItemId} cannot be renamed to {request.GetNewTodoItemName()}!"),
                        Result.OkIf(item.CanHaveSetStateTo(status),
                            $"Item with id {request.ItemId} cannot have state set to to {request.TodoItemDto.Status}!"))
                    .ToResult(new ValidRequest(item, status, request)));
        }

        public async Task<Unit> HandleResponse(ValidRequest request, CancellationToken cancellationToken)
        {
            request.Item.Rename(request.Request.GetNewTodoItemName());
            request.Item.SetState(request.Status);
            Context.Update(request.Item);
            await Context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}