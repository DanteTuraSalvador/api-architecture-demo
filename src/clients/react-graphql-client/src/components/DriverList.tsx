import { useQuery, useMutation } from '@apollo/client'
import { GET_DRIVERS } from '../graphql/queries'
import { UPDATE_DRIVER_STATUS, DELETE_DRIVER } from '../graphql/mutations'
import { Driver, DriverStatus } from '../types'
import { useState } from 'react'

interface DriversData {
  drivers: Driver[]
}

export function DriverList() {
  const [statusFilter, setStatusFilter] = useState<string>('')
  const { loading, error, data, refetch } = useQuery<DriversData>(GET_DRIVERS, {
    variables: statusFilter ? { status: statusFilter } : {},
  })

  const [updateStatus] = useMutation(UPDATE_DRIVER_STATUS, {
    onCompleted: () => refetch(),
  })

  const [deleteDriver] = useMutation(DELETE_DRIVER, {
    onCompleted: () => refetch(),
  })

  const handleStatusChange = async (id: string, newStatus: DriverStatus) => {
    await updateStatus({ variables: { id, status: newStatus } })
  }

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this driver?')) {
      await deleteDriver({ variables: { id } })
    }
  }

  const getStatusBadgeClass = (status: DriverStatus) => {
    switch (status) {
      case DriverStatus.AVAILABLE:
        return 'badge-success'
      case DriverStatus.ON_DUTY:
        return 'badge-info'
      case DriverStatus.OFF_DUTY:
        return 'badge-secondary'
      case DriverStatus.ON_BREAK:
        return 'badge-warning'
      default:
        return 'badge-secondary'
    }
  }

  if (loading) return <div className="loading">Loading drivers...</div>
  if (error) return <div className="error">Error: {error.message}</div>

  return (
    <div className="driver-list">
      <div className="filters">
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="filter-select"
        >
          <option value="">All Statuses</option>
          <option value="Available">Available</option>
          <option value="OnDuty">On Duty</option>
          <option value="OffDuty">Off Duty</option>
          <option value="OnBreak">On Break</option>
        </select>
        <button onClick={() => refetch()} className="btn btn-secondary">
          Refresh
        </button>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>License Number</th>
            <th>Phone</th>
            <th>Email</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data?.drivers.map((driver) => (
            <tr key={driver.id}>
              <td>
                <strong>{driver.firstName} {driver.lastName}</strong>
              </td>
              <td>{driver.licenseNumber}</td>
              <td>{driver.phoneNumber}</td>
              <td>{driver.email}</td>
              <td>
                <span className={`badge ${getStatusBadgeClass(driver.status)}`}>
                  {driver.status}
                </span>
              </td>
              <td>
                <select
                  value={driver.status}
                  onChange={(e) => handleStatusChange(driver.id, e.target.value as DriverStatus)}
                  className="status-select"
                >
                  {Object.values(DriverStatus).map((status) => (
                    <option key={status} value={status}>
                      {status}
                    </option>
                  ))}
                </select>
                <button
                  onClick={() => handleDelete(driver.id)}
                  className="btn btn-danger btn-sm"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {data?.drivers.length === 0 && (
        <div className="empty-state">No drivers found.</div>
      )}
    </div>
  )
}
