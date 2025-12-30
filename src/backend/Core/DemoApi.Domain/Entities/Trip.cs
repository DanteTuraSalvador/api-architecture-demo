using DemoApi.Domain.Enums;

namespace DemoApi.Domain.Entities;

public class Trip
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Location StartLocation { get; set; } = new();
    public Location? EndLocation { get; set; }
    public TripStatus Status { get; set; }
    public double DistanceTraveled { get; set; }
    public double FuelConsumed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Vehicle? Vehicle { get; set; }
    public Driver? Driver { get; set; }
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
