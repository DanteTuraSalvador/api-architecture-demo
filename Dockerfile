# Multi-stage build for the Fleet Management API Gateway
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY src/backend/Core/DemoApi.Domain/DemoApi.Domain.csproj src/backend/Core/DemoApi.Domain/
COPY src/backend/Core/DemoApi.Application/DemoApi.Application.csproj src/backend/Core/DemoApi.Application/
COPY src/backend/Core/DemoApi.Infrastructure/DemoApi.Infrastructure.csproj src/backend/Core/DemoApi.Infrastructure/
COPY src/backend/DemoApi.Gateway/DemoApi.Gateway.csproj src/backend/DemoApi.Gateway/

# Restore dependencies
RUN dotnet restore src/backend/DemoApi.Gateway/DemoApi.Gateway.csproj

# Copy source code
COPY src/backend/ src/backend/

# Build
WORKDIR /src/src/backend/DemoApi.Gateway
RUN dotnet build DemoApi.Gateway.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish DemoApi.Gateway.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApi.Gateway.dll"]
