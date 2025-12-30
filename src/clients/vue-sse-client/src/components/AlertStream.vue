<script setup>
import { computed } from 'vue'
import { useSSE } from '../composables/useSSE'

const props = defineProps({
  baseUrl: {
    type: String,
    default: 'https://localhost:5001'
  }
})

const {
  isConnected,
  connectionState,
  clientId,
  error,
  alerts,
  connect,
  disconnect,
  clearAlerts
} = useSSE(props.baseUrl)

const unacknowledgedAlerts = computed(() =>
  alerts.value.filter(a => !a.isAcknowledged)
)

const getAlertLevelClass = (level) => {
  switch (level) {
    case 'Critical':
    case 2:
      return 'alert-critical'
    case 'Warning':
    case 1:
      return 'alert-warning'
    default:
      return 'alert-info'
  }
}

const getAlertTypeName = (type) => {
  const types = ['Speeding', 'HarshBraking', 'LowFuel', 'EngineWarning', 'GeofenceViolation']
  return typeof type === 'number' ? types[type] || type : type
}

const formatTime = (timestamp) => {
  return new Date(timestamp).toLocaleString()
}
</script>

<template>
  <div class="alert-stream">
    <div class="stream-header">
      <h2>Real-time Alert Stream</h2>
      <div class="connection-status">
        <span :class="['status-badge', isConnected ? 'connected' : 'disconnected']">
          {{ connectionState }}
        </span>
        <span v-if="clientId" class="client-id">
          Client: {{ clientId.substring(0, 8) }}...
        </span>
      </div>
    </div>

    <div class="stream-controls">
      <button v-if="!isConnected" @click="connect" class="btn btn-primary">
        Connect to Stream
      </button>
      <button v-else @click="disconnect" class="btn btn-secondary">
        Disconnect
      </button>
      <button @click="clearAlerts" class="btn btn-outline" :disabled="alerts.length === 0">
        Clear Alerts ({{ alerts.length }})
      </button>
    </div>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div class="stats-bar">
      <div class="stat">
        <span class="stat-label">Total Alerts</span>
        <span class="stat-value">{{ alerts.length }}</span>
      </div>
      <div class="stat">
        <span class="stat-label">Unacknowledged</span>
        <span class="stat-value warning">{{ unacknowledgedAlerts.length }}</span>
      </div>
    </div>

    <div class="alerts-container">
      <TransitionGroup name="alert-list">
        <div
          v-for="alert in alerts"
          :key="alert.id"
          :class="['alert-card', getAlertLevelClass(alert.level)]"
        >
          <div class="alert-header">
            <span class="alert-type">{{ getAlertTypeName(alert.type) }}</span>
            <span class="alert-level">{{ alert.level }}</span>
          </div>
          <p class="alert-message">{{ alert.message }}</p>
          <div class="alert-footer">
            <span class="alert-time">{{ formatTime(alert.createdAt) }}</span>
            <span v-if="alert.isAcknowledged" class="acknowledged">
              Acknowledged
            </span>
            <span v-else class="new-badge">NEW</span>
          </div>
          <div class="alert-vehicle">
            Vehicle: {{ alert.vehicleId.substring(0, 8) }}...
          </div>
        </div>
      </TransitionGroup>

      <div v-if="alerts.length === 0 && isConnected" class="empty-state">
        <p>Waiting for alerts...</p>
        <p class="hint">Trigger test alerts using the form below</p>
      </div>

      <div v-if="!isConnected" class="empty-state">
        <p>Connect to start receiving real-time alerts</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.alert-stream {
  max-width: 800px;
  margin: 0 auto;
}

.stream-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.stream-header h2 {
  margin: 0;
  color: #333;
}

.connection-status {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.status-badge {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.875rem;
}

.status-badge.connected {
  background: #dcfce7;
  color: #166534;
}

.status-badge.disconnected {
  background: #f3f4f6;
  color: #6b7280;
}

.client-id {
  color: #6b7280;
  font-size: 0.875rem;
}

.stream-controls {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #10b981;
  color: white;
}

.btn-primary:hover {
  background: #059669;
}

.btn-secondary {
  background: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background: #4b5563;
}

.btn-outline {
  background: transparent;
  border: 1px solid #d1d5db;
  color: #374151;
}

.btn-outline:hover:not(:disabled) {
  background: #f3f4f6;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.error-message {
  background: #fee2e2;
  color: #991b1b;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  margin-bottom: 1rem;
}

.stats-bar {
  display: flex;
  gap: 2rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}

.stat {
  display: flex;
  flex-direction: column;
}

.stat-label {
  font-size: 0.75rem;
  color: #6b7280;
  text-transform: uppercase;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #333;
}

.stat-value.warning {
  color: #f59e0b;
}

.alerts-container {
  max-height: 500px;
  overflow-y: auto;
}

.alert-card {
  background: white;
  border-radius: 8px;
  padding: 1rem;
  margin-bottom: 0.75rem;
  border-left: 4px solid #d1d5db;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.alert-card.alert-critical {
  border-left-color: #ef4444;
  background: #fef2f2;
}

.alert-card.alert-warning {
  border-left-color: #f59e0b;
  background: #fffbeb;
}

.alert-card.alert-info {
  border-left-color: #3b82f6;
  background: #eff6ff;
}

.alert-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.alert-type {
  font-weight: 600;
  color: #333;
}

.alert-level {
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  background: rgba(0, 0, 0, 0.1);
}

.alert-message {
  margin: 0.5rem 0;
  color: #4b5563;
}

.alert-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.75rem;
}

.alert-time {
  color: #9ca3af;
}

.acknowledged {
  color: #10b981;
}

.new-badge {
  background: #ef4444;
  color: white;
  padding: 0.125rem 0.5rem;
  border-radius: 4px;
  font-weight: 600;
}

.alert-vehicle {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-top: 0.5rem;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.empty-state .hint {
  font-size: 0.875rem;
  color: #9ca3af;
}

/* Transition animations */
.alert-list-enter-active {
  transition: all 0.3s ease;
}

.alert-list-leave-active {
  transition: all 0.3s ease;
}

.alert-list-enter-from {
  opacity: 0;
  transform: translateX(-30px);
}

.alert-list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style>
