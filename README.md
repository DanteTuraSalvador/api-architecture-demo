# API Architecture Demo

[![CI](https://github.com/DanteTuraSalvador/api-architecture-demo/actions/workflows/ci.yml/badge.svg)](https://github.com/DanteTuraSalvador/api-architecture-demo/actions/workflows/ci.yml)

A comprehensive demonstration of **7 different API architectures** in a single .NET 9 solution, with **4 frontend clients** showcasing real-world integration patterns.

## Purpose

**This is NOT an IoT project.** This is a demonstration of different API communication patterns:

- REST API
- GraphQL
- gRPC
- SignalR/WebSocket
- Server-Sent Events (SSE)
- MQTT
- WebRTC

Fleet Management is simply the domain context to make the demonstrations realistic and relatable.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              FRONTEND CLIENTS                                │
├─────────────────┬─────────────────┬─────────────────┬───────────────────────┤
│  Blazor WASM    │  React + Apollo │   Vue 3 + SSE   │  Angular + WebRTC     │
│  (SignalR)      │  (GraphQL)      │   (EventSource) │  (Peer-to-Peer)       │
│  Port 5124      │  Port 5173      │   Port 5174     │  Port 4200            │
└────────┬────────┴────────┬────────┴────────┬────────┴───────────┬───────────┘
         │                 │                 │                    │
         │    WebSocket    │     HTTP        │    HTTP/SSE        │  WebSocket
         │                 │                 │                    │  (Signaling)
         ▼                 ▼                 ▼                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         API GATEWAY (Port 5181)                              │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────────────┐   │
│  │ REST API │ │ GraphQL  │ │ SignalR  │ │   SSE    │ │ WebRTC Signaling │   │
│  │ /api/*   │ │ /graphql │ │ /hubs/*  │ │ /sse/*   │ │ /hubs/signaling  │   │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
         │                              │                         │
         │                              │                         │
         ▼                              ▼                         ▼
┌─────────────────┐          ┌─────────────────┐        ┌─────────────────┐
│  gRPC Service   │          │  MQTT Broker    │        │  In-Memory DB   │
│  Port 5002      │          │  Port 1883      │        │  (EF Core)      │
└─────────────────┘          └─────────────────┘        └─────────────────┘
```

## How The Clients Interact

All frontend clients connect to the **same backend API Gateway**, demonstrating how different protocols can coexist:

### Data Flow Examples

1. **Create a Vehicle (React GraphQL)** → Data saved to database → **SignalR broadcasts update** → **Blazor Dashboard auto-updates**

2. **Trigger an Alert (Vue SSE)** → Alert created via REST → **SSE pushes to all Vue clients** → Alert displayed in real-time

3. **Update Vehicle Location (Blazor SignalR)** → Location sent via WebSocket → **All connected Blazor clients see movement**

4. **Video Call (Angular WebRTC)** → Signaling via SignalR Hub → **Peer-to-peer connection established**

## Quick Start

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- Docker & Docker Compose (optional)

### Option 1: Run Everything Locally

```bash
# Terminal 1: Start API Gateway
cd src/backend/DemoApi.Gateway
dotnet run

# Terminal 2: Start Blazor Client (SignalR Demo)
cd src/clients/DemoApi.BlazorClient
dotnet run

# Terminal 3: Start React Client (GraphQL Demo)
cd src/clients/react-graphql-client
npm install && npm run dev

# Terminal 4: Start Vue Client (SSE Demo)
cd src/clients/vue-sse-client
npm install && npm run dev

# Terminal 5: Start Angular Client (WebRTC Demo)
cd src/clients/angular-webrtc-client
npm install && npm start
```

### Option 2: Run with Docker

```bash
docker-compose up --build
```

## Service URLs

| Service | URL | Description |
|---------|-----|-------------|
| **API Gateway** | http://localhost:5181 | Swagger UI, REST, GraphQL, SignalR, SSE |
| **GraphQL Playground** | http://localhost:5181/graphql | Interactive GraphQL explorer |
| **Blazor Client** | http://localhost:5124 | Real-time vehicle tracking (SignalR) |
| **React Client** | http://localhost:5173 | Fleet management dashboard (GraphQL) |
| **Vue Client** | http://localhost:5174 | Live alert streaming (SSE) |
| **Angular Client** | http://localhost:4200 | Video chat (WebRTC) |

## Client Details

### Blazor WebAssembly (SignalR)
**Port 5124** | Real-time bidirectional communication

- **Dashboard**: Live vehicle tracking with auto-updating positions
- **Vehicles**: CRUD operations with real-time sync across all clients
- **Technology**: .NET Blazor WASM + Microsoft.AspNetCore.SignalR.Client

**Key Features:**
- WebSocket connection to `/hubs/tracking`
- Receives `LocationUpdated`, `VehicleStatusChanged` events
- Sends location updates from simulated GPS

### React (GraphQL)
**Port 5173** | Flexible data querying

- **Fleet Dashboard**: Query vehicles, drivers, alerts, and fleets
- **CRUD Operations**: Create, update, delete via GraphQL mutations
- **Technology**: React 18 + Apollo Client + TypeScript

**Key Features:**
- Single endpoint: `/graphql`
- Fetch exactly what you need (no over-fetching)
- Nested queries (vehicle → driver → trips)

**Example Query:**
```graphql
query {
  vehicles {
    id
    licensePlate
    status
    driver {
      firstName
      lastName
    }
    currentTrip {
      status
      startTime
    }
  }
}
```

### Vue 3 (Server-Sent Events)
**Port 5174** | One-way server push

- **Alert Stream**: Real-time alert notifications
- **Alert Trigger**: Create test alerts to see SSE in action
- **Technology**: Vue 3 Composition API + EventSource

**Key Features:**
- Connects to `/sse/alerts` endpoint
- Automatic reconnection on disconnect
- Low overhead (HTTP long-polling alternative)

**How to Test:**
1. Open Vue client at http://localhost:5174
2. Use the "Trigger Alert" form to create an alert
3. Watch the alert appear instantly in the stream

### Angular (WebRTC)
**Port 4200** | Peer-to-peer communication

- **Video Chat**: Direct browser-to-browser video/audio calls
- **Text Chat**: Real-time messaging via data channels
- **Technology**: Angular 19 + @microsoft/signalr

**Key Features:**
- SignalR hub for signaling (`/hubs/signaling`)
- ICE candidate exchange for NAT traversal
- Peer-to-peer after connection established

**How to Test:**
1. Open Angular client in two browser windows
2. Enter room name in both
3. Click "Join Room" → video call established

## API Types Explained

| API Type | Protocol | Direction | Best For |
|----------|----------|-----------|----------|
| **REST** | HTTP | Request/Response | CRUD operations, simple queries |
| **GraphQL** | HTTP | Request/Response | Complex queries, mobile apps |
| **gRPC** | HTTP/2 | Bidirectional Streaming | Microservices, high-performance |
| **SignalR** | WebSocket | Bidirectional | Real-time collaboration, gaming |
| **SSE** | HTTP | Server → Client | Notifications, live feeds |
| **MQTT** | TCP | Pub/Sub | IoT devices, sensors |
| **WebRTC** | UDP | Peer-to-Peer | Video calls, file sharing |

## Project Structure

```
ApiDemo/
├── src/
│   ├── backend/
│   │   ├── Core/
│   │   │   ├── DemoApi.Domain/           # Entities, Enums, Value Objects
│   │   │   ├── DemoApi.Application/      # Services, Interfaces, DTOs
│   │   │   └── DemoApi.Infrastructure/   # EF Core, External Services
│   │   ├── DemoApi.Gateway/              # REST, GraphQL, SignalR, SSE, WebRTC
│   │   ├── DemoApi.GrpcService/          # gRPC streaming service
│   │   └── DemoApi.MqttBroker/           # MQTT message broker
│   │
│   └── clients/
│       ├── DemoApi.BlazorClient/         # Blazor WASM (SignalR)
│       ├── DemoApi.GrpcConsoleClient/    # .NET Console (gRPC)
│       ├── react-graphql-client/         # React (GraphQL)
│       ├── vue-sse-client/               # Vue.js (SSE)
│       └── angular-webrtc-client/        # Angular (WebRTC)
│
├── tests/
│   ├── DemoApi.Domain.Tests/             # Unit tests
│   ├── DemoApi.Application.Tests/        # Service tests
│   ├── DemoApi.Integration.Tests/        # Integration tests
│   └── DemoApi.E2E.Tests/                # Playwright E2E tests
│
├── Dockerfile                            # API Gateway container
├── docker-compose.yml                    # Full stack orchestration
└── README.md
```

## Testing

### Unit & Integration Tests
```bash
# Run all 248 tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### E2E Tests (Playwright)
```bash
cd tests/DemoApi.E2E.Tests
npm install
npx playwright install

# Run tests (API must be running)
npm test

# Run with UI
npm run test:ui
```

## Docker Deployment

### Build and Run
```bash
# Build all images
docker-compose build

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Docker Services
| Service | Internal Port | External Port |
|---------|---------------|---------------|
| api-gateway | 8080 | 5001 |
| grpc-service | 5002 | 5002 |
| mqtt-broker | 1883 | 1883 |
| blazor-client | 80 | 5003 |
| react-client | 80 | 3000 |
| vue-client | 80 | 5173 |
| angular-client | 80 | 4200 |

## Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Code Quality
```bash
# Format code
dotnet format

# Analyze
dotnet build /p:TreatWarningsAsErrors=true
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests: `dotnet test`
5. Submit a pull request

## License

MIT
