# Multi-stage build for gRPC Service
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
COPY src/backend/DemoApi.GrpcService/DemoApi.GrpcService.csproj src/backend/DemoApi.GrpcService/

# Restore dependencies
RUN dotnet restore src/backend/DemoApi.GrpcService/DemoApi.GrpcService.csproj

# Copy source code
COPY src/backend/ src/backend/

# Build
WORKDIR /src/src/backend/DemoApi.GrpcService
RUN dotnet build DemoApi.GrpcService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish DemoApi.GrpcService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApi.GrpcService.dll"]
