using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class AlertTests
{
    [Fact]
    public void Alert_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var alert = new Alert();

        // Assert
        alert.Id.Should().Be(Guid.Empty);
        alert.VehicleId.Should().Be(Guid.Empty);
        alert.Type.Should().Be(AlertType.Speeding);
        alert.Level.Should().Be(AlertLevel.Info);
        alert.Message.Should().BeEmpty();
        alert.IsAcknowledged.Should().BeFalse();
        alert.AcknowledgedAt.Should().BeNull();
    }

    [Fact]
    public void Alert_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var alert = new Alert
        {
            Id = id,
            VehicleId = vehicleId,
            Type = AlertType.EngineWarning,
            Level = AlertLevel.Critical,
            Message = "Engine temperature exceeds safe limits",
            CreatedAt = now,
            IsAcknowledged = true,
            AcknowledgedAt = now.AddMinutes(5)
        };

        // Assert
        alert.Id.Should().Be(id);
        alert.VehicleId.Should().Be(vehicleId);
        alert.Type.Should().Be(AlertType.EngineWarning);
        alert.Level.Should().Be(AlertLevel.Critical);
        alert.Message.Should().Be("Engine temperature exceeds safe limits");
        alert.IsAcknowledged.Should().BeTrue();
        alert.AcknowledgedAt.Should().Be(now.AddMinutes(5));
    }

    [Theory]
    [InlineData(AlertType.Speeding)]
    [InlineData(AlertType.LowFuel)]
    [InlineData(AlertType.EngineWarning)]
    [InlineData(AlertType.MaintenanceDue)]
    [InlineData(AlertType.Geofence)]
    [InlineData(AlertType.Idling)]
    [InlineData(AlertType.HarshBraking)]
    [InlineData(AlertType.RapidAcceleration)]
    public void Alert_ShouldAcceptAllAlertTypes(AlertType alertType)
    {
        // Arrange & Act
        var alert = new Alert { Type = alertType };

        // Assert
        alert.Type.Should().Be(alertType);
    }

    [Theory]
    [InlineData(AlertLevel.Info)]
    [InlineData(AlertLevel.Warning)]
    [InlineData(AlertLevel.Critical)]
    public void Alert_ShouldAcceptAllAlertLevels(AlertLevel level)
    {
        // Arrange & Act
        var alert = new Alert { Level = level };

        // Assert
        alert.Level.Should().Be(level);
    }

    [Fact]
    public void Alert_Acknowledge_ShouldUpdateProperties()
    {
        // Arrange
        var alert = new Alert
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            Type = AlertType.LowFuel,
            Level = AlertLevel.Warning,
            Message = "Fuel level below 20%",
            IsAcknowledged = false
        };

        var acknowledgedTime = DateTime.UtcNow;

        // Act
        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = acknowledgedTime;

        // Assert
        alert.IsAcknowledged.Should().BeTrue();
        alert.AcknowledgedAt.Should().Be(acknowledgedTime);
    }
}
