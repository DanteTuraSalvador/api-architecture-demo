using DemoApi.Domain.Entities;

namespace DemoApi.Gateway.GraphQL;

/// <summary>
/// GraphQL Subscription type - demonstrates real-time data updates
/// </summary>
public class Subscription
{
    /// <summary>
    /// Subscribe to new alerts
    /// </summary>
    [Subscribe]
    [Topic("AlertCreated")]
    public Alert OnAlertCreated([EventMessage] Alert alert) => alert;

    /// <summary>
    /// Subscribe to vehicle status changes
    /// </summary>
    [Subscribe]
    [Topic("VehicleStatusChanged")]
    public Vehicle OnVehicleStatusChanged([EventMessage] Vehicle vehicle) => vehicle;

    /// <summary>
    /// Subscribe to trip updates
    /// </summary>
    [Subscribe]
    [Topic("TripUpdated")]
    public Trip OnTripUpdated([EventMessage] Trip trip) => trip;

    /// <summary>
    /// Subscribe to delivery updates
    /// </summary>
    [Subscribe]
    [Topic("DeliveryUpdated")]
    public Delivery OnDeliveryUpdated([EventMessage] Delivery delivery) => delivery;
}
