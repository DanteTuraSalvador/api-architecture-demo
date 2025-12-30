using Microsoft.AspNetCore.SignalR;

namespace DemoApi.Gateway.Hubs;

/// <summary>
/// SignalR Hub for real-time vehicle tracking - demonstrates bidirectional real-time communication
/// </summary>
public class TrackingHub : Hub
{
    private readonly ILogger<TrackingHub> _logger;

    public TrackingHub(ILogger<TrackingHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Subscribe to a specific vehicle's location updates
    /// </summary>
    public async Task SubscribeToVehicle(Guid vehicleId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"vehicle-{vehicleId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to vehicle {VehicleId}",
            Context.ConnectionId, vehicleId);
    }

    /// <summary>
    /// Unsubscribe from a vehicle's location updates
    /// </summary>
    public async Task UnsubscribeFromVehicle(Guid vehicleId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"vehicle-{vehicleId}");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from vehicle {VehicleId}",
            Context.ConnectionId, vehicleId);
    }

    /// <summary>
    /// Subscribe to all vehicles in a fleet
    /// </summary>
    public async Task SubscribeToFleet(Guid fleetId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"fleet-{fleetId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to fleet {FleetId}",
            Context.ConnectionId, fleetId);
    }

    /// <summary>
    /// Send a location update (called by vehicle devices/simulators)
    /// </summary>
    public async Task SendLocationUpdate(Guid vehicleId, double latitude, double longitude, double speed)
    {
        var update = new LocationUpdate
        {
            VehicleId = vehicleId,
            Latitude = latitude,
            Longitude = longitude,
            Speed = speed,
            Timestamp = DateTime.UtcNow
        };

        await Clients.Group($"vehicle-{vehicleId}").SendAsync("LocationUpdated", update);
        await Clients.All.SendAsync("VehicleMoved", update);

        _logger.LogDebug("Location update for vehicle {VehicleId}: ({Lat}, {Lng})",
            vehicleId, latitude, longitude);
    }

    /// <summary>
    /// Broadcast an alert to all connected clients
    /// </summary>
    public async Task BroadcastAlert(Guid vehicleId, string alertType, string message)
    {
        var alert = new AlertNotification
        {
            VehicleId = vehicleId,
            AlertType = alertType,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await Clients.All.SendAsync("AlertReceived", alert);
    }
}

public record LocationUpdate
{
    public Guid VehicleId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double Speed { get; init; }
    public DateTime Timestamp { get; init; }
}

public record AlertNotification
{
    public Guid VehicleId { get; init; }
    public string AlertType { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}
