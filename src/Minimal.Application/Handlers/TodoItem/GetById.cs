using Minimal.Application.Errors;
using Minimal.Redis;

namespace Minimal.Application.Handlers.TodoItem;

public abstract class GetById
{
    public record Request(long Id) : IRequest<Result<TodoItemDto>>
    {
        public class Handler : IRequestHandler<Request, Result<TodoItemDto>>
        {
            public Handler(RedisRepository repo)
            {
                Repository = repo;
            }

            private RedisRepository Repository { get; }

            public async Task<Result<TodoItemDto>> Handle(
                Request request,
                CancellationToken cancellationToken) =>
                await Repository.GetModelAsync<Model.TodoItem, TodoItemDto>(request.Id) is
                {
                    IsSuccess: true
                } successfulResult
                    ? successfulResult
                    : Result.Fail<TodoItemDto>(new NotFoundError($"Todo item with Id: {request.Id} was not found!"));
        }
    }
}