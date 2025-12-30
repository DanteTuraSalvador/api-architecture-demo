using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Application.Services;

public class DeliveryService : IDeliveryService
{
    private readonly List<Delivery> _deliveries = new();

    public Task<IEnumerable<Delivery>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Delivery>>(_deliveries.ToList());
    }

    public Task<Delivery?> GetByIdAsync(Guid id)
    {
        var delivery = _deliveries.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(delivery);
    }

    public Task<Delivery> CreateAsync(Delivery delivery)
    {
        delivery.Id = Guid.NewGuid();
        delivery.CreatedAt = DateTime.UtcNow;
        delivery.TrackingNumber = GenerateTrackingNumber();
        delivery.Status = DeliveryStatus.Pending;
        _deliveries.Add(delivery);
        return Task.FromResult(delivery);
    }

    public Task<Delivery?> UpdateAsync(Guid id, Delivery delivery)
    {
        var existing = _deliveries.FirstOrDefault(d => d.Id == id);
        if (existing == null) return Task.FromResult<Delivery?>(null);

        existing.TripId = delivery.TripId;
        existing.PickupLocation = delivery.PickupLocation;
        existing.DeliveryLocation = delivery.DeliveryLocation;
        existing.RecipientName = delivery.RecipientName;
        existing.RecipientPhone = delivery.RecipientPhone;
        existing.Status = delivery.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Delivery?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var delivery = _deliveries.FirstOrDefault(d => d.Id == id);
        if (delivery == null) return Task.FromResult(false);

        _deliveries.Remove(delivery);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Delivery>> GetByTripIdAsync(Guid tripId)
    {
        var deliveries = _deliveries.Where(d => d.TripId == tripId);
        return Task.FromResult<IEnumerable<Delivery>>(deliveries.ToList());
    }

    public Task<Delivery?> GetByTrackingNumberAsync(string trackingNumber)
    {
        var delivery = _deliveries.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
        return Task.FromResult(delivery);
    }

    public Task<Delivery?> MarkAsPickedUpAsync(Guid id)
    {
        var delivery = _deliveries.FirstOrDefault(d => d.Id == id);
        if (delivery == null) return Task.FromResult<Delivery?>(null);

        delivery.Status = DeliveryStatus.PickedUp;
        delivery.PickupTime = DateTime.UtcNow;
        delivery.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Delivery?>(delivery);
    }

    public Task<Delivery?> MarkAsDeliveredAsync(Guid id)
    {
        var delivery = _deliveries.FirstOrDefault(d => d.Id == id);
        if (delivery == null) return Task.FromResult<Delivery?>(null);

        delivery.Status = DeliveryStatus.Delivered;
        delivery.DeliveryTime = DateTime.UtcNow;
        delivery.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Delivery?>(delivery);
    }

    private static string GenerateTrackingNumber()
    {
        return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
