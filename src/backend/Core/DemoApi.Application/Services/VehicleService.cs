using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly List<Vehicle> _vehicles = new();

    public Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Vehicle>>(_vehicles.ToList());
    }

    public Task<Vehicle?> GetByIdAsync(Guid id)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
        return Task.FromResult(vehicle);
    }

    public Task<Vehicle> CreateAsync(Vehicle vehicle)
    {
        vehicle.Id = Guid.NewGuid();
        vehicle.CreatedAt = DateTime.UtcNow;
        _vehicles.Add(vehicle);
        return Task.FromResult(vehicle);
    }

    public Task<Vehicle?> UpdateAsync(Guid id, Vehicle vehicle)
    {
        var existing = _vehicles.FirstOrDefault(v => v.Id == id);
        if (existing == null) return Task.FromResult<Vehicle?>(null);

        existing.VehicleNumber = vehicle.VehicleNumber;
        existing.LicensePlate = vehicle.LicensePlate;
        existing.Type = vehicle.Type;
        existing.Status = vehicle.Status;
        existing.CurrentDriverId = vehicle.CurrentDriverId;
        existing.Mileage = vehicle.Mileage;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Vehicle?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
        if (vehicle == null) return Task.FromResult(false);

        _vehicles.Remove(vehicle);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Vehicle>> GetByFleetIdAsync(Guid fleetId)
    {
        var vehicles = _vehicles.Where(v => v.FleetId == fleetId);
        return Task.FromResult<IEnumerable<Vehicle>>(vehicles.ToList());
    }

    public Task<IEnumerable<Vehicle>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<VehicleStatus>(status, true, out var vehicleStatus))
            return Task.FromResult<IEnumerable<Vehicle>>(new List<Vehicle>());

        var vehicles = _vehicles.Where(v => v.Status == vehicleStatus);
        return Task.FromResult<IEnumerable<Vehicle>>(vehicles.ToList());
    }
}
