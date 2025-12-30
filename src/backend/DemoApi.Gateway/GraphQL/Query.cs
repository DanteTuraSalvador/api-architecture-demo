using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;

namespace DemoApi.Gateway.GraphQL;

/// <summary>
/// GraphQL Query type - demonstrates complex nested data queries
/// </summary>
public class Query
{
    /// <summary>
    /// Get all vehicles with optional filtering
    /// </summary>
    public async Task<IEnumerable<Vehicle>> GetVehicles(
        [Service] IVehicleService vehicleService,
        string? status = null)
    {
        if (!string.IsNullOrEmpty(status))
            return await vehicleService.GetByStatusAsync(status);

        return await vehicleService.GetAllAsync();
    }

    /// <summary>
    /// Get a vehicle by ID
    /// </summary>
    public async Task<Vehicle?> GetVehicle(
        [Service] IVehicleService vehicleService,
        Guid id)
    {
        return await vehicleService.GetByIdAsync(id);
    }

    /// <summary>
    /// Get all drivers with optional filtering
    /// </summary>
    public async Task<IEnumerable<Driver>> GetDrivers(
        [Service] IDriverService driverService,
        string? status = null)
    {
        if (!string.IsNullOrEmpty(status))
            return await driverService.GetByStatusAsync(status);

        return await driverService.GetAllAsync();
    }

    /// <summary>
    /// Get a driver by ID
    /// </summary>
    public async Task<Driver?> GetDriver(
        [Service] IDriverService driverService,
        Guid id)
    {
        return await driverService.GetByIdAsync(id);
    }

    /// <summary>
    /// Get available drivers
    /// </summary>
    public async Task<IEnumerable<Driver>> GetAvailableDrivers(
        [Service] IDriverService driverService)
    {
        return await driverService.GetAvailableDriversAsync();
    }

    /// <summary>
    /// Get all trips
    /// </summary>
    public async Task<IEnumerable<Trip>> GetTrips(
        [Service] ITripService tripService)
    {
        return await tripService.GetAllAsync();
    }

    /// <summary>
    /// Get a trip by ID
    /// </summary>
    public async Task<Trip?> GetTrip(
        [Service] ITripService tripService,
        Guid id)
    {
        return await tripService.GetByIdAsync(id);
    }

    /// <summary>
    /// Get active trips
    /// </summary>
    public async Task<IEnumerable<Trip>> GetActiveTrips(
        [Service] ITripService tripService)
    {
        return await tripService.GetActiveTripsAsync();
    }

    /// <summary>
    /// Get all deliveries
    /// </summary>
    public async Task<IEnumerable<Delivery>> GetDeliveries(
        [Service] IDeliveryService deliveryService)
    {
        return await deliveryService.GetAllAsync();
    }

    /// <summary>
    /// Get a delivery by tracking number
    /// </summary>
    public async Task<Delivery?> GetDeliveryByTracking(
        [Service] IDeliveryService deliveryService,
        string trackingNumber)
    {
        return await deliveryService.GetByTrackingNumberAsync(trackingNumber);
    }

    /// <summary>
    /// Get all alerts
    /// </summary>
    public async Task<IEnumerable<Alert>> GetAlerts(
        [Service] IAlertService alertService,
        bool? unacknowledgedOnly = null)
    {
        if (unacknowledgedOnly == true)
            return await alertService.GetUnacknowledgedAsync();

        return await alertService.GetAllAsync();
    }

    /// <summary>
    /// Get recent alerts
    /// </summary>
    public async Task<IEnumerable<Alert>> GetRecentAlerts(
        [Service] IAlertService alertService,
        int count = 10)
    {
        return await alertService.GetRecentAlertsAsync(count);
    }

    /// <summary>
    /// Get all fleets
    /// </summary>
    public async Task<IEnumerable<Fleet>> GetFleets(
        [Service] IFleetService fleetService)
    {
        return await fleetService.GetAllAsync();
    }

    /// <summary>
    /// Get a fleet by ID with its vehicles
    /// </summary>
    public async Task<Fleet?> GetFleet(
        [Service] IFleetService fleetService,
        Guid id)
    {
        return await fleetService.GetWithVehiclesAsync(id);
    }
}
