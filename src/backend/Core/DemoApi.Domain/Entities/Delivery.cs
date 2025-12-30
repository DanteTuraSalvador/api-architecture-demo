using DemoApi.Domain.Enums;

namespace DemoApi.Domain.Entities;

public class Delivery
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public Location PickupLocation { get; set; } = new();
    public Location DeliveryLocation { get; set; } = new();
    public DateTime? PickupTime { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public DeliveryStatus Status { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Trip? Trip { get; set; }
}
