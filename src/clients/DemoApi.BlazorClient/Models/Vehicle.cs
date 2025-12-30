namespace DemoApi.BlazorClient.Models;

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
}

public enum VehicleType
{
    Truck,
    Van,
    Car,
    Motorcycle,
    Bus
}

public enum VehicleStatus
{
    Available,
    InTransit,
    Maintenance,
    OutOfService
}
