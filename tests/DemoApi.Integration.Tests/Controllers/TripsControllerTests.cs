using System.Net;
using System.Net.Http.Json;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using DemoApi.Integration.Tests.Fixtures;
using FluentAssertions;

namespace DemoApi.Integration.Tests.Controllers;

public class TripsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TripsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region GET /api/trips Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithTrips()
    {
        // Act
        var response = await _client.GetAsync("/api/trips");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var trips = await response.Content.ReadFromJsonAsync<IEnumerable<Trip>>();
        trips.Should().NotBeNull();
    }

    #endregion

    #region POST /api/trips Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var newTrip = new Trip
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartLocation = new Location
            {
                Latitude = 40.7128,
                Longitude = -74.0060,
                Address = "New York, NY"
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/trips", newTrip);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTrip = await response.Content.ReadFromJsonAsync<Trip>();
        createdTrip.Should().NotBeNull();
        createdTrip!.Status.Should().Be(TripStatus.Planned);
    }

    [Fact]
    public async Task Create_ShouldSetStatusToPlanned()
    {
        // Arrange
        var newTrip = new Trip
        {
            VehicleId = Guid.NewGuid(),
            Status = TripStatus.Completed // Try to set different status
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/trips", newTrip);
        var createdTrip = await response.Content.ReadFromJsonAsync<Trip>();

        // Assert
        createdTrip!.Status.Should().Be(TripStatus.Planned);
    }

    #endregion

    #region GET /api/trips/{id} Tests

    [Fact]
    public async Task GetById_WhenTripDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/trips/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PUT /api/trips/{id} Tests

    [Fact]
    public async Task Update_WhenTripExists_ShouldReturnOk()
    {
        // Arrange
        var newTrip = new Trip { VehicleId = Guid.NewGuid() };
        var createResponse = await _client.PostAsJsonAsync("/api/trips", newTrip);
        var createdTrip = await createResponse.Content.ReadFromJsonAsync<Trip>();

        var updateData = new Trip
        {
            VehicleId = Guid.NewGuid(),
            DistanceTraveled = 150.5,
            FuelConsumed = 25.0
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/trips/{createdTrip!.Id}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTrip = await response.Content.ReadFromJsonAsync<Trip>();
        updatedTrip!.DistanceTraveled.Should().Be(150.5);
        updatedTrip.FuelConsumed.Should().Be(25.0);
    }

    [Fact]
    public async Task Update_WhenTripDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PutAsJsonAsync($"/api/trips/{Guid.NewGuid()}", new Trip());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/trips/{id} Tests

    [Fact]
    public async Task Delete_WhenTripExists_ShouldReturnNoContent()
    {
        // Arrange
        var newTrip = new Trip { VehicleId = Guid.NewGuid() };
        var createResponse = await _client.PostAsJsonAsync("/api/trips", newTrip);
        var createdTrip = await createResponse.Content.ReadFromJsonAsync<Trip>();

        // Act
        var response = await _client.DeleteAsync($"/api/trips/{createdTrip!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenTripDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/trips/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/trips/active Tests

    [Fact]
    public async Task GetActive_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/trips/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region POST /api/trips/{id}/start Tests

    [Fact]
    public async Task Start_WhenTripExists_ShouldReturnOk()
    {
        // Arrange
        var newTrip = new Trip { VehicleId = Guid.NewGuid() };
        var createResponse = await _client.PostAsJsonAsync("/api/trips", newTrip);
        var createdTrip = await createResponse.Content.ReadFromJsonAsync<Trip>();

        // Act
        var response = await _client.PostAsync($"/api/trips/{createdTrip!.Id}/start", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var startedTrip = await response.Content.ReadFromJsonAsync<Trip>();
        startedTrip!.Status.Should().Be(TripStatus.InProgress);
    }

    [Fact]
    public async Task Start_WhenTripDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PostAsync($"/api/trips/{Guid.NewGuid()}/start", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/trips/{id}/complete Tests

    [Fact]
    public async Task Complete_WhenTripExists_ShouldReturnOk()
    {
        // Arrange
        var newTrip = new Trip { VehicleId = Guid.NewGuid() };
        var createResponse = await _client.PostAsJsonAsync("/api/trips", newTrip);
        var createdTrip = await createResponse.Content.ReadFromJsonAsync<Trip>();
        await _client.PostAsync($"/api/trips/{createdTrip!.Id}/start", null);

        // Act
        var response = await _client.PostAsync($"/api/trips/{createdTrip.Id}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var completedTrip = await response.Content.ReadFromJsonAsync<Trip>();
        completedTrip!.Status.Should().Be(TripStatus.Completed);
        completedTrip.EndTime.Should().NotBeNull();
    }

    [Fact]
    public async Task Complete_WhenTripDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PostAsync($"/api/trips/{Guid.NewGuid()}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/trips/vehicle/{vehicleId} Tests

    [Fact]
    public async Task GetByVehicle_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync($"/api/trips/vehicle/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GET /api/trips/driver/{driverId} Tests

    [Fact]
    public async Task GetByDriver_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync($"/api/trips/driver/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
