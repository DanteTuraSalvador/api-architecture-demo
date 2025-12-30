using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Application.Services;

public class DriverService : IDriverService
{
    private readonly List<Driver> _drivers = new();

    public Task<IEnumerable<Driver>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Driver>>(_drivers.ToList());
    }

    public Task<Driver?> GetByIdAsync(Guid id)
    {
        var driver = _drivers.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(driver);
    }

    public Task<Driver> CreateAsync(Driver driver)
    {
        driver.Id = Guid.NewGuid();
        driver.CreatedAt = DateTime.UtcNow;
        _drivers.Add(driver);
        return Task.FromResult(driver);
    }

    public Task<Driver?> UpdateAsync(Guid id, Driver driver)
    {
        var existing = _drivers.FirstOrDefault(d => d.Id == id);
        if (existing == null) return Task.FromResult<Driver?>(null);

        existing.FirstName = driver.FirstName;
        existing.LastName = driver.LastName;
        existing.LicenseNumber = driver.LicenseNumber;
        existing.PhoneNumber = driver.PhoneNumber;
        existing.Email = driver.Email;
        existing.Status = driver.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Driver?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var driver = _drivers.FirstOrDefault(d => d.Id == id);
        if (driver == null) return Task.FromResult(false);

        _drivers.Remove(driver);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Driver>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<DriverStatus>(status, true, out var driverStatus))
            return Task.FromResult<IEnumerable<Driver>>(new List<Driver>());

        var drivers = _drivers.Where(d => d.Status == driverStatus);
        return Task.FromResult<IEnumerable<Driver>>(drivers.ToList());
    }

    public Task<IEnumerable<Driver>> GetAvailableDriversAsync()
    {
        var drivers = _drivers.Where(d => d.Status == DriverStatus.Available);
        return Task.FromResult<IEnumerable<Driver>>(drivers.ToList());
    }
}
