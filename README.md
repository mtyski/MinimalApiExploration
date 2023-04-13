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

## Helm chart

Helm chart for the api resides in `./src/todo-api` directory.

### Prerequisites

- Working Kubernetes cluster.
  - Chart was tested using Docker Desktop-provisioned Kubernetes cluster,
    but any solution should work.
- [Helm](https://helm.sh/).

### Create the deployment

To create a deployment, following steps have to be taken
(assuming all steps are done from the root of the repo):

- Deploy NGINX ingress controller resources (one-time setup,
unless the cluster is reset):

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.2.0/deploy/static/provider/cloud/deploy.yaml
```

- Build api image locally:

```bash
docker build -f ./src/Minimal.Api/Dockerfile -t todoapi:0.0.1 ./src
```

- Create values file for postgres sub-chart with credentials and db name:

```yaml
postgres:
  db:
    user: <username />
    password: <password />
    name: <database name />
```

- Install the chart:

```
helm install todo-api ./src/todo-api --create-namespace -n todo-api -f <path to the file created in the previous step />
```

You can monitor the deployment using command:

```bash
kubectl get deployment -n todo-api -w
```

### Exposing API

To expose the API, port-forwarding is required:

```bash
kubectl port-forward service/ingress-nginx-controller -n ingress-nginx 80:80
```

After running aforementioned command, `/etc/hosts` has to be edited with following entry provided:
`127.0.0.1 todo.poc.com`.

With those two steps completed, Swagger UI should be available under `todo.poc.com`.

**NOTE:** binding port 80 requires elevated privileges. If this port cannot be bound,
use any port >5000 (run `netstat -an` to check whether the port is available).

### Removing the deployment

Use `helm uninstall` command in conjunction with `kubectl delete namespace`:

```bash
helm uninstall todo-api -n todo-api && kubectl delete namespace todo-api
```
