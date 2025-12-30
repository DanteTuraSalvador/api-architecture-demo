using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface IFleetService
{
    Task<IEnumerable<Fleet>> GetAllAsync();
    Task<Fleet?> GetByIdAsync(Guid id);
    Task<Fleet> CreateAsync(Fleet fleet);
    Task<Fleet?> UpdateAsync(Guid id, Fleet fleet);
    Task<bool> DeleteAsync(Guid id);
    Task<Fleet?> GetWithVehiclesAsync(Guid id);
}
