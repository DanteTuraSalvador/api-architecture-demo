using DemoApi.Domain.Entities;
using FluentAssertions;

namespace DemoApi.Domain.Tests.Entities;

public class LocationTests
{
    [Fact]
    public void Location_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var location = new Location();

        // Assert
        location.Latitude.Should().Be(0);
        location.Longitude.Should().Be(0);
        location.Address.Should().BeEmpty();
    }

    [Fact]
    public void Location_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var location = new Location
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            Address = "New York, NY",
            Timestamp = DateTime.UtcNow
        };

        // Assert
        location.Latitude.Should().Be(40.7128);
        location.Longitude.Should().Be(-74.0060);
        location.Address.Should().Be("New York, NY");
        location.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(51.5074, -0.1278)] // London
    [InlineData(35.6762, 139.6503)] // Tokyo
    [InlineData(-33.8688, 151.2093)] // Sydney
    public void Location_ShouldAcceptVariousCoordinates(double latitude, double longitude)
    {
        // Arrange & Act
        var location = new Location
        {
            Latitude = latitude,
            Longitude = longitude
        };

        // Assert
        location.Latitude.Should().Be(latitude);
        location.Longitude.Should().Be(longitude);
    }

    [Fact]
    public void Location_ShouldHandlePreciseCoordinates()
    {
        // Arrange
        var preciseLatitude = 40.712776123456789;
        var preciseLongitude = -74.005974987654321;

        // Act
        var location = new Location
        {
            Latitude = preciseLatitude,
            Longitude = preciseLongitude
        };

        // Assert
        location.Latitude.Should().BeApproximately(40.712776123456789, 0.000000001);
        location.Longitude.Should().BeApproximately(-74.005974987654321, 0.000000001);
    }
}
