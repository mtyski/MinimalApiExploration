namespace Minimal.Application;

public interface
    IValidRequestHandler<in TRequest, TValidRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    Task<Result<TValidRequest>> Parse(TRequest request, CancellationToken cancellationToken);

    async Task<TResponse> HandleResponse(TValidRequest request, CancellationToken cancellationToken) =>
        (await HandleResult(request, cancellationToken)).Value;

    async Task<Result<TResponse>> HandleResult(TValidRequest request, CancellationToken cancellationToken) =>
        Result.Ok(await HandleResponse(request, cancellationToken));

    async Task<Result<TResponse>> IRequestHandler<TRequest, Result<TResponse>>.Handle(TRequest request,
        CancellationToken cancellationToken)
    {
        var parsed = await Parse(request, cancellationToken);
        return parsed.IsSuccess
            ? await HandleResult(parsed.Value, cancellationToken)
            : new Result<TResponse>().WithErrors(parsed.Errors);
    }
}