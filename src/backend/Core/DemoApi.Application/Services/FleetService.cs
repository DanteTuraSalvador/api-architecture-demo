using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;

namespace DemoApi.Application.Services;

public class FleetService : IFleetService
{
    private readonly List<Fleet> _fleets = new();

    public Task<IEnumerable<Fleet>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Fleet>>(_fleets.ToList());
    }

    public Task<Fleet?> GetByIdAsync(Guid id)
    {
        var fleet = _fleets.FirstOrDefault(f => f.Id == id);
        return Task.FromResult(fleet);
    }

    public Task<Fleet> CreateAsync(Fleet fleet)
    {
        fleet.Id = Guid.NewGuid();
        fleet.CreatedAt = DateTime.UtcNow;
        _fleets.Add(fleet);
        return Task.FromResult(fleet);
    }

    public Task<Fleet?> UpdateAsync(Guid id, Fleet fleet)
    {
        var existing = _fleets.FirstOrDefault(f => f.Id == id);
        if (existing == null) return Task.FromResult<Fleet?>(null);

        existing.Name = fleet.Name;
        existing.CompanyName = fleet.CompanyName;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Fleet?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var fleet = _fleets.FirstOrDefault(f => f.Id == id);
        if (fleet == null) return Task.FromResult(false);

        _fleets.Remove(fleet);
        return Task.FromResult(true);
    }

    public Task<Fleet?> GetWithVehiclesAsync(Guid id)
    {
        var fleet = _fleets.FirstOrDefault(f => f.Id == id);
        return Task.FromResult(fleet);
    }
}
