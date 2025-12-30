using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface ITripService
{
    Task<IEnumerable<Trip>> GetAllAsync();
    Task<Trip?> GetByIdAsync(Guid id);
    Task<Trip> CreateAsync(Trip trip);
    Task<Trip?> UpdateAsync(Guid id, Trip trip);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Trip>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Trip>> GetByDriverIdAsync(Guid driverId);
    Task<IEnumerable<Trip>> GetActiveTripsAsync();
    Task<Trip?> StartTripAsync(Guid tripId);
    Task<Trip?> CompleteTripAsync(Guid tripId);
}
