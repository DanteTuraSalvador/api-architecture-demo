using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task<Vehicle?> UpdateAsync(Guid id, Vehicle vehicle);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Vehicle>> GetByFleetIdAsync(Guid fleetId);
    Task<IEnumerable<Vehicle>> GetByStatusAsync(string status);
}
