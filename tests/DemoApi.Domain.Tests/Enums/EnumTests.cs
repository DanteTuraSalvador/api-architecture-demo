using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Enums;

public class EnumTests
{
    [Fact]
    public void VehicleType_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<VehicleType>().Should().HaveCount(5);
        ((int)VehicleType.Truck).Should().Be(0);
        ((int)VehicleType.Van).Should().Be(1);
        ((int)VehicleType.Car).Should().Be(2);
        ((int)VehicleType.Motorcycle).Should().Be(3);
        ((int)VehicleType.Bus).Should().Be(4);
    }

    [Fact]
    public void VehicleStatus_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<VehicleStatus>().Should().HaveCount(4);
        ((int)VehicleStatus.Available).Should().Be(0);
        ((int)VehicleStatus.InTransit).Should().Be(1);
        ((int)VehicleStatus.Maintenance).Should().Be(2);
        ((int)VehicleStatus.OutOfService).Should().Be(3);
    }

    [Fact]
    public void DriverStatus_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<DriverStatus>().Should().HaveCount(4);
        ((int)DriverStatus.Available).Should().Be(0);
        ((int)DriverStatus.OnDuty).Should().Be(1);
        ((int)DriverStatus.OffDuty).Should().Be(2);
        ((int)DriverStatus.OnBreak).Should().Be(3);
    }

    [Fact]
    public void TripStatus_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<TripStatus>().Should().HaveCount(4);
        ((int)TripStatus.Planned).Should().Be(0);
        ((int)TripStatus.InProgress).Should().Be(1);
        ((int)TripStatus.Completed).Should().Be(2);
        ((int)TripStatus.Cancelled).Should().Be(3);
    }

    [Fact]
    public void DeliveryStatus_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<DeliveryStatus>().Should().HaveCount(5);
        ((int)DeliveryStatus.Pending).Should().Be(0);
        ((int)DeliveryStatus.PickedUp).Should().Be(1);
        ((int)DeliveryStatus.InTransit).Should().Be(2);
        ((int)DeliveryStatus.Delivered).Should().Be(3);
        ((int)DeliveryStatus.Failed).Should().Be(4);
    }

    [Fact]
    public void AlertType_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<AlertType>().Should().HaveCount(8);
        ((int)AlertType.Speeding).Should().Be(0);
        ((int)AlertType.LowFuel).Should().Be(1);
        ((int)AlertType.EngineWarning).Should().Be(2);
        ((int)AlertType.MaintenanceDue).Should().Be(3);
        ((int)AlertType.Geofence).Should().Be(4);
        ((int)AlertType.Idling).Should().Be(5);
        ((int)AlertType.HarshBraking).Should().Be(6);
        ((int)AlertType.RapidAcceleration).Should().Be(7);
    }

    [Fact]
    public void AlertLevel_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<AlertLevel>().Should().HaveCount(3);
        ((int)AlertLevel.Info).Should().Be(0);
        ((int)AlertLevel.Warning).Should().Be(1);
        ((int)AlertLevel.Critical).Should().Be(2);
    }

    [Fact]
    public void DeviceType_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<DeviceType>().Should().HaveCount(6);
        ((int)DeviceType.GPSTracker).Should().Be(0);
        ((int)DeviceType.FuelSensor).Should().Be(1);
        ((int)DeviceType.TemperatureSensor).Should().Be(2);
        ((int)DeviceType.SpeedSensor).Should().Be(3);
        ((int)DeviceType.OBDDevice).Should().Be(4);
        ((int)DeviceType.Camera).Should().Be(5);
    }

    [Fact]
    public void TelemetryType_ShouldHaveExpectedValues()
    {
        // Assert
        Enum.GetValues<TelemetryType>().Should().HaveCount(6);
        ((int)TelemetryType.Location).Should().Be(0);
        ((int)TelemetryType.Speed).Should().Be(1);
        ((int)TelemetryType.FuelLevel).Should().Be(2);
        ((int)TelemetryType.EngineTemperature).Should().Be(3);
        ((int)TelemetryType.BatteryVoltage).Should().Be(4);
        ((int)TelemetryType.Odometer).Should().Be(5);
    }

    [Theory]
    [InlineData("Available", VehicleStatus.Available)]
    [InlineData("InTransit", VehicleStatus.InTransit)]
    [InlineData("Maintenance", VehicleStatus.Maintenance)]
    [InlineData("OutOfService", VehicleStatus.OutOfService)]
    public void VehicleStatus_ShouldParseFromString(string statusString, VehicleStatus expected)
    {
        // Act
        var parsed = Enum.Parse<VehicleStatus>(statusString);

        // Assert
        parsed.Should().Be(expected);
    }

    [Theory]
    [InlineData("Available", DriverStatus.Available)]
    [InlineData("OnDuty", DriverStatus.OnDuty)]
    [InlineData("OffDuty", DriverStatus.OffDuty)]
    [InlineData("OnBreak", DriverStatus.OnBreak)]
    public void DriverStatus_ShouldParseFromString(string statusString, DriverStatus expected)
    {
        // Act
        var parsed = Enum.Parse<DriverStatus>(statusString);

        // Assert
        parsed.Should().Be(expected);
    }

    [Fact]
    public void InvalidEnumString_ShouldThrowException()
    {
        // Act
        var action = () => Enum.Parse<VehicleStatus>("InvalidStatus");

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("available", true, VehicleStatus.Available)]
    [InlineData("INTRANSIT", true, VehicleStatus.InTransit)]
    [InlineData("Maintenance", true, VehicleStatus.Maintenance)]
    public void VehicleStatus_ShouldParseCaseInsensitive(string statusString, bool ignoreCase, VehicleStatus expected)
    {
        // Act
        var parsed = Enum.Parse<VehicleStatus>(statusString, ignoreCase);

        // Assert
        parsed.Should().Be(expected);
    }
}
