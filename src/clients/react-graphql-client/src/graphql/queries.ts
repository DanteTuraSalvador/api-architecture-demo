import { gql } from '@apollo/client'

export const GET_VEHICLES = gql`
  query GetVehicles($status: String) {
    vehicles(status: $status) {
      id
      vehicleNumber
      licensePlate
      type
      status
      mileage
      lastMaintenance
      fleetId
    }
  }
`

export const GET_VEHICLE = gql`
  query GetVehicle($id: UUID!) {
    vehicle(id: $id) {
      id
      vehicleNumber
      licensePlate
      type
      status
      mileage
      lastMaintenance
      fleetId
    }
  }
`

export const GET_DRIVERS = gql`
  query GetDrivers($status: String) {
    drivers(status: $status) {
      id
      firstName
      lastName
      licenseNumber
      phoneNumber
      email
      status
    }
  }
`

export const GET_AVAILABLE_DRIVERS = gql`
  query GetAvailableDrivers {
    availableDrivers {
      id
      firstName
      lastName
      licenseNumber
      status
    }
  }
`

export const GET_TRIPS = gql`
  query GetTrips {
    trips {
      id
      vehicleId
      driverId
      startTime
      endTime
      distance
      status
    }
  }
`

export const GET_ACTIVE_TRIPS = gql`
  query GetActiveTrips {
    activeTrips {
      id
      vehicleId
      driverId
      startTime
      distance
      status
    }
  }
`

export const GET_ALERTS = gql`
  query GetAlerts($unacknowledgedOnly: Boolean) {
    alerts(unacknowledgedOnly: $unacknowledgedOnly) {
      id
      vehicleId
      type
      level
      message
      createdAt
      isAcknowledged
      acknowledgedAt
    }
  }
`

export const GET_RECENT_ALERTS = gql`
  query GetRecentAlerts($count: Int!) {
    recentAlerts(count: $count) {
      id
      vehicleId
      type
      level
      message
      createdAt
      isAcknowledged
    }
  }
`

export const GET_FLEETS = gql`
  query GetFleets {
    fleets {
      id
      name
      description
      createdAt
    }
  }
`

export const GET_FLEET = gql`
  query GetFleet($id: UUID!) {
    fleet(id: $id) {
      id
      name
      description
      vehicles {
        id
        vehicleNumber
        status
      }
    }
  }
`

export const GET_DELIVERIES = gql`
  query GetDeliveries {
    deliveries {
      id
      trackingNumber
      status
      recipientName
      deliveryAddress
      estimatedDelivery
      actualDelivery
    }
  }
`

export const GET_DELIVERY_BY_TRACKING = gql`
  query GetDeliveryByTracking($trackingNumber: String!) {
    deliveryByTracking(trackingNumber: $trackingNumber) {
      id
      trackingNumber
      status
      recipientName
      recipientPhone
      deliveryAddress
      estimatedDelivery
      actualDelivery
    }
  }
`
