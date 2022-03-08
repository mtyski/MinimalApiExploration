using Minimal.Application.Errors;

namespace Minimal.Application.PipelineBehavior;

public abstract class ValidationPipelineBehavior
{
    protected static readonly Dictionary<string, Func<string, IReason>> ErrorCodeToFactoryFuncMap = new()
    {
        [NotFoundError.ErrorCode] = static em => new NotFoundError(em),
    };
}

public class ValidationPipelineBehavior<TRequest, TResponse> : ValidationPipelineBehavior, IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly IReadOnlyList<IValidator<TRequest>> validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators.ToList().AsReadOnly();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var validationContext = new ValidationContext<TRequest>(request);

        var validationTasks = validators.Select(v => v.ValidateAsync(validationContext, cancellationToken))
            .Memoize();

        await Task.WhenAll(validationTasks);

        var validationFailures = validationTasks.Select(static t => t.Result)
            .SelectMany(static vr => vr.Errors)
            .ToLookup(
                static vr => vr.PropertyName,
                static vr => (vr.ErrorCode, vr.ErrorMessage));

        if (!validationFailures.Any())
        {
            return await next();
        }

        var response = new TResponse();
        response.Reasons.AddRange(
            validationFailures.SelectMany(
                static vf => vf,
                static (_, tuple) => MapToError(tuple.ErrorCode, tuple.ErrorMessage)));
        return response;

        static IReason MapToError(string errorCode, string errorMessage) =>
            ErrorCodeToFactoryFuncMap.TryGetValue(errorCode, out var factoryFunc) ?
                factoryFunc(errorMessage) :
                new BadRequestError(errorMessage);
    }
}
