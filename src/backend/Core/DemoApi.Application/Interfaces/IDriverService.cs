using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface IDriverService
{
    Task<IEnumerable<Driver>> GetAllAsync();
    Task<Driver?> GetByIdAsync(Guid id);
    Task<Driver> CreateAsync(Driver driver);
    Task<Driver?> UpdateAsync(Guid id, Driver driver);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Driver>> GetByStatusAsync(string status);
    Task<IEnumerable<Driver>> GetAvailableDriversAsync();
}
