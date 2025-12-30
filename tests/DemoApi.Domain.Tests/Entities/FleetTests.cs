using DemoApi.Domain.Entities;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class FleetTests
{
    [Fact]
    public void Fleet_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var fleet = new Fleet();

        // Assert
        fleet.Id.Should().Be(Guid.Empty);
        fleet.Name.Should().BeEmpty();
        fleet.CompanyName.Should().BeEmpty();
        fleet.Vehicles.Should().BeEmpty();
    }

    [Fact]
    public void Fleet_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var fleet = new Fleet
        {
            Id = id,
            Name = "East Coast Fleet",
            CompanyName = "Logistics Corp",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        fleet.Id.Should().Be(id);
        fleet.Name.Should().Be("East Coast Fleet");
        fleet.CompanyName.Should().Be("Logistics Corp");
        fleet.CreatedAt.Should().Be(now);
        fleet.UpdatedAt.Should().Be(now);
    }

    [Fact]
    public void Fleet_ShouldAllowAddingVehicles()
    {
        // Arrange
        var fleet = new Fleet
        {
            Id = Guid.NewGuid(),
            Name = "Test Fleet"
        };

        var vehicle1 = new Vehicle { Id = Guid.NewGuid(), VehicleNumber = "VH-001" };
        var vehicle2 = new Vehicle { Id = Guid.NewGuid(), VehicleNumber = "VH-002" };

        // Act
        fleet.Vehicles.Add(vehicle1);
        fleet.Vehicles.Add(vehicle2);

        // Assert
        fleet.Vehicles.Should().HaveCount(2);
        fleet.Vehicles.Should().Contain(v => v.VehicleNumber == "VH-001");
        fleet.Vehicles.Should().Contain(v => v.VehicleNumber == "VH-002");
    }

    [Fact]
    public void Fleet_Vehicles_ShouldBeModifiable()
    {
        // Arrange
        var fleet = new Fleet { Id = Guid.NewGuid() };
        var vehicle = new Vehicle { Id = Guid.NewGuid(), VehicleNumber = "VH-001" };
        fleet.Vehicles.Add(vehicle);

        // Act
        fleet.Vehicles.Remove(vehicle);

        // Assert
        fleet.Vehicles.Should().BeEmpty();
    }
}
