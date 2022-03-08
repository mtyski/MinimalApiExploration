using Minimal.Api.Extensions;
using Minimal.Application.Errors;

namespace Minimal.Api.Endpoints;

internal abstract class Endpoint
{
    protected static readonly Dictionary<Http, Func<WebApplication, string, Delegate, RouteHandlerBuilder>> VerbToMapFactoryMethod = new()
    {
        [Http.Get] = static (app, route, handler) => app.MapGet(route, handler),
        [Http.Post] = static (app, route, handler) => app.MapPost(route, handler),
        [Http.Put] = static (app, route, handler) => app.MapPut(route, handler),
        [Http.Patch] = static (app, route, handler) => app.MapMethods(route, new[] { "PATCH" }, handler),
        [Http.Delete] = static (app, route, handler) => app.MapDelete(route, handler)
    };

    protected readonly List<(Predicate<ResultBase> Filter, Func<ResultBase, IResult> MappingFunction)> ReasonToResultFactoryMap =
        new()
        {
            (static result => result.Reasons.Contains<NotFoundError>(),
                static result => Results.NotFound(result.Errors.OfType<NotFoundError>().Stringy())),
            (static result => result.Reasons.Contains<BadRequestError>(),
                static result => Results.BadRequest(result.Errors.OfType<BadRequestError>().Stringy()))
        };

    /// <summary>
    /// Gets the route associated with the endpoint.
    /// </summary>
    protected string? EndpointRoute { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="Http"/> verb associated with the endpoint.
    /// </summary>
    protected Http? HttpVerb { get; private set; }

    protected Delegate? RequestHandler { get; private set; }

    /// <summary>
    /// Sets the route associated with the endpoint.
    /// </summary>
    /// <param name="route">Route as string to be matched to a handler.</param>
    protected void Route(string route) => EndpointRoute = route;

    /// <summary>
    /// Sets <see cref="Http"/> verb associated with the endpoint.
    /// </summary>
    /// <param name="verb"><see cref="Http"/> enumeration member, representing supported http verbs.</param>
    protected void Verb(Http verb) => HttpVerb = verb;

    protected void Handler(Delegate handler) => RequestHandler = handler;

    /// <summary>
    /// Builds the endpoint instance.
    /// </summary>
    /// <param name="app"><see cref="WebApplication"/> to add built endpoint to.</param>
    internal abstract void Build(WebApplication app);

    /// <summary>
    /// Adds new error mapping to predefined errors.
    /// </summary>
    /// <param name="filter">Predicate that applies for a mapping function.</param>
    /// <param name="mappingFunction">Mapping function between internal <see cref="ResultBase"/> implementor and </param>
    protected void AddResultMap(Predicate<ResultBase> filter, Func<ResultBase, IResult> mappingFunction) =>
        ReasonToResultFactoryMap.Insert(0, (filter, mappingFunction));
}

internal abstract class Endpoint<TRequest, TResult> : Endpoint
    where TRequest : IRequest<TResult>
    where TResult : ResultBase
{
    protected Endpoint()
    {
        AddResultMap(IsNonGenericSuccessfulResult, static _ => Results.NoContent());

        static bool IsNonGenericSuccessfulResult(ResultBase r) =>
            r is Result { IsSuccess: true };
    }

    protected async Task<IResult> Handle(TRequest request, IMediator mediator, CancellationToken token)
    {
        var result = await mediator.Send(request, token);

        return MapResult(result);

        IResult MapResult(ResultBase resultBase) =>
            ReasonToResultFactoryMap.First(map => map.Filter(resultBase))
                .MappingFunction(resultBase);
    }

    /// <inheritdoc />
    internal override void Build(WebApplication app)
    {
        if (string.IsNullOrWhiteSpace(EndpointRoute))
        {
            throw new InvalidOperationException(
                $"{nameof(EndpointRoute)} cannot be null! " +
                $"Call {nameof(Route)} method in the endpoint constructor!");
        }

        if (!HttpVerb.HasValue ||
            !Enum.GetValues<Http>().Contains(HttpVerb.Value) ||
            !VerbToMapFactoryMethod.TryGetValue(HttpVerb.Value, out var endpointRegistrationFunc))
        {
            throw new InvalidOperationException(
                $"{nameof(HttpVerb)} has to be within {nameof(Http)} enumeration! " +
                $"Call {nameof(Verb)} method in the endpoint constructor!");
        }

        if (RequestHandler is null)
        {
            throw new InvalidOperationException(
                $"{nameof(RequestHandler)} cannot be null! " +
                $"Call {nameof(Handler)} method in the endpoint constructor!");
        }

        endpointRegistrationFunc.Invoke(
            app,
            EndpointRoute,
            RequestHandler);
    }
}

internal abstract class Endpoint<TRequest, TResult, TResponse> : Endpoint<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : Result<TResponse>
    where TResponse : class
{
    protected Endpoint()
    {
        AddResultMap(
            IsSuccessfulGenericResultWithValue,
            static result => Results.Ok(GetResultValue(result)));

        static bool IsSuccessfulGenericResultWithValue(ResultBase r) =>
            r is Result<TResponse> { IsSuccess: true };

        static TResponse? GetResultValue(ResultBase r) => 
            r is Result<TResponse> genericResult ?
                genericResult.Value :
                null;
    }
}
