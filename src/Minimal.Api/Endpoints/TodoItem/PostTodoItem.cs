using Microsoft.AspNetCore.Mvc;
using Minimal.Api.Extensions;
using Minimal.Application;
using Minimal.Application.Handlers.TodoItems;

namespace Minimal.Api.Endpoints.TodoItem;

internal class PostTodoItem : Endpoint<Post.Request, Result<TodoItemDto>, TodoItemDto>
{
    public PostTodoItem()
    {
        AddResultMap(
            IsGenericResultWithLocation,
            static result => Results.Created(
                GetLocationFrom(result),
                GetValueFrom(result)));

        Verb(Http.Post);
        Route("/items");
        Handler(async (
            [FromBody] NewTodoItemDto inputDto,
            [FromServices] IMediator mediator,
            CancellationToken token) =>
            await Handle(
                new(inputDto.Name),
                mediator,
                token));

        static bool IsGenericResultWithLocation(ResultBase resultBase) =>
            resultBase is Result<TodoItemDto> { Successes: var successes } &&
            successes.OfType<Post.CreatedAtSuccess>().Any();

        static string GetLocationFrom(ResultBase resultBase) =>
            resultBase.Reasons.Contains<Post.CreatedAtSuccess>() ?
            resultBase.Reasons.OfType<Post.CreatedAtSuccess>().First().Message :
            throw new ArgumentException(
                $"Received {nameof(resultBase)} does not contain an instance of {nameof(Post.CreatedAtSuccess)}!");

        static TodoItemDto GetValueFrom(ResultBase resultBase) =>
            resultBase is Result<TodoItemDto> { IsSuccess: true, Value: var value } ?
            value :
            throw new ArgumentException(
                $"Received {nameof(resultBase)} is not a generic {nameof(Result<TodoItemDto>)} indicating success!");
    }
}
