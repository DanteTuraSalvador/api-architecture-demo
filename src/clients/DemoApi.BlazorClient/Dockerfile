# Multi-stage build for Blazor WebAssembly Client
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY DemoApi.BlazorClient.csproj .

# Restore dependencies
RUN dotnet restore DemoApi.BlazorClient.csproj

# Copy source code
COPY . .

# Build and publish
RUN dotnet publish DemoApi.BlazorClient.csproj -c Release -o /app/publish

# Serve with nginx
FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
