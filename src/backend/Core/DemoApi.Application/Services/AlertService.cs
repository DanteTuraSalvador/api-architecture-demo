using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;

namespace DemoApi.Application.Services;

public class AlertService : IAlertService
{
    private readonly List<Alert> _alerts = new();

    public Task<IEnumerable<Alert>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Alert>>(_alerts.OrderByDescending(a => a.CreatedAt).ToList());
    }

    public Task<Alert?> GetByIdAsync(Guid id)
    {
        var alert = _alerts.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(alert);
    }

    public Task<Alert> CreateAsync(Alert alert)
    {
        alert.Id = Guid.NewGuid();
        alert.CreatedAt = DateTime.UtcNow;
        alert.IsAcknowledged = false;
        _alerts.Add(alert);
        return Task.FromResult(alert);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var alert = _alerts.FirstOrDefault(a => a.Id == id);
        if (alert == null) return Task.FromResult(false);

        _alerts.Remove(alert);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Alert>> GetByVehicleIdAsync(Guid vehicleId)
    {
        var alerts = _alerts.Where(a => a.VehicleId == vehicleId).OrderByDescending(a => a.CreatedAt);
        return Task.FromResult<IEnumerable<Alert>>(alerts.ToList());
    }

    public Task<IEnumerable<Alert>> GetUnacknowledgedAsync()
    {
        var alerts = _alerts.Where(a => !a.IsAcknowledged).OrderByDescending(a => a.CreatedAt);
        return Task.FromResult<IEnumerable<Alert>>(alerts.ToList());
    }

    public Task<Alert?> AcknowledgeAsync(Guid id)
    {
        var alert = _alerts.FirstOrDefault(a => a.Id == id);
        if (alert == null) return Task.FromResult<Alert?>(null);

        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = DateTime.UtcNow;

        return Task.FromResult<Alert?>(alert);
    }

    public Task<IEnumerable<Alert>> GetRecentAlertsAsync(int count = 10)
    {
        var alerts = _alerts.OrderByDescending(a => a.CreatedAt).Take(count);
        return Task.FromResult<IEnumerable<Alert>>(alerts.ToList());
    }
}
