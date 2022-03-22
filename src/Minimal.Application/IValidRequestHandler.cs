using Minimal.Application.Extensions;

namespace Minimal.Application;

internal interface
    IValidatedRequestHandler<in TRequest, TValidatedRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    async Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(
        TRequest request,
        CancellationToken cancellationToken) =>
        await Parse(request, cancellationToken)
            .Bind(
                parseResult =>
                    parseResult.BindAsync(resultValue => HandleValidatedRequest(resultValue, cancellationToken)),
                cancellationToken);

    Task<Result<TValidatedRequest>> Parse(TRequest request, CancellationToken cancellationToken = default);

    Task<TResponse> HandleValidatedRequest(TValidatedRequest request, CancellationToken cancellationToken = default);
}