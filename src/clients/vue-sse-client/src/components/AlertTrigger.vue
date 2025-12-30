<script setup>
import { ref } from 'vue'
import { triggerTestAlert } from '../composables/useSSE'

const props = defineProps({
  baseUrl: {
    type: String,
    default: 'https://localhost:5001'
  }
})

const emit = defineEmits(['alert-triggered'])

const isSubmitting = ref(false)
const message = ref(null)

const alertTypes = [
  { value: 0, label: 'Speeding' },
  { value: 1, label: 'Harsh Braking' },
  { value: 2, label: 'Low Fuel' },
  { value: 3, label: 'Engine Warning' },
  { value: 4, label: 'Geofence Violation' }
]

const alertLevels = [
  { value: 0, label: 'Info' },
  { value: 1, label: 'Warning' },
  { value: 2, label: 'Critical' }
]

const form = ref({
  vehicleId: crypto.randomUUID(),
  type: 0,
  level: 1,
  message: 'Test alert from Vue SSE client'
})

const submitAlert = async () => {
  isSubmitting.value = true
  message.value = null

  try {
    const result = await triggerTestAlert(props.baseUrl, form.value)
    message.value = { type: 'success', text: 'Alert triggered successfully!' }
    emit('alert-triggered', result.alert)

    // Generate new vehicle ID for next test
    form.value.vehicleId = crypto.randomUUID()
  } catch (err) {
    message.value = { type: 'error', text: `Failed to trigger alert: ${err.message}` }
  } finally {
    isSubmitting.value = false
  }
}

const triggerMultiple = async () => {
  isSubmitting.value = true
  message.value = null

  try {
    const alerts = [
      { ...form.value, vehicleId: crypto.randomUUID(), type: 0, level: 1, message: 'Speeding detected' },
      { ...form.value, vehicleId: crypto.randomUUID(), type: 2, level: 2, message: 'Critical fuel level' },
      { ...form.value, vehicleId: crypto.randomUUID(), type: 3, level: 2, message: 'Engine temperature high' }
    ]

    await Promise.all(alerts.map(a => triggerTestAlert(props.baseUrl, a)))
    message.value = { type: 'success', text: '3 alerts triggered!' }
  } catch (err) {
    message.value = { type: 'error', text: `Failed: ${err.message}` }
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="alert-trigger">
    <h3>Trigger Test Alert</h3>
    <p class="description">Send test alerts to the SSE stream</p>

    <form @submit.prevent="submitAlert" class="trigger-form">
      <div class="form-group">
        <label for="alertType">Alert Type</label>
        <select id="alertType" v-model.number="form.type" class="form-control">
          <option v-for="type in alertTypes" :key="type.value" :value="type.value">
            {{ type.label }}
          </option>
        </select>
      </div>

      <div class="form-group">
        <label for="alertLevel">Alert Level</label>
        <select id="alertLevel" v-model.number="form.level" class="form-control">
          <option v-for="level in alertLevels" :key="level.value" :value="level.value">
            {{ level.label }}
          </option>
        </select>
      </div>

      <div class="form-group">
        <label for="message">Message</label>
        <input
          id="message"
          v-model="form.message"
          type="text"
          class="form-control"
          placeholder="Alert message..."
        />
      </div>

      <div class="form-actions">
        <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Sending...' : 'Send Alert' }}
        </button>
        <button type="button" class="btn btn-secondary" @click="triggerMultiple" :disabled="isSubmitting">
          Send 3 Random
        </button>
      </div>
    </form>

    <div v-if="message" :class="['message', message.type]">
      {{ message.text }}
    </div>
  </div>
</template>

<style scoped>
.alert-trigger {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.alert-trigger h3 {
  margin: 0 0 0.25rem;
  color: #333;
}

.description {
  color: #6b7280;
  margin: 0 0 1.5rem;
  font-size: 0.875rem;
}

.trigger-form {
  display: grid;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.form-group label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
}

.form-control {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
}

.form-control:focus {
  outline: none;
  border-color: #10b981;
  box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.1);
}

.form-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
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
  flex: 1;
}

.btn-primary:hover:not(:disabled) {
  background: #059669;
}

.btn-secondary {
  background: #6b7280;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #4b5563;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.message {
  margin-top: 1rem;
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.875rem;
}

.message.success {
  background: #dcfce7;
  color: #166534;
}

.message.error {
  background: #fee2e2;
  color: #991b1b;
}
</style>
