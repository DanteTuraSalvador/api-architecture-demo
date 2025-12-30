# Multi-stage build for MQTT Broker
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 1883
EXPOSE 8883

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY src/backend/DemoApi.MqttBroker/DemoApi.MqttBroker.csproj src/backend/DemoApi.MqttBroker/

# Restore dependencies
RUN dotnet restore src/backend/DemoApi.MqttBroker/DemoApi.MqttBroker.csproj

# Copy source code
COPY src/backend/DemoApi.MqttBroker/ src/backend/DemoApi.MqttBroker/

# Build
WORKDIR /src/src/backend/DemoApi.MqttBroker
RUN dotnet build DemoApi.MqttBroker.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish DemoApi.MqttBroker.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApi.MqttBroker.dll"]
