using DemoApi.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 16 * 1024 * 1024; // 16 MB
    options.MaxSendMessageSize = 16 * 1024 * 1024; // 16 MB
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Add gRPC reflection for development tools
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Map gRPC services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<SensorServiceImpl>();

// Enable gRPC reflection in development
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Info page for browser access
app.MapGet("/", () => @"
╔══════════════════════════════════════════════════════════════════╗
║              gRPC Sensor Service - Port 5002                     ║
╠══════════════════════════════════════════════════════════════════╣
║  This is a gRPC service. Use a gRPC client to connect.          ║
║                                                                   ║
║  Available Services:                                              ║
║  - SensorService: Telemetry streaming                            ║
║  - GreeterService: Sample greeting service                       ║
║                                                                   ║
║  Methods:                                                         ║
║  - GetCurrentTelemetry (Unary)                                   ║
║  - StreamTelemetry (Server Streaming)                            ║
║  - SendTelemetryBatch (Client Streaming)                         ║
║  - TelemetryExchange (Bidirectional Streaming)                   ║
║  - GetLocationHistory (Server Streaming)                         ║
╚══════════════════════════════════════════════════════════════════╝
");

Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════╗
║              gRPC Sensor Service Started                         ║
╠══════════════════════════════════════════════════════════════════╣
║  Listening on: https://localhost:5002                            ║
║  Services: SensorService, GreeterService                         ║
╚══════════════════════════════════════════════════════════════════╝
");

app.Run();
