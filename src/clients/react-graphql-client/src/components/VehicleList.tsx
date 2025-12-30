import { useQuery, useMutation } from '@apollo/client'
import { GET_VEHICLES } from '../graphql/queries'
import { UPDATE_VEHICLE_STATUS, DELETE_VEHICLE } from '../graphql/mutations'
import { Vehicle, VehicleStatus } from '../types'
import { useState } from 'react'

interface VehiclesData {
  vehicles: Vehicle[]
}

export function VehicleList() {
  const [statusFilter, setStatusFilter] = useState<string>('')
  const { loading, error, data, refetch } = useQuery<VehiclesData>(GET_VEHICLES, {
    variables: statusFilter ? { status: statusFilter } : {},
  })

  const [updateStatus] = useMutation(UPDATE_VEHICLE_STATUS, {
    onCompleted: () => refetch(),
  })

  const [deleteVehicle] = useMutation(DELETE_VEHICLE, {
    onCompleted: () => refetch(),
  })

  const handleStatusChange = async (id: string, newStatus: VehicleStatus) => {
    await updateStatus({ variables: { id, status: newStatus } })
  }

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this vehicle?')) {
      await deleteVehicle({ variables: { id } })
    }
  }

  const getStatusBadgeClass = (status: VehicleStatus) => {
    switch (status) {
      case VehicleStatus.AVAILABLE:
        return 'badge-success'
      case VehicleStatus.IN_TRANSIT:
        return 'badge-info'
      case VehicleStatus.MAINTENANCE:
        return 'badge-warning'
      case VehicleStatus.OUT_OF_SERVICE:
        return 'badge-error'
      default:
        return 'badge-secondary'
    }
  }

  if (loading) return <div className="loading">Loading vehicles...</div>
  if (error) return <div className="error">Error: {error.message}</div>

  return (
    <div className="vehicle-list">
      <div className="filters">
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="filter-select"
        >
          <option value="">All Statuses</option>
          <option value="Available">Available</option>
          <option value="InTransit">In Transit</option>
          <option value="Maintenance">Maintenance</option>
          <option value="OutOfService">Out of Service</option>
        </select>
        <button onClick={() => refetch()} className="btn btn-secondary">
          Refresh
        </button>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>Vehicle Number</th>
            <th>License Plate</th>
            <th>Type</th>
            <th>Status</th>
            <th>Mileage</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data?.vehicles.map((vehicle) => (
            <tr key={vehicle.id}>
              <td><strong>{vehicle.vehicleNumber}</strong></td>
              <td>{vehicle.licensePlate}</td>
              <td>{vehicle.type}</td>
              <td>
                <span className={`badge ${getStatusBadgeClass(vehicle.status)}`}>
                  {vehicle.status}
                </span>
              </td>
              <td>{vehicle.mileage.toLocaleString()} km</td>
              <td>
                <select
                  value={vehicle.status}
                  onChange={(e) => handleStatusChange(vehicle.id, e.target.value as VehicleStatus)}
                  className="status-select"
                >
                  {Object.values(VehicleStatus).map((status) => (
                    <option key={status} value={status}>
                      {status}
                    </option>
                  ))}
                </select>
                <button
                  onClick={() => handleDelete(vehicle.id)}
                  className="btn btn-danger btn-sm"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {data?.vehicles.length === 0 && (
        <div className="empty-state">No vehicles found.</div>
      )}
    </div>
  )
}
