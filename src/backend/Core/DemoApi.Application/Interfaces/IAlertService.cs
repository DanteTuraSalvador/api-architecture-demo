using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface IAlertService
{
    Task<IEnumerable<Alert>> GetAllAsync();
    Task<Alert?> GetByIdAsync(Guid id);
    Task<Alert> CreateAsync(Alert alert);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Alert>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Alert>> GetUnacknowledgedAsync();
    Task<Alert?> AcknowledgeAsync(Guid id);
    Task<IEnumerable<Alert>> GetRecentAlertsAsync(int count = 10);
}
