using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Application.Tests.Services;

public class AlertServiceTests
{
    private readonly AlertService _sut;

    public AlertServiceTests()
    {
        _sut = new AlertService();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenNoAlerts_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAlertsOrderedByCreatedAtDescending()
    {
        // Arrange
        var alert1 = await _sut.CreateAsync(new Alert { Message = "First" });
        await Task.Delay(10); // Ensure different timestamps
        var alert2 = await _sut.CreateAsync(new Alert { Message = "Second" });
        await Task.Delay(10);
        var alert3 = await _sut.CreateAsync(new Alert { Message = "Third" });

        // Act
        var result = (await _sut.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Message.Should().Be("Third");
        result[1].Message.Should().Be("Second");
        result[2].Message.Should().Be("First");
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldGenerateNewId()
    {
        // Arrange
        var alert = new Alert { Message = "Test alert" };

        // Act
        var result = await _sut.CreateAsync(alert);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var alert = new Alert { Message = "Test alert" };

        // Act
        var result = await _sut.CreateAsync(alert);
        var after = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(before);
        result.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetIsAcknowledgedToFalse()
    {
        // Arrange
        var alert = new Alert { Message = "Test alert", IsAcknowledged = true }; // Even if set to true

        // Act
        var result = await _sut.CreateAsync(alert);

        // Assert
        result.IsAcknowledged.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldPreserveAlertProperties()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var alert = new Alert
        {
            VehicleId = vehicleId,
            Type = AlertType.EngineWarning,
            Level = AlertLevel.Critical,
            Message = "Engine overheating"
        };

        // Act
        var result = await _sut.CreateAsync(alert);

        // Assert
        result.VehicleId.Should().Be(vehicleId);
        result.Type.Should().Be(AlertType.EngineWarning);
        result.Level.Should().Be(AlertLevel.Critical);
        result.Message.Should().Be("Engine overheating");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenAlertExists_ShouldReturnAlert()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Alert { Message = "Test alert" });

        // Act
        var result = await _sut.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Message.Should().Be("Test alert");
    }

    [Fact]
    public async Task GetByIdAsync_WhenAlertDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenAlertExists_ShouldReturnTrue()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Alert { Message = "Test alert" });

        // Act
        var result = await _sut.DeleteAsync(created.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenAlertExists_ShouldRemoveFromCollection()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Alert { Message = "Test alert" });

        // Act
        await _sut.DeleteAsync(created.Id);
        var allAlerts = await _sut.GetAllAsync();

        // Assert
        allAlerts.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenAlertDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetByVehicleIdAsync Tests

    [Fact]
    public async Task GetByVehicleIdAsync_ShouldReturnAlertsForVehicle()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        await _sut.CreateAsync(new Alert { VehicleId = vehicleId, Message = "Alert 1" });
        await _sut.CreateAsync(new Alert { VehicleId = vehicleId, Message = "Alert 2" });
        await _sut.CreateAsync(new Alert { VehicleId = Guid.NewGuid(), Message = "Other vehicle" });

        // Act
        var result = await _sut.GetByVehicleIdAsync(vehicleId);

        // Assert
        result.Should().HaveCount(2);
        result.All(a => a.VehicleId == vehicleId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByVehicleIdAsync_ShouldReturnOrderedByCreatedAtDescending()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        await _sut.CreateAsync(new Alert { VehicleId = vehicleId, Message = "First" });
        await Task.Delay(10);
        await _sut.CreateAsync(new Alert { VehicleId = vehicleId, Message = "Second" });

        // Act
        var result = (await _sut.GetByVehicleIdAsync(vehicleId)).ToList();

        // Assert
        result[0].Message.Should().Be("Second");
        result[1].Message.Should().Be("First");
    }

    #endregion

    #region GetUnacknowledgedAsync Tests

    [Fact]
    public async Task GetUnacknowledgedAsync_ShouldReturnOnlyUnacknowledgedAlerts()
    {
        // Arrange
        var alert1 = await _sut.CreateAsync(new Alert { Message = "Unack 1" });
        await _sut.CreateAsync(new Alert { Message = "Unack 2" });
        await _sut.AcknowledgeAsync(alert1.Id);

        // Act
        var result = await _sut.GetUnacknowledgedAsync();

        // Assert
        result.Should().ContainSingle();
        result.First().Message.Should().Be("Unack 2");
    }

    [Fact]
    public async Task GetUnacknowledgedAsync_WhenAllAcknowledged_ShouldReturnEmptyList()
    {
        // Arrange
        var alert = await _sut.CreateAsync(new Alert { Message = "Test" });
        await _sut.AcknowledgeAsync(alert.Id);

        // Act
        var result = await _sut.GetUnacknowledgedAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region AcknowledgeAsync Tests

    [Fact]
    public async Task AcknowledgeAsync_WhenAlertExists_ShouldSetIsAcknowledgedToTrue()
    {
        // Arrange
        var alert = await _sut.CreateAsync(new Alert { Message = "Test" });

        // Act
        var result = await _sut.AcknowledgeAsync(alert.Id);

        // Assert
        result.Should().NotBeNull();
        result!.IsAcknowledged.Should().BeTrue();
    }

    [Fact]
    public async Task AcknowledgeAsync_WhenAlertExists_ShouldSetAcknowledgedAt()
    {
        // Arrange
        var alert = await _sut.CreateAsync(new Alert { Message = "Test" });
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.AcknowledgeAsync(alert.Id);
        var after = DateTime.UtcNow;

        // Assert
        result!.AcknowledgedAt.Should().NotBeNull();
        result.AcknowledgedAt.Should().BeOnOrAfter(before);
        result.AcknowledgedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task AcknowledgeAsync_WhenAlertDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.AcknowledgeAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetRecentAlertsAsync Tests

    [Fact]
    public async Task GetRecentAlertsAsync_ShouldReturnSpecifiedCount()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            await _sut.CreateAsync(new Alert { Message = $"Alert {i}" });
        }

        // Act
        var result = await _sut.GetRecentAlertsAsync(5);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetRecentAlertsAsync_WithDefaultCount_ShouldReturn10()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            await _sut.CreateAsync(new Alert { Message = $"Alert {i}" });
        }

        // Act
        var result = await _sut.GetRecentAlertsAsync();

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetRecentAlertsAsync_ShouldReturnMostRecent()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            await _sut.CreateAsync(new Alert { Message = $"Alert {i}" });
            await Task.Delay(10);
        }

        // Act
        var result = (await _sut.GetRecentAlertsAsync(3)).ToList();

        // Assert
        result[0].Message.Should().Be("Alert 4");
        result[1].Message.Should().Be("Alert 3");
        result[2].Message.Should().Be("Alert 2");
    }

    [Fact]
    public async Task GetRecentAlertsAsync_WhenFewerThanCount_ShouldReturnAll()
    {
        // Arrange
        await _sut.CreateAsync(new Alert { Message = "Alert 1" });
        await _sut.CreateAsync(new Alert { Message = "Alert 2" });

        // Act
        var result = await _sut.GetRecentAlertsAsync(10);

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion
}
