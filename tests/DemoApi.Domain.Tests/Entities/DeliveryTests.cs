using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class DeliveryTests
{
    [Fact]
    public void Delivery_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var delivery = new Delivery();

        // Assert
        delivery.Id.Should().Be(Guid.Empty);
        delivery.TripId.Should().Be(Guid.Empty);
        delivery.TrackingNumber.Should().BeEmpty();
        delivery.Status.Should().Be(DeliveryStatus.Pending);
        delivery.RecipientName.Should().BeEmpty();
        delivery.RecipientPhone.Should().BeEmpty();
        delivery.PickupTime.Should().BeNull();
        delivery.DeliveryTime.Should().BeNull();
    }

    [Fact]
    public void Delivery_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var pickupLocation = new Location
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            Address = "123 Warehouse St, NY"
        };

        var deliveryLocation = new Location
        {
            Latitude = 40.7580,
            Longitude = -73.9855,
            Address = "456 Customer Ave, NY"
        };

        // Act
        var delivery = new Delivery
        {
            Id = id,
            TripId = tripId,
            TrackingNumber = "TRK-20231215-ABC12345",
            PickupLocation = pickupLocation,
            DeliveryLocation = deliveryLocation,
            PickupTime = now,
            DeliveryTime = now.AddHours(2),
            Status = DeliveryStatus.Delivered,
            RecipientName = "John Customer",
            RecipientPhone = "+1-555-987-6543",
            CreatedAt = now.AddHours(-3),
            UpdatedAt = now
        };

        // Assert
        delivery.Id.Should().Be(id);
        delivery.TripId.Should().Be(tripId);
        delivery.TrackingNumber.Should().Be("TRK-20231215-ABC12345");
        delivery.PickupLocation.Address.Should().Be("123 Warehouse St, NY");
        delivery.DeliveryLocation.Address.Should().Be("456 Customer Ave, NY");
        delivery.Status.Should().Be(DeliveryStatus.Delivered);
        delivery.RecipientName.Should().Be("John Customer");
        delivery.RecipientPhone.Should().Be("+1-555-987-6543");
    }

    [Theory]
    [InlineData(DeliveryStatus.Pending)]
    [InlineData(DeliveryStatus.PickedUp)]
    [InlineData(DeliveryStatus.InTransit)]
    [InlineData(DeliveryStatus.Delivered)]
    [InlineData(DeliveryStatus.Failed)]
    public void Delivery_ShouldAcceptAllDeliveryStatuses(DeliveryStatus status)
    {
        // Arrange & Act
        var delivery = new Delivery { Status = status };

        // Assert
        delivery.Status.Should().Be(status);
    }

    [Fact]
    public void Delivery_StatusTransition_ShouldWorkCorrectly()
    {
        // Arrange
        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            Status = DeliveryStatus.Pending
        };

        // Act & Assert - Pending to PickedUp
        delivery.Status = DeliveryStatus.PickedUp;
        delivery.PickupTime = DateTime.UtcNow;
        delivery.Status.Should().Be(DeliveryStatus.PickedUp);
        delivery.PickupTime.Should().NotBeNull();

        // Act & Assert - PickedUp to InTransit
        delivery.Status = DeliveryStatus.InTransit;
        delivery.Status.Should().Be(DeliveryStatus.InTransit);

        // Act & Assert - InTransit to Delivered
        delivery.Status = DeliveryStatus.Delivered;
        delivery.DeliveryTime = DateTime.UtcNow;
        delivery.Status.Should().Be(DeliveryStatus.Delivered);
        delivery.DeliveryTime.Should().NotBeNull();
    }
}
