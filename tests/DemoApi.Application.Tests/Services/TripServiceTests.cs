using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Application.Tests.Services;

public class TripServiceTests
{
    private readonly TripService _sut;

    public TripServiceTests()
    {
        _sut = new TripService();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenNoTrips_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenTripsExist_ShouldReturnAllTrips()
    {
        // Arrange
        await _sut.CreateAsync(new Trip { VehicleId = Guid.NewGuid() });
        await _sut.CreateAsync(new Trip { VehicleId = Guid.NewGuid() });

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldGenerateNewId()
    {
        // Arrange
        var trip = new Trip { VehicleId = Guid.NewGuid() };

        // Act
        var result = await _sut.CreateAsync(trip);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetStatusToPlanned()
    {
        // Arrange
        var trip = new Trip { Status = TripStatus.Completed }; // Even if set differently

        // Act
        var result = await _sut.CreateAsync(trip);

        // Assert
        result.Status.Should().Be(TripStatus.Planned);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var trip = new Trip();

        // Act
        var result = await _sut.CreateAsync(trip);
        var after = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(before);
        result.CreatedAt.Should().BeOnOrBefore(after);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenTripExists_ShouldReturnTrip()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Trip { VehicleId = Guid.NewGuid() });

        // Act
        var result = await _sut.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTripDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenTripExists_ShouldUpdateProperties()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Trip());
        var newVehicleId = Guid.NewGuid();
        var newDriverId = Guid.NewGuid();

        var updateData = new Trip
        {
            VehicleId = newVehicleId,
            DriverId = newDriverId,
            Status = TripStatus.InProgress,
            DistanceTraveled = 150.5,
            FuelConsumed = 25.0
        };

        // Act
        var result = await _sut.UpdateAsync(created.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.VehicleId.Should().Be(newVehicleId);
        result.DriverId.Should().Be(newDriverId);
        result.Status.Should().Be(TripStatus.InProgress);
        result.DistanceTraveled.Should().Be(150.5);
        result.FuelConsumed.Should().Be(25.0);
    }

    [Fact]
    public async Task UpdateAsync_WhenTripExists_ShouldSetUpdatedAt()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Trip());
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.UpdateAsync(created.Id, new Trip());
        var after = DateTime.UtcNow;

        // Assert
        result!.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeOnOrAfter(before);
        result.UpdatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task UpdateAsync_WhenTripDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new Trip());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenTripExists_ShouldReturnTrue()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Trip());

        // Act
        var result = await _sut.DeleteAsync(created.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenTripExists_ShouldRemoveFromCollection()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Trip());

        // Act
        await _sut.DeleteAsync(created.Id);
        var allTrips = await _sut.GetAllAsync();

        // Assert
        allTrips.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenTripDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetByVehicleIdAsync Tests

    [Fact]
    public async Task GetByVehicleIdAsync_ShouldReturnTripsForVehicle()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        await _sut.CreateAsync(new Trip { VehicleId = vehicleId });
        await _sut.CreateAsync(new Trip { VehicleId = vehicleId });
        await _sut.CreateAsync(new Trip { VehicleId = Guid.NewGuid() });

        // Act
        var result = await _sut.GetByVehicleIdAsync(vehicleId);

        // Assert
        result.Should().HaveCount(2);
        result.All(t => t.VehicleId == vehicleId).Should().BeTrue();
    }

    #endregion

    #region GetByDriverIdAsync Tests

    [Fact]
    public async Task GetByDriverIdAsync_ShouldReturnTripsForDriver()
    {
        // Arrange
        var driverId = Guid.NewGuid();
        await _sut.CreateAsync(new Trip { DriverId = driverId });
        await _sut.CreateAsync(new Trip { DriverId = driverId });
        await _sut.CreateAsync(new Trip { DriverId = Guid.NewGuid() });

        // Act
        var result = await _sut.GetByDriverIdAsync(driverId);

        // Assert
        result.Should().HaveCount(2);
        result.All(t => t.DriverId == driverId).Should().BeTrue();
    }

    #endregion

    #region GetActiveTripsAsync Tests

    [Fact]
    public async Task GetActiveTripsAsync_ShouldReturnOnlyInProgressTrips()
    {
        // Arrange
        var trip1 = await _sut.CreateAsync(new Trip());
        await _sut.CreateAsync(new Trip());
        await _sut.StartTripAsync(trip1.Id);

        // Act
        var result = await _sut.GetActiveTripsAsync();

        // Assert
        result.Should().ContainSingle();
        result.First().Status.Should().Be(TripStatus.InProgress);
    }

    [Fact]
    public async Task GetActiveTripsAsync_WhenNoActiveTrips_ShouldReturnEmptyList()
    {
        // Arrange
        await _sut.CreateAsync(new Trip()); // Status = Planned

        // Act
        var result = await _sut.GetActiveTripsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region StartTripAsync Tests

    [Fact]
    public async Task StartTripAsync_WhenTripExists_ShouldSetStatusToInProgress()
    {
        // Arrange
        var trip = await _sut.CreateAsync(new Trip());

        // Act
        var result = await _sut.StartTripAsync(trip.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(TripStatus.InProgress);
    }

    [Fact]
    public async Task StartTripAsync_WhenTripExists_ShouldSetStartTime()
    {
        // Arrange
        var trip = await _sut.CreateAsync(new Trip());
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.StartTripAsync(trip.Id);
        var after = DateTime.UtcNow;

        // Assert
        result!.StartTime.Should().BeOnOrAfter(before);
        result.StartTime.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task StartTripAsync_WhenTripExists_ShouldSetUpdatedAt()
    {
        // Arrange
        var trip = await _sut.CreateAsync(new Trip());
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.StartTripAsync(trip.Id);
        var after = DateTime.UtcNow;

        // Assert
        result!.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeOnOrAfter(before);
        result.UpdatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task StartTripAsync_WhenTripDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.StartTripAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CompleteTripAsync Tests

    [Fact]
    public async Task CompleteTripAsync_WhenTripExists_ShouldSetStatusToCompleted()
    {
        // Arrange
        var trip = await _sut.CreateAsync(new Trip());
        await _sut.StartTripAsync(trip.Id);

        // Act
        var result = await _sut.CompleteTripAsync(trip.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(TripStatus.Completed);
    }

    [Fact]
    public async Task CompleteTripAsync_WhenTripExists_ShouldSetEndTime()
    {
        // Arrange
        var trip = await _sut.CreateAsync(new Trip());
        await _sut.StartTripAsync(trip.Id);
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.CompleteTripAsync(trip.Id);
        var after = DateTime.UtcNow;

        // Assert
        result!.EndTime.Should().NotBeNull();
        result.EndTime.Should().BeOnOrAfter(before);
        result.EndTime.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task CompleteTripAsync_WhenTripDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.CompleteTripAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
