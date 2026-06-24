# orders-api

Microservicio REST de plantilla base para microservicios .NET 10 (Minimal APIs), listo para contenerizar y desplegar en Kubernetes.

## Estructura

```
├── src/OrdersApi/          # Proyecto principal (.NET 10 Minimal APIs)
├── tests/OrdersApi.Tests/  # Pruebas unitarias (xUnit)
├── k8s/                    # Manifiestos de Kubernetes
├── Dockerfile              # Multi-stage build
└── .github/workflows/ci.yml
```

## Endpoints

| Método | Ruta          | Descripción                          |
|--------|---------------|--------------------------------------|
| GET    | /health       | Liveness probe                       |
| GET    | /ready        | Readiness probe                      |
| GET    | /orders       | Listar todas las órdenes             |
| GET    | /orders/{id}  | Obtener orden por ID                 |
| POST   | /orders       | Crear orden                          |
| PUT    | /orders/{id}  | Actualizar orden                     |
| DELETE | /orders/{id}  | Eliminar orden                       |
| GET    | /openapi/v1.json | Especificación OpenAPI (solo Development) |

## Correr en local

```bash
cd src/OrdersApi
dotnet run
# API disponible en http://localhost:5000
```

## Correr con Docker

```bash
# Construir imagen (desde la raíz del repo)
docker build -t orders-api:latest .

# Ejecutar contenedor
docker run -p 8080:8080 orders-api:latest
# API disponible en http://localhost:8080
```

## Pruebas

```bash
dotnet test health-check.slnx
```

## Desplegar en Kubernetes

```bash
# Aplicar manifiestos (en orden)
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml

# Verificar estado
kubectl get pods -l app=orders-api
kubectl get svc orders-api
```

### Variables de entorno y secretos

La configuración sensible (cadenas de conexión, claves de API) debe almacenarse en un `Secret` de Kubernetes y referenciarse en el deployment:

```yaml
envFrom:
  - secretRef:
      name: orders-api-secret
```

Crear el Secret (ejemplo):

```bash
kubectl create secret generic orders-api-secret \
  --from-literal=ConnectionStrings__Default="Server=...;Database=orders;"
```
