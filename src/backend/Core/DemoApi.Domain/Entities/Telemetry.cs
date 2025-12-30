using DemoApi.Domain.Enums;

namespace DemoApi.Domain.Entities;

public class Telemetry
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid VehicleId { get; set; }
    public TelemetryType Type { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public VehicleDevice? Device { get; set; }
}
