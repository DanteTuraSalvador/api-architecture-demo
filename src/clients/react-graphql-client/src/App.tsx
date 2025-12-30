import { useState } from 'react'
import { FleetDashboard } from './components/FleetDashboard'
import { VehicleList } from './components/VehicleList'
import { DriverList } from './components/DriverList'
import { AlertList } from './components/AlertList'
import './App.css'

type Tab = 'dashboard' | 'vehicles' | 'drivers' | 'alerts'

function App() {
  const [activeTab, setActiveTab] = useState<Tab>('dashboard')

  return (
    <div className="app">
      <header className="app-header">
        <h1>Fleet Management</h1>
        <p>React GraphQL Client Demo</p>
      </header>

      <nav className="app-nav">
        <button
          className={`nav-btn ${activeTab === 'dashboard' ? 'active' : ''}`}
          onClick={() => setActiveTab('dashboard')}
        >
          Dashboard
        </button>
        <button
          className={`nav-btn ${activeTab === 'vehicles' ? 'active' : ''}`}
          onClick={() => setActiveTab('vehicles')}
        >
          Vehicles
        </button>
        <button
          className={`nav-btn ${activeTab === 'drivers' ? 'active' : ''}`}
          onClick={() => setActiveTab('drivers')}
        >
          Drivers
        </button>
        <button
          className={`nav-btn ${activeTab === 'alerts' ? 'active' : ''}`}
          onClick={() => setActiveTab('alerts')}
        >
          Alerts
        </button>
      </nav>

      <main className="app-main">
        {activeTab === 'dashboard' && <FleetDashboard />}
        {activeTab === 'vehicles' && <VehicleList />}
        {activeTab === 'drivers' && <DriverList />}
        {activeTab === 'alerts' && <AlertList />}
      </main>

      <footer className="app-footer">
        <p>
          Part of the Multi-Protocol API Architecture Demo<br />
          REST | GraphQL | gRPC | SignalR | SSE | MQTT | WebRTC
        </p>
      </footer>
    </div>
  )
}

export default App
