using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Application.Tests.Services;

public class VehicleServiceTests
{
    private readonly VehicleService _sut;

    public VehicleServiceTests()
    {
        _sut = new VehicleService();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenNoVehicles_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenVehiclesExist_ShouldReturnAllVehicles()
    {
        // Arrange
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001" });
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-002" });
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-003" });

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenVehicleExists_ShouldReturnVehicle()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001" });

        // Act
        var result = await _sut.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.VehicleNumber.Should().Be("VH-001");
    }

    [Fact]
    public async Task GetByIdAsync_WhenVehicleDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldGenerateNewId()
    {
        // Arrange
        var vehicle = new Vehicle { VehicleNumber = "VH-001" };

        // Act
        var result = await _sut.CreateAsync(vehicle);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var vehicle = new Vehicle { VehicleNumber = "VH-001" };

        // Act
        var result = await _sut.CreateAsync(vehicle);
        var after = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(before);
        result.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddVehicleToCollection()
    {
        // Arrange
        var vehicle = new Vehicle { VehicleNumber = "VH-001" };

        // Act
        await _sut.CreateAsync(vehicle);
        var allVehicles = await _sut.GetAllAsync();

        // Assert
        allVehicles.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateAsync_ShouldPreserveVehicleProperties()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            VehicleNumber = "VH-001",
            LicensePlate = "ABC-1234",
            Type = VehicleType.Van,
            Status = VehicleStatus.Available,
            FleetId = Guid.NewGuid(),
            Mileage = 50000
        };

        // Act
        var result = await _sut.CreateAsync(vehicle);

        // Assert
        result.VehicleNumber.Should().Be("VH-001");
        result.LicensePlate.Should().Be("ABC-1234");
        result.Type.Should().Be(VehicleType.Van);
        result.Status.Should().Be(VehicleStatus.Available);
        result.Mileage.Should().Be(50000);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenVehicleExists_ShouldUpdateProperties()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Vehicle
        {
            VehicleNumber = "VH-001",
            Status = VehicleStatus.Available
        });

        var updateData = new Vehicle
        {
            VehicleNumber = "VH-001-UPDATED",
            LicensePlate = "XYZ-9999",
            Type = VehicleType.Truck,
            Status = VehicleStatus.InTransit,
            Mileage = 75000
        };

        // Act
        var result = await _sut.UpdateAsync(created.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.VehicleNumber.Should().Be("VH-001-UPDATED");
        result.LicensePlate.Should().Be("XYZ-9999");
        result.Type.Should().Be(VehicleType.Truck);
        result.Status.Should().Be(VehicleStatus.InTransit);
        result.Mileage.Should().Be(75000);
    }

    [Fact]
    public async Task UpdateAsync_WhenVehicleExists_ShouldSetUpdatedAt()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001" });
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.UpdateAsync(created.Id, new Vehicle { VehicleNumber = "VH-001-UPDATED" });
        var after = DateTime.UtcNow;

        // Assert
        result!.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeOnOrAfter(before);
        result.UpdatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task UpdateAsync_WhenVehicleDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new Vehicle { VehicleNumber = "VH-001" });

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenVehicleExists_ShouldReturnTrue()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001" });

        // Act
        var result = await _sut.DeleteAsync(created.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenVehicleExists_ShouldRemoveFromCollection()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001" });

        // Act
        await _sut.DeleteAsync(created.Id);
        var allVehicles = await _sut.GetAllAsync();

        // Assert
        allVehicles.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenVehicleDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetByFleetIdAsync Tests

    [Fact]
    public async Task GetByFleetIdAsync_ShouldReturnVehiclesInFleet()
    {
        // Arrange
        var fleetId = Guid.NewGuid();
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001", FleetId = fleetId });
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-002", FleetId = fleetId });
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-003", FleetId = Guid.NewGuid() });

        // Act
        var result = await _sut.GetByFleetIdAsync(fleetId);

        // Assert
        result.Should().HaveCount(2);
        result.All(v => v.FleetId == fleetId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByFleetIdAsync_WhenNoVehiclesInFleet_ShouldReturnEmptyList()
    {
        // Arrange
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001", FleetId = Guid.NewGuid() });

        // Act
        var result = await _sut.GetByFleetIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByStatusAsync Tests

    [Theory]
    [InlineData("Available", VehicleStatus.Available)]
    [InlineData("InTransit", VehicleStatus.InTransit)]
    [InlineData("Maintenance", VehicleStatus.Maintenance)]
    [InlineData("OutOfService", VehicleStatus.OutOfService)]
    public async Task GetByStatusAsync_ShouldReturnVehiclesWithMatchingStatus(string statusString, VehicleStatus status)
    {
        // Arrange
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001", Status = status });
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-002", Status = VehicleStatus.Available });

        // Act
        var result = await _sut.GetByStatusAsync(statusString);

        // Assert
        result.All(v => v.Status == status).Should().BeTrue();
    }

    [Theory]
    [InlineData("available")] // lowercase
    [InlineData("AVAILABLE")] // uppercase
    [InlineData("AvAiLaBlE")] // mixed case
    public async Task GetByStatusAsync_ShouldBeCaseInsensitive(string statusString)
    {
        // Arrange
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001", Status = VehicleStatus.Available });

        // Act
        var result = await _sut.GetByStatusAsync(statusString);

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_WithInvalidStatus_ShouldReturnEmptyList()
    {
        // Arrange
        await _sut.CreateAsync(new Vehicle { VehicleNumber = "VH-001", Status = VehicleStatus.Available });

        // Act
        var result = await _sut.GetByStatusAsync("InvalidStatus");

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
