using Minimal.Redis;

namespace Minimal.Application.Handlers.TodoItem;

public abstract class GetAll
{
    public record Request : IRequest<Result<IEnumerable<TodoItemDto>>>
    {
        public class Handler : IRequestHandler<Request, Result<IEnumerable<TodoItemDto>>>
        {
            public Handler(RedisRepository repo)
            {
                Repository = repo;
            }

            private RedisRepository Repository { get; }

            public async Task<Result<IEnumerable<TodoItemDto>>> Handle(
                Request request,
                CancellationToken cancellationToken) =>
                Result.Ok(
                    await Repository.GetAllAsync<Model.TodoItem, TodoItemDto>()
                        .ContinueWith(static t => t.Result.AsEnumerable(), cancellationToken));
        }
    }
}