using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class DriverTests
{
    [Fact]
    public void Driver_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var driver = new Driver();

        // Assert
        driver.Id.Should().Be(Guid.Empty);
        driver.FirstName.Should().BeEmpty();
        driver.LastName.Should().BeEmpty();
        driver.LicenseNumber.Should().BeEmpty();
        driver.PhoneNumber.Should().BeEmpty();
        driver.Email.Should().BeEmpty();
        driver.Status.Should().Be(DriverStatus.Available);
        driver.Trips.Should().BeEmpty();
    }

    [Fact]
    public void Driver_FullName_ShouldCombineFirstAndLastName()
    {
        // Arrange
        var driver = new Driver
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var fullName = driver.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Theory]
    [InlineData("", "", " ")]
    [InlineData("John", "", "John ")]
    [InlineData("", "Doe", " Doe")]
    [InlineData("John", "Doe", "John Doe")]
    public void Driver_FullName_ShouldHandleVariousCombinations(string firstName, string lastName, string expectedFullName)
    {
        // Arrange
        var driver = new Driver
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Act & Assert
        driver.FullName.Should().Be(expectedFullName);
    }

    [Fact]
    public void Driver_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var driver = new Driver
        {
            Id = id,
            FirstName = "Jane",
            LastName = "Smith",
            LicenseNumber = "DL-123456",
            PhoneNumber = "+1-555-123-4567",
            Email = "jane.smith@example.com",
            Status = DriverStatus.OnDuty,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        driver.Id.Should().Be(id);
        driver.FirstName.Should().Be("Jane");
        driver.LastName.Should().Be("Smith");
        driver.LicenseNumber.Should().Be("DL-123456");
        driver.PhoneNumber.Should().Be("+1-555-123-4567");
        driver.Email.Should().Be("jane.smith@example.com");
        driver.Status.Should().Be(DriverStatus.OnDuty);
        driver.CreatedAt.Should().Be(now);
        driver.UpdatedAt.Should().Be(now);
    }

    [Theory]
    [InlineData(DriverStatus.Available)]
    [InlineData(DriverStatus.OnDuty)]
    [InlineData(DriverStatus.OffDuty)]
    [InlineData(DriverStatus.OnBreak)]
    public void Driver_ShouldAcceptAllDriverStatuses(DriverStatus status)
    {
        // Arrange & Act
        var driver = new Driver { Status = status };

        // Assert
        driver.Status.Should().Be(status);
    }

    [Fact]
    public void Driver_ShouldAllowAddingTrips()
    {
        // Arrange
        var driver = new Driver();
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            DriverId = driver.Id
        };

        // Act
        driver.Trips.Add(trip);

        // Assert
        driver.Trips.Should().ContainSingle();
    }
}
