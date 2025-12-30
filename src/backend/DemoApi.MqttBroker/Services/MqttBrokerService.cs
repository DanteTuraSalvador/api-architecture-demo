using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;

namespace DemoApi.MqttBroker.Services;

/// <summary>
/// MQTT broker service for fleet telemetry - demonstrates pub/sub messaging
/// </summary>
public class MqttBrokerService : BackgroundService
{
    private readonly ILogger<MqttBrokerService> _logger;
    private MqttServer? _mqttServer;

    public MqttBrokerService(ILogger<MqttBrokerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttFactory();

        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883);

        _mqttServer = mqttFactory.CreateMqttServer(optionsBuilder.Build());

        // Handle client connections
        _mqttServer.ClientConnectedAsync += args =>
        {
            _logger.LogInformation("Client connected: {ClientId}", args.ClientId);
            return Task.CompletedTask;
        };

        // Handle client disconnections
        _mqttServer.ClientDisconnectedAsync += args =>
        {
            _logger.LogInformation("Client disconnected: {ClientId}, Reason: {Reason}",
                args.ClientId, args.DisconnectType);
            return Task.CompletedTask;
        };

        // Handle incoming messages
        _mqttServer.InterceptingPublishAsync += args =>
        {
            var topic = args.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

            _logger.LogDebug("Message received - Topic: {Topic}, Payload: {Payload}",
                topic, payload);

            // Parse fleet telemetry topics
            if (TryParseFleetTopic(topic, out var fleetId, out var vehicleId, out var messageType))
            {
                ProcessFleetMessage(fleetId, vehicleId, messageType, payload);
            }

            return Task.CompletedTask;
        };

        // Handle subscriptions
        _mqttServer.ClientSubscribedTopicAsync += args =>
        {
            _logger.LogInformation("Client {ClientId} subscribed to: {Topic}",
                args.ClientId, args.TopicFilter.Topic);
            return Task.CompletedTask;
        };

        await _mqttServer.StartAsync();
        _logger.LogInformation("MQTT Broker started on port 1883");

        // Keep the service running
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Expected when stopping
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttServer != null)
        {
            await _mqttServer.StopAsync();
            _logger.LogInformation("MQTT Broker stopped");
        }

        await base.StopAsync(cancellationToken);
    }

    private bool TryParseFleetTopic(string topic, out string? fleetId, out string? vehicleId, out string? messageType)
    {
        fleetId = null;
        vehicleId = null;
        messageType = null;

        // Expected format: fleet/{fleetId}/vehicle/{vehicleId}/{messageType}
        var parts = topic.Split('/');
        if (parts.Length >= 5 && parts[0] == "fleet" && parts[2] == "vehicle")
        {
            fleetId = parts[1];
            vehicleId = parts[3];
            messageType = parts[4];
            return true;
        }

        return false;
    }

    private void ProcessFleetMessage(string? fleetId, string? vehicleId, string? messageType, string payload)
    {
        switch (messageType?.ToLower())
        {
            case "telemetry":
                ProcessTelemetry(fleetId, vehicleId, payload);
                break;
            case "location":
                ProcessLocation(fleetId, vehicleId, payload);
                break;
            case "alert":
                ProcessAlert(fleetId, vehicleId, payload);
                break;
            case "status":
                ProcessStatus(fleetId, vehicleId, payload);
                break;
            default:
                _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                break;
        }
    }

    private void ProcessTelemetry(string? fleetId, string? vehicleId, string payload)
    {
        _logger.LogInformation("Telemetry from Fleet {FleetId}, Vehicle {VehicleId}: {Payload}",
            fleetId, vehicleId, payload);
    }

    private void ProcessLocation(string? fleetId, string? vehicleId, string payload)
    {
        try
        {
            var location = JsonSerializer.Deserialize<LocationPayload>(payload);
            _logger.LogInformation(
                "Location update - Fleet: {FleetId}, Vehicle: {VehicleId}, Lat: {Lat}, Lon: {Lon}, Speed: {Speed}",
                fleetId, vehicleId, location?.Latitude, location?.Longitude, location?.Speed);
        }
        catch (JsonException)
        {
            _logger.LogWarning("Invalid location payload: {Payload}", payload);
        }
    }

    private void ProcessAlert(string? fleetId, string? vehicleId, string payload)
    {
        _logger.LogWarning("Alert from Fleet {FleetId}, Vehicle {VehicleId}: {Payload}",
            fleetId, vehicleId, payload);
    }

    private void ProcessStatus(string? fleetId, string? vehicleId, string payload)
    {
        _logger.LogInformation("Status update - Fleet: {FleetId}, Vehicle: {VehicleId}: {Payload}",
            fleetId, vehicleId, payload);
    }
}

/// <summary>
/// Location payload for GPS data
/// </summary>
public class LocationPayload
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public DateTime Timestamp { get; set; }
}
