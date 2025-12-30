import { ref, onUnmounted } from 'vue'

export function useSSE(baseUrl = 'https://localhost:5001') {
  const isConnected = ref(false)
  const connectionState = ref('Disconnected')
  const clientId = ref(null)
  const error = ref(null)
  let eventSource = null

  const alerts = ref([])
  const listeners = new Map()

  const connect = () => {
    if (eventSource) {
      eventSource.close()
    }

    error.value = null
    connectionState.value = 'Connecting...'

    eventSource = new EventSource(`${baseUrl}/sse/alerts`)

    eventSource.addEventListener('connected', (event) => {
      const data = JSON.parse(event.data)
      clientId.value = data.clientId
      isConnected.value = true
      connectionState.value = 'Connected'
      console.log('SSE Connected:', data)
    })

    eventSource.addEventListener('alert', (event) => {
      const alert = JSON.parse(event.data)
      alerts.value.unshift(alert)

      // Keep only last 100 alerts
      if (alerts.value.length > 100) {
        alerts.value.pop()
      }

      // Notify listeners
      listeners.forEach((callback) => callback(alert))
    })

    eventSource.onerror = (err) => {
      console.error('SSE Error:', err)
      isConnected.value = false
      connectionState.value = 'Error'
      error.value = 'Connection lost. Retrying...'
    }

    eventSource.onopen = () => {
      console.log('SSE Connection opened')
    }
  }

  const disconnect = () => {
    if (eventSource) {
      eventSource.close()
      eventSource = null
    }
    isConnected.value = false
    connectionState.value = 'Disconnected'
    clientId.value = null
  }

  const onAlert = (callback) => {
    const id = Date.now().toString()
    listeners.set(id, callback)
    return () => listeners.delete(id)
  }

  const clearAlerts = () => {
    alerts.value = []
  }

  onUnmounted(() => {
    disconnect()
  })

  return {
    isConnected,
    connectionState,
    clientId,
    error,
    alerts,
    connect,
    disconnect,
    onAlert,
    clearAlerts
  }
}

export async function triggerTestAlert(baseUrl, alert) {
  const response = await fetch(`${baseUrl}/sse/alerts/test`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(alert),
  })
  return response.json()
}

export async function getSSEStats(baseUrl) {
  const response = await fetch(`${baseUrl}/sse/stats`)
  return response.json()
}
