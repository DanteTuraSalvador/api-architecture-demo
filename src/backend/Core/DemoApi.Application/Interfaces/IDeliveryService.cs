using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface IDeliveryService
{
    Task<IEnumerable<Delivery>> GetAllAsync();
    Task<Delivery?> GetByIdAsync(Guid id);
    Task<Delivery> CreateAsync(Delivery delivery);
    Task<Delivery?> UpdateAsync(Guid id, Delivery delivery);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Delivery>> GetByTripIdAsync(Guid tripId);
    Task<Delivery?> GetByTrackingNumberAsync(string trackingNumber);
    Task<Delivery?> MarkAsPickedUpAsync(Guid id);
    Task<Delivery?> MarkAsDeliveredAsync(Guid id);
}
