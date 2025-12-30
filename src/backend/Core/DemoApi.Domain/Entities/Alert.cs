using DemoApi.Domain.Enums;

namespace DemoApi.Domain.Entities;

public class Alert
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public AlertType Type { get; set; }
    public AlertLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }

    public Vehicle? Vehicle { get; set; }
}
