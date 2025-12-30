using DemoApi.MqttBroker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();

// Add MQTT broker service
builder.Services.AddHostedService<MqttBrokerService>();

Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════╗
║              MQTT Broker - Fleet Telemetry                       ║
╠══════════════════════════════════════════════════════════════════╣
║  Port: 1883 (TCP)                                                ║
║  Topics:                                                         ║
║    - fleet/+/vehicle/+/telemetry                                 ║
║    - fleet/+/vehicle/+/location                                  ║
║    - fleet/+/vehicle/+/alert                                     ║
║    - fleet/+/vehicle/+/status                                    ║
╚══════════════════════════════════════════════════════════════════╝
");

var host = builder.Build();
await host.RunAsync();
