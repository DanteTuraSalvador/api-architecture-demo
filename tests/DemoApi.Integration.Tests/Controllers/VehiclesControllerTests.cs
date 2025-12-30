using System.Net;
using System.Net.Http.Json;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using DemoApi.Integration.Tests.Fixtures;
using FluentAssertions;

namespace DemoApi.Integration.Tests.Controllers;

public class VehiclesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VehiclesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region GET /api/vehicles Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithVehicles()
    {
        // Act
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicles = await response.Content.ReadFromJsonAsync<IEnumerable<Vehicle>>();
        vehicles.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_AfterCreating_ShouldReturnVehicles()
    {
        // Arrange - Create a vehicle to ensure data exists
        var newVehicle = new Vehicle { VehicleNumber = "SEED-TEST-001", LicensePlate = "SEED-1234" };
        await _client.PostAsJsonAsync("/api/vehicles", newVehicle);

        // Act
        var response = await _client.GetAsync("/api/vehicles");
        var vehicles = await response.Content.ReadFromJsonAsync<List<Vehicle>>();

        // Assert
        vehicles.Should().NotBeEmpty();
    }

    #endregion

    #region GET /api/vehicles/{id} Tests

    [Fact]
    public async Task GetById_WhenVehicleExists_ShouldReturnOk()
    {
        // Arrange - Create a vehicle first
        var newVehicle = new Vehicle { VehicleNumber = "GETBYID-001", LicensePlate = "GBI-1234" };
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", newVehicle);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<Vehicle>();

        // Act
        var response = await _client.GetAsync($"/api/vehicles/{createdVehicle!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicle = await response.Content.ReadFromJsonAsync<Vehicle>();
        vehicle.Should().NotBeNull();
        vehicle!.Id.Should().Be(createdVehicle.Id);
    }

    [Fact]
    public async Task GetById_WhenVehicleDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/vehicles/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/vehicles Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newVehicle = new Vehicle
        {
            VehicleNumber = "TEST-001",
            LicensePlate = "TEST-1234",
            Type = VehicleType.Van,
            Status = VehicleStatus.Available,
            FleetId = Guid.NewGuid(),
            Mileage = 10000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vehicles", newVehicle);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdVehicle = await response.Content.ReadFromJsonAsync<Vehicle>();
        createdVehicle.Should().NotBeNull();
        createdVehicle!.Id.Should().NotBe(Guid.Empty);
        createdVehicle.VehicleNumber.Should().Be("TEST-001");
    }

    [Fact]
    public async Task Create_ShouldReturnLocationHeader()
    {
        // Arrange
        var newVehicle = new Vehicle
        {
            VehicleNumber = "TEST-002",
            LicensePlate = "TEST-5678",
            Type = VehicleType.Truck
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vehicles", newVehicle);

        // Assert
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().ToLower().Should().Contain("/api/vehicles/");
    }

    #endregion

    #region PUT /api/vehicles/{id} Tests

    [Fact]
    public async Task Update_WhenVehicleExists_ShouldReturnOk()
    {
        // Arrange - Create a vehicle first
        var newVehicle = new Vehicle
        {
            VehicleNumber = "UPDATE-001",
            LicensePlate = "UPD-1234",
            Type = VehicleType.Car
        };
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", newVehicle);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<Vehicle>();

        // Update data
        var updateData = new Vehicle
        {
            VehicleNumber = "UPDATE-001-MODIFIED",
            LicensePlate = "UPD-9999",
            Type = VehicleType.Van,
            Status = VehicleStatus.InTransit,
            Mileage = 25000
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/vehicles/{createdVehicle!.Id}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedVehicle = await response.Content.ReadFromJsonAsync<Vehicle>();
        updatedVehicle!.VehicleNumber.Should().Be("UPDATE-001-MODIFIED");
        updatedVehicle.Mileage.Should().Be(25000);
    }

    [Fact]
    public async Task Update_WhenVehicleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var updateData = new Vehicle { VehicleNumber = "NONEXISTENT" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/vehicles/{Guid.NewGuid()}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/vehicles/{id} Tests

    [Fact]
    public async Task Delete_WhenVehicleExists_ShouldReturnNoContent()
    {
        // Arrange - Create a vehicle first
        var newVehicle = new Vehicle
        {
            VehicleNumber = "DELETE-001",
            LicensePlate = "DEL-1234"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", newVehicle);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<Vehicle>();

        // Act
        var response = await _client.DeleteAsync($"/api/vehicles/{createdVehicle!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenVehicleDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/vehicles/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/vehicles/status/{status} Tests

    [Fact]
    public async Task GetByStatus_WithValidStatus_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/vehicles/status/Available");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicles = await response.Content.ReadFromJsonAsync<List<Vehicle>>();
        vehicles.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Available")]
    [InlineData("InTransit")]
    [InlineData("Maintenance")]
    [InlineData("OutOfService")]
    public async Task GetByStatus_WithAllValidStatuses_ShouldReturnOk(string status)
    {
        // Act
        var response = await _client.GetAsync($"/api/vehicles/status/{status}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
