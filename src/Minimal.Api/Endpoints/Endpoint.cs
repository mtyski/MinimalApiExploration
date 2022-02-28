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

    protected readonly ICollection<(Predicate<Result> Filter, Func<Result, IResult> MappingFunction)> ReasonToResultFactoryMap =
        new List<(Predicate<Result> Filter, Func<Result, IResult> MappingFunction)>
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
    /// <param name="tuple"></param>
    protected void AddResultMap((Predicate<Result> Filter, Func<Result, IResult> MappingFunction) tuple) =>
        ReasonToResultFactoryMap.Add(tuple);
}

internal abstract class Endpoint<TRequest> : Endpoint
    where TRequest : IRequest<Result>
{
    protected Endpoint()
    {
        AddResultMap((IsNonGenericSuccessfulResult, static result => Results.NoContent()));

        static bool IsNonGenericSuccessfulResult(Result r) =>
            r is { IsSuccess: true } && !r.GetType().IsGenericType;
    }

    protected async Task<IResult> Handle(TRequest request, IMediator mediator, CancellationToken token)
    {
        var result = await mediator.Send(request, token);

        return MapResult(result);

        IResult MapResult(Result result) =>
            ReasonToResultFactoryMap.First(map => map.Filter(result))
                .MappingFunction(result);
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

internal abstract class Endpoint<TRequest, TResponse> : Endpoint<TRequest>
    where TRequest : IRequest<Result>
    where TResponse : class
{
    public Endpoint()
    {
        AddResultMap((IsSuccessfulGenericResultWithValue, static result => Results.Ok(GetResultValue(result))));

        static bool IsSuccessfulGenericResultWithValue(Result r) =>
            r is { IsSuccess: true } && r.GetType().IsGenericType && GetResultValue(r) is not null;

        static TResponse? GetResultValue(Result r)
        {
            dynamic result = r;
            try
            {
                return result.Value;
            }
            catch // cannot bind value in runtime
            {
                return null;
            }
        }
    }
}
