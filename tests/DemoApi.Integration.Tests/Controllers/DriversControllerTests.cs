using System.Net;
using System.Net.Http.Json;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using DemoApi.Integration.Tests.Fixtures;
using FluentAssertions;

namespace DemoApi.Integration.Tests.Controllers;

public class DriversControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DriversControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region GET /api/drivers Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithDrivers()
    {
        // Act
        var response = await _client.GetAsync("/api/drivers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var drivers = await response.Content.ReadFromJsonAsync<IEnumerable<Driver>>();
        drivers.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_AfterCreating_ShouldReturnDrivers()
    {
        // Arrange - Create a driver to ensure data exists
        var newDriver = new Driver { FirstName = "Seed", LastName = "Test" };
        await _client.PostAsJsonAsync("/api/drivers", newDriver);

        // Act
        var response = await _client.GetAsync("/api/drivers");
        var drivers = await response.Content.ReadFromJsonAsync<List<Driver>>();

        // Assert
        drivers.Should().NotBeEmpty();
    }

    #endregion

    #region GET /api/drivers/{id} Tests

    [Fact]
    public async Task GetById_WhenDriverExists_ShouldReturnOk()
    {
        // Arrange - Create a driver first
        var newDriver = new Driver { FirstName = "GetById", LastName = "Test" };
        var createResponse = await _client.PostAsJsonAsync("/api/drivers", newDriver);
        var createdDriver = await createResponse.Content.ReadFromJsonAsync<Driver>();

        // Act
        var response = await _client.GetAsync($"/api/drivers/{createdDriver!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var driver = await response.Content.ReadFromJsonAsync<Driver>();
        driver.Should().NotBeNull();
        driver!.Id.Should().Be(createdDriver.Id);
    }

    [Fact]
    public async Task GetById_WhenDriverDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/drivers/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/drivers Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newDriver = new Driver
        {
            FirstName = "Test",
            LastName = "Driver",
            LicenseNumber = "DL-TEST-001",
            PhoneNumber = "+1-555-TEST",
            Email = "test.driver@example.com",
            Status = DriverStatus.Available
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/drivers", newDriver);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdDriver = await response.Content.ReadFromJsonAsync<Driver>();
        createdDriver.Should().NotBeNull();
        createdDriver!.Id.Should().NotBe(Guid.Empty);
        createdDriver.FirstName.Should().Be("Test");
        createdDriver.LastName.Should().Be("Driver");
    }

    [Fact]
    public async Task Create_ShouldReturnLocationHeader()
    {
        // Arrange
        var newDriver = new Driver
        {
            FirstName = "Location",
            LastName = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/drivers", newDriver);

        // Assert
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().ToLower().Should().Contain("/api/drivers/");
    }

    #endregion

    #region PUT /api/drivers/{id} Tests

    [Fact]
    public async Task Update_WhenDriverExists_ShouldReturnOk()
    {
        // Arrange - Create a driver first
        var newDriver = new Driver
        {
            FirstName = "Original",
            LastName = "Name"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/drivers", newDriver);
        var createdDriver = await createResponse.Content.ReadFromJsonAsync<Driver>();

        var updateData = new Driver
        {
            FirstName = "Updated",
            LastName = "Driver",
            LicenseNumber = "DL-UPDATED",
            Status = DriverStatus.OnDuty
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/drivers/{createdDriver!.Id}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedDriver = await response.Content.ReadFromJsonAsync<Driver>();
        updatedDriver!.FirstName.Should().Be("Updated");
        updatedDriver.Status.Should().Be(DriverStatus.OnDuty);
    }

    [Fact]
    public async Task Update_WhenDriverDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PutAsJsonAsync($"/api/drivers/{Guid.NewGuid()}", new Driver());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/drivers/{id} Tests

    [Fact]
    public async Task Delete_WhenDriverExists_ShouldReturnNoContent()
    {
        // Arrange
        var newDriver = new Driver { FirstName = "Delete", LastName = "Me" };
        var createResponse = await _client.PostAsJsonAsync("/api/drivers", newDriver);
        var createdDriver = await createResponse.Content.ReadFromJsonAsync<Driver>();

        // Act
        var response = await _client.DeleteAsync($"/api/drivers/{createdDriver!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenDriverDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/drivers/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/drivers/available Tests

    [Fact]
    public async Task GetAvailable_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/drivers/available");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var drivers = await response.Content.ReadFromJsonAsync<List<Driver>>();
        drivers.Should().NotBeNull();
    }

    #endregion

    #region GET /api/drivers/status/{status} Tests

    [Theory]
    [InlineData("Available")]
    [InlineData("OnDuty")]
    [InlineData("OffDuty")]
    [InlineData("OnBreak")]
    public async Task GetByStatus_WithAllValidStatuses_ShouldReturnOk(string status)
    {
        // Act
        var response = await _client.GetAsync($"/api/drivers/status/{status}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
