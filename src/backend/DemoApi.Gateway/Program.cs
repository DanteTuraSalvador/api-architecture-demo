using DemoApi.Gateway.GraphQL;
using DemoApi.Gateway.Hubs;
using DemoApi.Gateway.SSE;
using DemoApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Fleet Management API",
        Version = "v1",
        Description = "A comprehensive API demonstrating 7 different API architectures: REST, GraphQL, gRPC, SignalR, SSE, MQTT, and WebRTC"
    });
});

// Add Infrastructure services (includes Application services)
builder.Services.AddInfrastructure();

// Add GraphQL with HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add SSE service
builder.Services.AddSingleton<AlertStreamService>();

// Add CORS for client applications
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",    // React GraphQL client
                "http://localhost:5003",    // Blazor SignalR client (Docker)
                "http://localhost:5124",    // Blazor SignalR client (dev)
                "http://localhost:5173",    // Vue SSE client
                "http://localhost:4200")    // Angular WebRTC client
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Alternative policy for Swagger/testing
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fleet Management API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Use WebSockets for SignalR and GraphQL subscriptions
app.UseWebSockets();

app.UseCors();
app.UseAuthorization();

// Map REST controllers
app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL("/graphql");

// Map SignalR hubs
app.MapHub<TrackingHub>("/hubs/tracking");
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<SignalingHub>("/hubs/signaling");

// Seed the database
app.Services.SeedDatabase();

Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════╗
║           Fleet Management API - Multi-Protocol Demo             ║
╠══════════════════════════════════════════════════════════════════╣
║  REST API:     https://localhost:5001/api/*                      ║
║  GraphQL:      https://localhost:5001/graphql                    ║
║  SignalR:      wss://localhost:5001/hubs/tracking                ║
║                wss://localhost:5001/hubs/chat                    ║
║                wss://localhost:5001/hubs/signaling               ║
║  SSE:          https://localhost:5001/sse/alerts                 ║
║  Swagger:      https://localhost:5001                            ║
╚══════════════════════════════════════════════════════════════════╝
");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
