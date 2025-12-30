import { useQuery, useMutation } from '@apollo/client'
import { GET_ALERTS, GET_RECENT_ALERTS } from '../graphql/queries'
import { ACKNOWLEDGE_ALERT } from '../graphql/mutations'
import { Alert, AlertLevel } from '../types'
import { useState } from 'react'

interface AlertsData {
  alerts: Alert[]
}

interface RecentAlertsData {
  recentAlerts: Alert[]
}

export function AlertList() {
  const [showUnacknowledgedOnly, setShowUnacknowledgedOnly] = useState(false)
  const [recentCount, setRecentCount] = useState(10)
  const [viewMode, setViewMode] = useState<'all' | 'recent'>('all')

  const { loading: loadingAll, error: errorAll, data: dataAll, refetch: refetchAll } =
    useQuery<AlertsData>(GET_ALERTS, {
      variables: { unacknowledgedOnly: showUnacknowledgedOnly },
      skip: viewMode !== 'all',
    })

  const { loading: loadingRecent, error: errorRecent, data: dataRecent, refetch: refetchRecent } =
    useQuery<RecentAlertsData>(GET_RECENT_ALERTS, {
      variables: { count: recentCount },
      skip: viewMode !== 'recent',
    })

  const [acknowledgeAlert] = useMutation(ACKNOWLEDGE_ALERT, {
    onCompleted: () => {
      viewMode === 'all' ? refetchAll() : refetchRecent()
    },
  })

  const handleAcknowledge = async (id: string) => {
    await acknowledgeAlert({ variables: { id } })
  }

  const getAlertLevelClass = (level: AlertLevel) => {
    switch (level) {
      case AlertLevel.INFO:
        return 'alert-info'
      case AlertLevel.WARNING:
        return 'alert-warning'
      case AlertLevel.CRITICAL:
        return 'alert-critical'
      default:
        return ''
    }
  }

  const loading = viewMode === 'all' ? loadingAll : loadingRecent
  const error = viewMode === 'all' ? errorAll : errorRecent
  const alerts = viewMode === 'all' ? dataAll?.alerts : dataRecent?.recentAlerts

  if (loading) return <div className="loading">Loading alerts...</div>
  if (error) return <div className="error">Error: {error.message}</div>

  return (
    <div className="alert-list">
      <div className="filters">
        <div className="view-toggle">
          <button
            className={`btn ${viewMode === 'all' ? 'btn-primary' : 'btn-secondary'}`}
            onClick={() => setViewMode('all')}
          >
            All Alerts
          </button>
          <button
            className={`btn ${viewMode === 'recent' ? 'btn-primary' : 'btn-secondary'}`}
            onClick={() => setViewMode('recent')}
          >
            Recent
          </button>
        </div>

        {viewMode === 'all' && (
          <label className="checkbox-label">
            <input
              type="checkbox"
              checked={showUnacknowledgedOnly}
              onChange={(e) => setShowUnacknowledgedOnly(e.target.checked)}
            />
            Unacknowledged Only
          </label>
        )}

        {viewMode === 'recent' && (
          <select
            value={recentCount}
            onChange={(e) => setRecentCount(parseInt(e.target.value))}
            className="filter-select"
          >
            <option value={5}>Last 5</option>
            <option value={10}>Last 10</option>
            <option value={25}>Last 25</option>
            <option value={50}>Last 50</option>
          </select>
        )}

        <button
          onClick={() => viewMode === 'all' ? refetchAll() : refetchRecent()}
          className="btn btn-secondary"
        >
          Refresh
        </button>
      </div>

      <div className="alerts-grid">
        {alerts?.map((alert) => (
          <div
            key={alert.id}
            className={`alert-card ${getAlertLevelClass(alert.level)} ${
              alert.isAcknowledged ? 'acknowledged' : ''
            }`}
          >
            <div className="alert-header">
              <span className="alert-type">{alert.type}</span>
              <span className={`alert-level badge ${
                alert.level === AlertLevel.CRITICAL ? 'badge-error' :
                alert.level === AlertLevel.WARNING ? 'badge-warning' : 'badge-info'
              }`}>
                {alert.level}
              </span>
            </div>
            <p className="alert-message">{alert.message}</p>
            <div className="alert-footer">
              <span className="alert-time">
                {new Date(alert.createdAt).toLocaleString()}
              </span>
              {!alert.isAcknowledged ? (
                <button
                  onClick={() => handleAcknowledge(alert.id)}
                  className="btn btn-sm btn-primary"
                >
                  Acknowledge
                </button>
              ) : (
                <span className="acknowledged-badge">
                  Acknowledged {alert.acknowledgedAt &&
                    `at ${new Date(alert.acknowledgedAt).toLocaleString()}`}
                </span>
              )}
            </div>
          </div>
        ))}
      </div>

      {alerts?.length === 0 && (
        <div className="empty-state">No alerts found.</div>
      )}
    </div>
  )
}
