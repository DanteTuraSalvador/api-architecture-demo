using System.Net;
using System.Net.Http.Json;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using DemoApi.Integration.Tests.Fixtures;
using FluentAssertions;

namespace DemoApi.Integration.Tests.Controllers;

public class AlertsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region GET /api/alerts Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAlerts()
    {
        // Act
        var response = await _client.GetAsync("/api/alerts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var alerts = await response.Content.ReadFromJsonAsync<IEnumerable<Alert>>();
        alerts.Should().NotBeNull();
    }

    #endregion

    #region GET /api/alerts/{id} Tests

    [Fact]
    public async Task GetById_WhenAlertDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/alerts/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/alerts Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newAlert = new Alert
        {
            VehicleId = Guid.NewGuid(),
            Type = AlertType.Speeding,
            Level = AlertLevel.Warning,
            Message = "Vehicle exceeding speed limit"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/alerts", newAlert);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAlert = await response.Content.ReadFromJsonAsync<Alert>();
        createdAlert.Should().NotBeNull();
        createdAlert!.Id.Should().NotBe(Guid.Empty);
        createdAlert.Message.Should().Be("Vehicle exceeding speed limit");
        createdAlert.IsAcknowledged.Should().BeFalse();
    }

    [Fact]
    public async Task Create_ShouldSetIsAcknowledgedToFalse()
    {
        // Arrange
        var newAlert = new Alert
        {
            Type = AlertType.LowFuel,
            Level = AlertLevel.Info,
            Message = "Fuel level below 25%",
            IsAcknowledged = true // Try to set it true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/alerts", newAlert);
        var createdAlert = await response.Content.ReadFromJsonAsync<Alert>();

        // Assert
        createdAlert!.IsAcknowledged.Should().BeFalse(); // Should be overridden to false
    }

    #endregion

    #region DELETE /api/alerts/{id} Tests

    [Fact]
    public async Task Delete_WhenAlertExists_ShouldReturnNoContent()
    {
        // Arrange
        var newAlert = new Alert
        {
            Type = AlertType.EngineWarning,
            Level = AlertLevel.Critical,
            Message = "Delete test alert"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/alerts", newAlert);
        var createdAlert = await createResponse.Content.ReadFromJsonAsync<Alert>();

        // Act
        var response = await _client.DeleteAsync($"/api/alerts/{createdAlert!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenAlertDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/alerts/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/alerts/unacknowledged Tests

    [Fact]
    public async Task GetUnacknowledged_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/alerts/unacknowledged");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/alerts/recent Tests

    [Fact]
    public async Task GetRecent_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/alerts/recent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRecent_WithCountParameter_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/alerts/recent?count=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /api/alerts/{id}/acknowledge Tests

    [Fact]
    public async Task Acknowledge_WhenAlertExists_ShouldReturnOk()
    {
        // Arrange
        var newAlert = new Alert
        {
            Type = AlertType.HarshBraking,
            Level = AlertLevel.Warning,
            Message = "Harsh braking detected"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/alerts", newAlert);
        var createdAlert = await createResponse.Content.ReadFromJsonAsync<Alert>();

        // Act
        var response = await _client.PostAsync($"/api/alerts/{createdAlert!.Id}/acknowledge", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var acknowledgedAlert = await response.Content.ReadFromJsonAsync<Alert>();
        acknowledgedAlert!.IsAcknowledged.Should().BeTrue();
        acknowledgedAlert.AcknowledgedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Acknowledge_WhenAlertDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PostAsync($"/api/alerts/{Guid.NewGuid()}/acknowledge", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/alerts/vehicle/{vehicleId} Tests

    [Fact]
    public async Task GetByVehicle_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync($"/api/alerts/vehicle/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
