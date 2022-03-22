namespace Minimal.Application.Extensions;

public static class ResultExtensions
{
    public static TResult Bind<TRequest, TResult>(
        this Result<TRequest> result,
        Func<TRequest, TResult> f)
        where TResult : ResultBase, new() =>
        result.IsSuccess ? f(result.Value) : new TResult().WithReasons(result.Reasons);

    public static Task<TResult> BindAsync<TRequest, TResult>(
        this Result<TRequest> result,
        Func<TRequest, Task<TResult>> f)
        where TResult : ResultBase, new() =>
        result.IsSuccess ? f(result.Value) : Task.FromResult(new TResult().WithReasons(result.Reasons));

    public static Result<TResponse> Map<TRequest, TResponse>(
        this Result<TRequest> result,
        Func<TRequest, TResponse> f) =>
        result.IsSuccess
            ? f(result.Value)
                .ToResult()
            : new Result<TResponse>().WithReasons(result.Reasons);

    public static TResult WithReasons<TResult>(this TResult result, IEnumerable<IReason> reasons)
        where TResult : ResultBase
    {
        result.Reasons.AddRange(reasons);
        return result;
    }
}