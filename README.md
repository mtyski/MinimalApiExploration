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

## K8s cluster

Kubernetes resource configuration files are located in `src/k8s`
subdirectory. Resources were tested using [minikube](https://minikube.sigs.k8s.io/docs/),
and following instructions assume that minikube is installed on your environment.

### Create the deployment

To create a deployment, following steps have to be taken
(assuming all steps are done from the root of the repo):

- Build api image locally:

```bash
docker build -f ./src/Minimal.Api/Dockerfile -t todoapi:0.0.1 ./src
```

- Load the image in minikube cluster:

```bash
minikube image load todoapi:0.0.1
```

- Apply Kubernetes resource definitions

```bash
kubectl apply -f ./src/k8s
```

You can monitor the deployment in minikube dashboard.

### Exposing API

To expose the API, port-forwarding is required:

```bash
kubectl port-forward service/todoapi -n todo 30000:
```

After running aforementioned command, Swagger UI can be accessed under
`localhost:30000/swagger/index.html`

### Removing the deployment

Configuration files create a separate namespace used to run the deployment.
Removing it can be done with:

```bash
kubectl delete namespace todo
```
