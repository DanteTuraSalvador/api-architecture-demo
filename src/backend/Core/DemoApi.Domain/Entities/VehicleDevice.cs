using DemoApi.Domain.Enums;

namespace DemoApi.Domain.Entities;

public class VehicleDevice
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public Guid VehicleId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Vehicle? Vehicle { get; set; }
    public ICollection<Telemetry> TelemetryData { get; set; } = new List<Telemetry>();
}
