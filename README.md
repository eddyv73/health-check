# orders-api

Scaffolding base de un microservicio REST con .NET 10 (Minimal APIs), listo para contenedores y despliegue en Kubernetes.

## Ejecutar localmente

```bash
dotnet restore /home/runner/work/health-check/health-check/health-check.slnx
dotnet run --project /home/runner/work/health-check/health-check/src/Orders.Api/Orders.Api.csproj
```

Endpoints principales:

- `GET /health`
- `GET /ready`
- CRUD de orders en `/orders`

OpenAPI solo en `Development`:

- `GET /openapi/v1.json`

## Ejecutar con Docker

```bash
docker build -t orders-api:local /home/runner/work/health-check/health-check
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development orders-api:local
```

## Deploy en Kubernetes

Aplica los manifiestos:

```bash
kubectl apply -f /home/runner/work/health-check/health-check/k8s/configmap.yaml
kubectl apply -f /home/runner/work/health-check/health-check/k8s/service.yaml
kubectl apply -f /home/runner/work/health-check/health-check/k8s/deployment.yaml
```

El deployment espera un Secret llamado `orders-api-secrets` con la llave `connection-string`.
