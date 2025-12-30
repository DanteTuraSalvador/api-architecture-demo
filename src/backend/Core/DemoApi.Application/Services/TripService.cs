using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Application.Services;

public class TripService : ITripService
{
    private readonly List<Trip> _trips = new();

    public Task<IEnumerable<Trip>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Trip>>(_trips.ToList());
    }

    public Task<Trip?> GetByIdAsync(Guid id)
    {
        var trip = _trips.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(trip);
    }

    public Task<Trip> CreateAsync(Trip trip)
    {
        trip.Id = Guid.NewGuid();
        trip.CreatedAt = DateTime.UtcNow;
        trip.Status = TripStatus.Planned;
        _trips.Add(trip);
        return Task.FromResult(trip);
    }

    public Task<Trip?> UpdateAsync(Guid id, Trip trip)
    {
        var existing = _trips.FirstOrDefault(t => t.Id == id);
        if (existing == null) return Task.FromResult<Trip?>(null);

        existing.VehicleId = trip.VehicleId;
        existing.DriverId = trip.DriverId;
        existing.StartTime = trip.StartTime;
        existing.EndTime = trip.EndTime;
        existing.StartLocation = trip.StartLocation;
        existing.EndLocation = trip.EndLocation;
        existing.Status = trip.Status;
        existing.DistanceTraveled = trip.DistanceTraveled;
        existing.FuelConsumed = trip.FuelConsumed;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Trip?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var trip = _trips.FirstOrDefault(t => t.Id == id);
        if (trip == null) return Task.FromResult(false);

        _trips.Remove(trip);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Trip>> GetByVehicleIdAsync(Guid vehicleId)
    {
        var trips = _trips.Where(t => t.VehicleId == vehicleId);
        return Task.FromResult<IEnumerable<Trip>>(trips.ToList());
    }

    public Task<IEnumerable<Trip>> GetByDriverIdAsync(Guid driverId)
    {
        var trips = _trips.Where(t => t.DriverId == driverId);
        return Task.FromResult<IEnumerable<Trip>>(trips.ToList());
    }

    public Task<IEnumerable<Trip>> GetActiveTripsAsync()
    {
        var trips = _trips.Where(t => t.Status == TripStatus.InProgress);
        return Task.FromResult<IEnumerable<Trip>>(trips.ToList());
    }

    public Task<Trip?> StartTripAsync(Guid tripId)
    {
        var trip = _trips.FirstOrDefault(t => t.Id == tripId);
        if (trip == null) return Task.FromResult<Trip?>(null);

        trip.Status = TripStatus.InProgress;
        trip.StartTime = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Trip?>(trip);
    }

    public Task<Trip?> CompleteTripAsync(Guid tripId)
    {
        var trip = _trips.FirstOrDefault(t => t.Id == tripId);
        if (trip == null) return Task.FromResult<Trip?>(null);

        trip.Status = TripStatus.Completed;
        trip.EndTime = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Trip?>(trip);
    }
}
