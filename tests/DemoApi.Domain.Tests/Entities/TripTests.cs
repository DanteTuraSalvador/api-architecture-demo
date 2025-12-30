using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class TripTests
{
    [Fact]
    public void Trip_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var trip = new Trip();

        // Assert
        trip.Id.Should().Be(Guid.Empty);
        trip.VehicleId.Should().Be(Guid.Empty);
        trip.DriverId.Should().Be(Guid.Empty);
        trip.Status.Should().Be(TripStatus.Planned);
        trip.DistanceTraveled.Should().Be(0);
        trip.FuelConsumed.Should().Be(0);
        trip.EndTime.Should().BeNull();
        trip.EndLocation.Should().BeNull();
        trip.Deliveries.Should().BeEmpty();
    }

    [Fact]
    public void Trip_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var startLocation = new Location
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            Address = "New York, NY"
        };

        var endLocation = new Location
        {
            Latitude = 34.0522,
            Longitude = -118.2437,
            Address = "Los Angeles, CA"
        };

        // Act
        var trip = new Trip
        {
            Id = id,
            VehicleId = vehicleId,
            DriverId = driverId,
            StartTime = now,
            EndTime = now.AddHours(5),
            StartLocation = startLocation,
            EndLocation = endLocation,
            Status = TripStatus.Completed,
            DistanceTraveled = 500.5,
            FuelConsumed = 50.25,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        trip.Id.Should().Be(id);
        trip.VehicleId.Should().Be(vehicleId);
        trip.DriverId.Should().Be(driverId);
        trip.StartLocation.Address.Should().Be("New York, NY");
        trip.EndLocation!.Address.Should().Be("Los Angeles, CA");
        trip.Status.Should().Be(TripStatus.Completed);
        trip.DistanceTraveled.Should().Be(500.5);
        trip.FuelConsumed.Should().Be(50.25);
    }

    [Theory]
    [InlineData(TripStatus.Planned)]
    [InlineData(TripStatus.InProgress)]
    [InlineData(TripStatus.Completed)]
    [InlineData(TripStatus.Cancelled)]
    public void Trip_ShouldAcceptAllTripStatuses(TripStatus status)
    {
        // Arrange & Act
        var trip = new Trip { Status = status };

        // Assert
        trip.Status.Should().Be(status);
    }

    [Fact]
    public void Trip_ShouldAllowAddingDeliveries()
    {
        // Arrange
        var trip = new Trip { Id = Guid.NewGuid() };
        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            TripId = trip.Id,
            TrackingNumber = "TRK-001"
        };

        // Act
        trip.Deliveries.Add(delivery);

        // Assert
        trip.Deliveries.Should().ContainSingle();
        trip.Deliveries.First().TrackingNumber.Should().Be("TRK-001");
    }
}
