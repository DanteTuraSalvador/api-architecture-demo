using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class VehicleTests
{
    [Fact]
    public void Vehicle_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var vehicle = new Vehicle();

        // Assert
        vehicle.Id.Should().Be(Guid.Empty);
        vehicle.VehicleNumber.Should().BeEmpty();
        vehicle.LicensePlate.Should().BeEmpty();
        vehicle.Type.Should().Be(VehicleType.Truck);
        vehicle.Status.Should().Be(VehicleStatus.Available);
        vehicle.CurrentDriverId.Should().BeNull();
        vehicle.Devices.Should().BeEmpty();
        vehicle.Trips.Should().BeEmpty();
        vehicle.Alerts.Should().BeEmpty();
    }

    [Fact]
    public void Vehicle_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fleetId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var vehicle = new Vehicle
        {
            Id = id,
            VehicleNumber = "VH-001",
            LicensePlate = "ABC-1234",
            Type = VehicleType.Van,
            Status = VehicleStatus.InTransit,
            CurrentDriverId = driverId,
            FleetId = fleetId,
            LastMaintenance = now.AddDays(-30),
            Mileage = 50000.5,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        vehicle.Id.Should().Be(id);
        vehicle.VehicleNumber.Should().Be("VH-001");
        vehicle.LicensePlate.Should().Be("ABC-1234");
        vehicle.Type.Should().Be(VehicleType.Van);
        vehicle.Status.Should().Be(VehicleStatus.InTransit);
        vehicle.CurrentDriverId.Should().Be(driverId);
        vehicle.FleetId.Should().Be(fleetId);
        vehicle.Mileage.Should().Be(50000.5);
    }

    [Theory]
    [InlineData(VehicleType.Truck)]
    [InlineData(VehicleType.Van)]
    [InlineData(VehicleType.Car)]
    [InlineData(VehicleType.Motorcycle)]
    [InlineData(VehicleType.Bus)]
    public void Vehicle_ShouldAcceptAllVehicleTypes(VehicleType vehicleType)
    {
        // Arrange & Act
        var vehicle = new Vehicle { Type = vehicleType };

        // Assert
        vehicle.Type.Should().Be(vehicleType);
    }

    [Theory]
    [InlineData(VehicleStatus.Available)]
    [InlineData(VehicleStatus.InTransit)]
    [InlineData(VehicleStatus.Maintenance)]
    [InlineData(VehicleStatus.OutOfService)]
    public void Vehicle_ShouldAcceptAllVehicleStatuses(VehicleStatus status)
    {
        // Arrange & Act
        var vehicle = new Vehicle { Status = status };

        // Assert
        vehicle.Status.Should().Be(status);
    }

    [Fact]
    public void Vehicle_NavigationProperties_ShouldBeInitialized()
    {
        // Arrange & Act
        var vehicle = new Vehicle();

        // Assert
        vehicle.Devices.Should().NotBeNull();
        vehicle.Trips.Should().NotBeNull();
        vehicle.Alerts.Should().NotBeNull();
    }

    [Fact]
    public void Vehicle_ShouldAllowAddingDevices()
    {
        // Arrange
        var vehicle = new Vehicle();
        var device = new VehicleDevice
        {
            Id = Guid.NewGuid(),
            DeviceId = "GPS-001",
            Type = DeviceType.GPSTracker
        };

        // Act
        vehicle.Devices.Add(device);

        // Assert
        vehicle.Devices.Should().ContainSingle();
        vehicle.Devices.First().DeviceId.Should().Be("GPS-001");
    }
}
