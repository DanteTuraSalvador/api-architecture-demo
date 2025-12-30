using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Gateway.GraphQL;

/// <summary>
/// GraphQL Mutation type - demonstrates data modification operations
/// </summary>
public class Mutation
{
    // Vehicle mutations
    public async Task<Vehicle> CreateVehicle(
        [Service] IVehicleService vehicleService,
        CreateVehicleInput input)
    {
        var vehicle = new Vehicle
        {
            VehicleNumber = input.VehicleNumber,
            LicensePlate = input.LicensePlate,
            Type = input.Type,
            Status = VehicleStatus.Available,
            FleetId = input.FleetId,
            Mileage = input.Mileage,
            LastMaintenance = DateTime.UtcNow
        };
        return await vehicleService.CreateAsync(vehicle);
    }

    public async Task<Vehicle?> UpdateVehicleStatus(
        [Service] IVehicleService vehicleService,
        Guid id,
        VehicleStatus status)
    {
        var vehicle = await vehicleService.GetByIdAsync(id);
        if (vehicle == null) return null;

        vehicle.Status = status;
        return await vehicleService.UpdateAsync(id, vehicle);
    }

    public async Task<bool> DeleteVehicle(
        [Service] IVehicleService vehicleService,
        Guid id)
    {
        return await vehicleService.DeleteAsync(id);
    }

    // Driver mutations
    public async Task<Driver> CreateDriver(
        [Service] IDriverService driverService,
        CreateDriverInput input)
    {
        var driver = new Driver
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            LicenseNumber = input.LicenseNumber,
            PhoneNumber = input.PhoneNumber,
            Email = input.Email,
            Status = DriverStatus.Available
        };
        return await driverService.CreateAsync(driver);
    }

    public async Task<Driver?> UpdateDriverStatus(
        [Service] IDriverService driverService,
        Guid id,
        DriverStatus status)
    {
        var driver = await driverService.GetByIdAsync(id);
        if (driver == null) return null;

        driver.Status = status;
        return await driverService.UpdateAsync(id, driver);
    }

    public async Task<bool> DeleteDriver(
        [Service] IDriverService driverService,
        Guid id)
    {
        return await driverService.DeleteAsync(id);
    }

    // Trip mutations
    public async Task<Trip> CreateTrip(
        [Service] ITripService tripService,
        CreateTripInput input)
    {
        var trip = new Trip
        {
            VehicleId = input.VehicleId,
            DriverId = input.DriverId,
            StartTime = input.StartTime ?? DateTime.UtcNow,
            StartLocation = new DemoApi.Domain.Entities.Location
            {
                Latitude = input.StartLatitude,
                Longitude = input.StartLongitude,
                Address = input.StartAddress,
                Timestamp = DateTime.UtcNow
            }
        };
        return await tripService.CreateAsync(trip);
    }

    public async Task<Trip?> StartTrip(
        [Service] ITripService tripService,
        Guid id)
    {
        return await tripService.StartTripAsync(id);
    }

    public async Task<Trip?> CompleteTrip(
        [Service] ITripService tripService,
        Guid id)
    {
        return await tripService.CompleteTripAsync(id);
    }

    // Alert mutations
    public async Task<Alert> CreateAlert(
        [Service] IAlertService alertService,
        CreateAlertInput input)
    {
        var alert = new Alert
        {
            VehicleId = input.VehicleId,
            Type = input.Type,
            Level = input.Level,
            Message = input.Message
        };
        return await alertService.CreateAsync(alert);
    }

    public async Task<Alert?> AcknowledgeAlert(
        [Service] IAlertService alertService,
        Guid id)
    {
        return await alertService.AcknowledgeAsync(id);
    }

    // Delivery mutations
    public async Task<Delivery?> MarkDeliveryPickedUp(
        [Service] IDeliveryService deliveryService,
        Guid id)
    {
        return await deliveryService.MarkAsPickedUpAsync(id);
    }

    public async Task<Delivery?> MarkDeliveryComplete(
        [Service] IDeliveryService deliveryService,
        Guid id)
    {
        return await deliveryService.MarkAsDeliveredAsync(id);
    }
}

// Input types for mutations
public record CreateVehicleInput(
    string VehicleNumber,
    string LicensePlate,
    VehicleType Type,
    Guid FleetId,
    double Mileage = 0);

public record CreateDriverInput(
    string FirstName,
    string LastName,
    string LicenseNumber,
    string PhoneNumber,
    string Email);

public record CreateTripInput(
    Guid VehicleId,
    Guid DriverId,
    double StartLatitude,
    double StartLongitude,
    string StartAddress,
    DateTime? StartTime = null);

public record CreateAlertInput(
    Guid VehicleId,
    AlertType Type,
    AlertLevel Level,
    string Message);
