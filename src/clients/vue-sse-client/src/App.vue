<script setup>
import AlertStream from './components/AlertStream.vue'
import AlertTrigger from './components/AlertTrigger.vue'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001'
</script>

<template>
  <div class="app">
    <header class="app-header">
      <h1>Fleet Alerts Dashboard</h1>
      <p>Real-time alerts using Server-Sent Events (SSE)</p>
    </header>

    <main class="app-main">
      <div class="content-grid">
        <div class="stream-section">
          <AlertStream :baseUrl="API_BASE_URL" />
        </div>

        <aside class="sidebar">
          <AlertTrigger :baseUrl="API_BASE_URL" />

          <div class="info-card">
            <h3>About SSE</h3>
            <p>
              Server-Sent Events provide a simple, efficient mechanism for the server
              to push real-time updates to the browser over a single HTTP connection.
            </p>
            <ul>
              <li>One-way communication (server to client)</li>
              <li>Automatic reconnection</li>
              <li>Event-based messaging</li>
              <li>Simpler than WebSockets for server push</li>
            </ul>
          </div>

          <div class="tech-stack">
            <h4>Technology Stack</h4>
            <div class="tech-list">
              <span class="tech-badge vue">Vue 3</span>
              <span class="tech-badge composition">Composition API</span>
              <span class="tech-badge sse">EventSource</span>
              <span class="tech-badge dotnet">.NET 9</span>
            </div>
          </div>
        </aside>
      </div>
    </main>

    <footer class="app-footer">
      <p>
        Part of the Multi-Protocol API Architecture Demo<br>
        REST | GraphQL | gRPC | SignalR | <strong>SSE</strong> | MQTT | WebRTC
      </p>
    </footer>
  </div>
</template>

<style>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: #f3f4f6;
  min-height: 100vh;
}

.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  padding: 2rem;
  text-align: center;
}

.app-header h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

.app-header p {
  opacity: 0.9;
}

.app-main {
  flex: 1;
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
  width: 100%;
}

.content-grid {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 2rem;
}

@media (max-width: 1024px) {
  .content-grid {
    grid-template-columns: 1fr;
  }
}

.stream-section {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.sidebar {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.info-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.info-card h3 {
  margin-bottom: 0.75rem;
  color: #333;
}

.info-card p {
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.6;
  margin-bottom: 1rem;
}

.info-card ul {
  padding-left: 1.25rem;
  color: #4b5563;
  font-size: 0.875rem;
}

.info-card li {
  margin-bottom: 0.5rem;
}

.tech-stack {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.tech-stack h4 {
  margin-bottom: 1rem;
  color: #333;
}

.tech-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tech-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
}

.tech-badge.vue {
  background: #dcfce7;
  color: #166534;
}

.tech-badge.composition {
  background: #dbeafe;
  color: #1e40af;
}

.tech-badge.sse {
  background: #fef3c7;
  color: #92400e;
}

.tech-badge.dotnet {
  background: #ede9fe;
  color: #5b21b6;
}

.app-footer {
  background: #1f2937;
  color: #9ca3af;
  text-align: center;
  padding: 1.5rem;
  font-size: 0.875rem;
}

.app-footer strong {
  color: #10b981;
}
</style>
