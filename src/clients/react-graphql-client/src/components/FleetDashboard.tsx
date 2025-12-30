import { useQuery } from '@apollo/client'
import { GET_VEHICLES, GET_DRIVERS, GET_ACTIVE_TRIPS, GET_RECENT_ALERTS, GET_FLEETS } from '../graphql/queries'
import { Vehicle, Driver, Trip, Alert, Fleet, VehicleStatus, DriverStatus } from '../types'

interface VehiclesData {
  vehicles: Vehicle[]
}

interface DriversData {
  drivers: Driver[]
}

interface TripsData {
  activeTrips: Trip[]
}

interface AlertsData {
  recentAlerts: Alert[]
}

interface FleetsData {
  fleets: Fleet[]
}

export function FleetDashboard() {
  const { data: vehicleData, loading: vehicleLoading } = useQuery<VehiclesData>(GET_VEHICLES)
  const { data: driverData, loading: driverLoading } = useQuery<DriversData>(GET_DRIVERS)
  const { data: tripData, loading: tripLoading } = useQuery<TripsData>(GET_ACTIVE_TRIPS)
  const { data: alertData, loading: alertLoading } = useQuery<AlertsData>(GET_RECENT_ALERTS, {
    variables: { count: 5 },
  })
  const { data: fleetData, loading: fleetLoading } = useQuery<FleetsData>(GET_FLEETS)

  const vehicles = vehicleData?.vehicles || []
  const drivers = driverData?.drivers || []
  const activeTrips = tripData?.activeTrips || []
  const recentAlerts = alertData?.recentAlerts || []
  const fleets = fleetData?.fleets || []

  const vehicleStats = {
    total: vehicles.length,
    available: vehicles.filter((v) => v.status === VehicleStatus.AVAILABLE).length,
    inTransit: vehicles.filter((v) => v.status === VehicleStatus.IN_TRANSIT).length,
    maintenance: vehicles.filter((v) => v.status === VehicleStatus.MAINTENANCE).length,
  }

  const driverStats = {
    total: drivers.length,
    available: drivers.filter((d) => d.status === DriverStatus.AVAILABLE).length,
    onDuty: drivers.filter((d) => d.status === DriverStatus.ON_DUTY).length,
  }

  const isLoading = vehicleLoading || driverLoading || tripLoading || alertLoading || fleetLoading

  return (
    <div className="dashboard">
      <h1>Fleet Dashboard</h1>
      <p className="subtitle">Real-time fleet overview using GraphQL queries</p>

      {isLoading ? (
        <div className="loading">Loading dashboard data...</div>
      ) : (
        <>
          <div className="stats-grid">
            <div className="stat-card">
              <h3>Vehicles</h3>
              <div className="stat-number">{vehicleStats.total}</div>
              <div className="stat-details">
                <span className="stat-success">{vehicleStats.available} Available</span>
                <span className="stat-info">{vehicleStats.inTransit} In Transit</span>
                <span className="stat-warning">{vehicleStats.maintenance} Maintenance</span>
              </div>
            </div>

            <div className="stat-card">
              <h3>Drivers</h3>
              <div className="stat-number">{driverStats.total}</div>
              <div className="stat-details">
                <span className="stat-success">{driverStats.available} Available</span>
                <span className="stat-info">{driverStats.onDuty} On Duty</span>
              </div>
            </div>

            <div className="stat-card">
              <h3>Active Trips</h3>
              <div className="stat-number">{activeTrips.length}</div>
              <div className="stat-details">
                <span>Currently in progress</span>
              </div>
            </div>

            <div className="stat-card">
              <h3>Fleets</h3>
              <div className="stat-number">{fleets.length}</div>
              <div className="stat-details">
                <span>Managed fleets</span>
              </div>
            </div>
          </div>

          <div className="dashboard-panels">
            <div className="panel">
              <h3>Active Trips</h3>
              {activeTrips.length > 0 ? (
                <ul className="trip-list">
                  {activeTrips.map((trip) => (
                    <li key={trip.id}>
                      <span className="trip-id">{trip.id.substring(0, 8)}...</span>
                      <span className="trip-time">
                        Started: {new Date(trip.startTime).toLocaleTimeString()}
                      </span>
                      <span className="trip-distance">{trip.distance} km</span>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="empty-state">No active trips</p>
              )}
            </div>

            <div className="panel">
              <h3>Recent Alerts</h3>
              {recentAlerts.length > 0 ? (
                <ul className="alert-list-mini">
                  {recentAlerts.map((alert) => (
                    <li key={alert.id} className={`alert-item ${alert.level.toLowerCase()}`}>
                      <span className="alert-type">{alert.type}</span>
                      <span className="alert-message">{alert.message}</span>
                      {!alert.isAcknowledged && (
                        <span className="unread-badge">NEW</span>
                      )}
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="empty-state">No recent alerts</p>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  )
}
