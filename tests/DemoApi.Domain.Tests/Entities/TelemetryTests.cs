using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class TelemetryTests
{
    [Fact]
    public void Telemetry_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var telemetry = new Telemetry();

        // Assert
        telemetry.Id.Should().Be(Guid.Empty);
        telemetry.DeviceId.Should().Be(Guid.Empty);
        telemetry.VehicleId.Should().Be(Guid.Empty);
        telemetry.Type.Should().Be(TelemetryType.Location);
        telemetry.Value.Should().Be(0);
        telemetry.Unit.Should().BeEmpty();
    }

    [Fact]
    public void Telemetry_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var telemetry = new Telemetry
        {
            Id = id,
            DeviceId = deviceId,
            VehicleId = vehicleId,
            Type = TelemetryType.Speed,
            Value = 65.5,
            Unit = "km/h",
            Timestamp = now
        };

        // Assert
        telemetry.Id.Should().Be(id);
        telemetry.DeviceId.Should().Be(deviceId);
        telemetry.VehicleId.Should().Be(vehicleId);
        telemetry.Type.Should().Be(TelemetryType.Speed);
        telemetry.Value.Should().Be(65.5);
        telemetry.Unit.Should().Be("km/h");
        telemetry.Timestamp.Should().Be(now);
    }

    [Theory]
    [InlineData(TelemetryType.Location, 0, "degrees")]
    [InlineData(TelemetryType.Speed, 120, "km/h")]
    [InlineData(TelemetryType.FuelLevel, 75.5, "%")]
    [InlineData(TelemetryType.EngineTemperature, 95, "°C")]
    [InlineData(TelemetryType.BatteryVoltage, 12.6, "V")]
    [InlineData(TelemetryType.Odometer, 150000, "km")]
    public void Telemetry_ShouldAcceptAllTelemetryTypesWithValues(TelemetryType type, double value, string unit)
    {
        // Arrange & Act
        var telemetry = new Telemetry
        {
            Type = type,
            Value = value,
            Unit = unit
        };

        // Assert
        telemetry.Type.Should().Be(type);
        telemetry.Value.Should().Be(value);
        telemetry.Unit.Should().Be(unit);
    }

    [Fact]
    public void Telemetry_ShouldHandleNegativeValues()
    {
        // Arrange & Act
        var telemetry = new Telemetry
        {
            Type = TelemetryType.EngineTemperature,
            Value = -20.5, // Cold start in winter
            Unit = "°C"
        };

        // Assert
        telemetry.Value.Should().Be(-20.5);
    }

    [Fact]
    public void Telemetry_ShouldHandleZeroValues()
    {
        // Arrange & Act
        var telemetry = new Telemetry
        {
            Type = TelemetryType.Speed,
            Value = 0,
            Unit = "km/h"
        };

        // Assert
        telemetry.Value.Should().Be(0);
    }
}
