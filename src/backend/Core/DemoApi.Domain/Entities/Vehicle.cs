using DemoApi.Domain.Enums;

namespace DemoApi.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType Type { get; set; }
    public VehicleStatus Status { get; set; }
    public Guid? CurrentDriverId { get; set; }
    public Guid FleetId { get; set; }
    public DateTime LastMaintenance { get; set; }
    public double Mileage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Fleet? Fleet { get; set; }
    public Driver? CurrentDriver { get; set; }
    public ICollection<VehicleDevice> Devices { get; set; } = new List<VehicleDevice>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
