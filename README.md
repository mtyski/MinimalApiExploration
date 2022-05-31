# MinimalApiExploration

.NET 6 minimal API exploration and experiments.

## Used technologies

- ASP.NET Core 6 minimal API.
- [Entity Framework Core](https://github.com/dotnet/efcore).
- [MediatR](https://github.com/jbogard/MediatR).
- [FluentResults](https://github.com/altmann/FluentResults).
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)

## Testing the solution

### Manual tests

To perform manual tests, `docker-compose up` has to be ran
from `src` directory. Swagger is exposed on port 5000.

### Development tests

#### CLI

To run integration test suite from the console,
following steps have to be taken:

1. `docker-compose up -d` should be ran from `src` directory.
2. `dotnet test` command should be ran next,
targetting any of the test projects (or the solution itself).

Aforementioned steps can be combined into following one-liner:

```bash
docker-compose up -d && dotnet test ./MinimalApiExploration.sln
```

#### Integration with Visual Studio

Given that VS automatically spins up containers
if docker compose support is provided, running `docker-compose`
commands is no longer necessary. Entire test suite can be ran
either from CLI, or using built-in VS test runner.
