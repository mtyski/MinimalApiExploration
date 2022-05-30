# MinimalApiExploration

.NET 6 minimal API exploration and experiments

## Used technologies

- ASP.NET Core 6 minimal API.
- [Entity Framework Core](https://github.com/dotnet/efcore).
- [MediatR](https://github.com/jbogard/MediatR).
- [FluentResults](https://github.com/altmann/FluentResults).
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)

## Testing the solution

All tests (with the exception of domain model unit tests)
require running `docker-compose up` from `src` **beforehand**.

### Manual tests

To manually test the solution, `Minimal.Api` project
has to be launched using either `dotnet` CLI or VS.

### Integration testing

To run integration tests suite, either `dotnet test` command
should be ran, targetting `Minimal.Api.IntegrationTests` project,
or VS test runner should be used to execute tests.
