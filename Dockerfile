# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Restore only the app project for better layer caching
COPY src/OrdersApi/OrdersApi.csproj src/OrdersApi/
RUN dotnet restore src/OrdersApi/OrdersApi.csproj

COPY src/ src/

RUN dotnet publish src/OrdersApi/OrdersApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Run as non-root user
RUN groupadd --system --gid 1001 appgroup && \
    useradd --system --uid 1001 --gid appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OrdersApi.dll"]
