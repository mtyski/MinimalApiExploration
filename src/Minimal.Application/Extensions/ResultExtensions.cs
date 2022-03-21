namespace Minimal.Application.Extensions;

public static class ResultExtensions
{
    public static Result<TResponse> Bind<TRequest, TResponse>(this Result<TRequest> result,
        Func<TRequest, Result<TResponse>> f) =>
        result.IsSuccess ? f(result.Value) : new Result<TResponse>().WithReasons(result.Reasons);

    public static Result<TResponse> Map<TRequest, TResponse>(this Result<TRequest> result,
        Func<TRequest, TResponse> f) =>
        result.IsSuccess ? f(result.Value).ToResult() : new Result<TResponse>().WithReasons(result.Reasons);

    public static Result<TRequest> ToResultFromNull<TRequest>(this TRequest? request, IError error) =>
        request is null ? Result.Fail<TRequest>(error) : Result.Ok(request);
}