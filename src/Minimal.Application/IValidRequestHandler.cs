using Minimal.Application.Extensions;

namespace Minimal.Application;

/// <summary>
///     Contract for request handlers which parse incoming instances of <typeparamref name="TRequest" /> into
///     <typeparamref name="TValidatedRequest" />s.
/// </summary>
/// <remarks>
///     This interface should be implemented for handlers, which full requests' validation would entail potentially costly
///     operations (for example, database reads).
/// </remarks>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TValidatedRequest">Validated request type.</typeparam>
/// <typeparam name="TResponse">Response type of the handler.</typeparam>
internal interface
    IValidatedRequestHandler<in TRequest, TValidatedRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TValidatedRequest : notnull
    where TResponse : ResultBase, new()
{
    /// <inheritdoc cref="IRequestHandler{TRequest,TResponse}.Handle" />
    async Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(
        TRequest request,
        CancellationToken cancellationToken) =>
        await Parse(request, cancellationToken)
            .Bind(
                parseResult =>
                    parseResult.BindAsync(resultValue => HandleValidatedRequest(resultValue, cancellationToken)),
                cancellationToken);

    /// <summary>
    ///     Parses the <paramref name="request" /> to a <typeparamref name="TValidatedRequest" /> instance.
    /// </summary>
    /// <param name="request"><typeparamref name="TRequest" /> instance to parse.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken" /> used in asynchronous operations.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" />, representing the result of an asynchronous operation, wrapping the
    ///     <see cref="Result{T}" /> of parse operation.
    /// </returns>
    Task<Result<TValidatedRequest>> Parse(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles validated <see cref="request" />.
    /// </summary>
    /// <param name="request">Validated request.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken" /> used in asynchronous operations.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/>, representing the result of an asynchronous operation, wrapping the
    ///     <see cref="TResponse"/> of a processed <paramref name="request"/>.
    /// </returns>
    Task<TResponse> HandleValidatedRequest(TValidatedRequest request, CancellationToken cancellationToken = default);
}