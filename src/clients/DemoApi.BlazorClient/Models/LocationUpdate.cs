namespace DemoApi.BlazorClient.Models;

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

public class VehicleTrackingState
{
    public Guid VehicleId { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public DateTime LastUpdate { get; set; }
    public VehicleStatus Status { get; set; }
}
