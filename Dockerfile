FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Orders.Api/Orders.Api.csproj", "src/Orders.Api/"]
RUN dotnet restore "src/Orders.Api/Orders.Api.csproj"

COPY . .
RUN dotnet publish "src/Orders.Api/Orders.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
