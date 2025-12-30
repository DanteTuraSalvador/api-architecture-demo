import { gql } from '@apollo/client'

export const CREATE_VEHICLE = gql`
  mutation CreateVehicle($input: CreateVehicleInput!) {
    createVehicle(input: $input) {
      id
      vehicleNumber
      licensePlate
      type
      status
      mileage
    }
  }
`

export const UPDATE_VEHICLE_STATUS = gql`
  mutation UpdateVehicleStatus($id: UUID!, $status: VehicleStatus!) {
    updateVehicleStatus(id: $id, status: $status) {
      id
      vehicleNumber
      status
    }
  }
`

export const DELETE_VEHICLE = gql`
  mutation DeleteVehicle($id: UUID!) {
    deleteVehicle(id: $id)
  }
`

export const CREATE_DRIVER = gql`
  mutation CreateDriver($input: CreateDriverInput!) {
    createDriver(input: $input) {
      id
      firstName
      lastName
      licenseNumber
      status
    }
  }
`

export const UPDATE_DRIVER_STATUS = gql`
  mutation UpdateDriverStatus($id: UUID!, $status: DriverStatus!) {
    updateDriverStatus(id: $id, status: $status) {
      id
      firstName
      lastName
      status
    }
  }
`

export const DELETE_DRIVER = gql`
  mutation DeleteDriver($id: UUID!) {
    deleteDriver(id: $id)
  }
`

export const CREATE_TRIP = gql`
  mutation CreateTrip($input: CreateTripInput!) {
    createTrip(input: $input) {
      id
      vehicleId
      driverId
      startTime
      status
    }
  }
`

export const START_TRIP = gql`
  mutation StartTrip($id: UUID!) {
    startTrip(id: $id) {
      id
      startTime
      status
    }
  }
`

export const COMPLETE_TRIP = gql`
  mutation CompleteTrip($id: UUID!) {
    completeTrip(id: $id) {
      id
      endTime
      status
      distance
    }
  }
`

export const CREATE_ALERT = gql`
  mutation CreateAlert($input: CreateAlertInput!) {
    createAlert(input: $input) {
      id
      vehicleId
      type
      level
      message
      createdAt
    }
  }
`

export const ACKNOWLEDGE_ALERT = gql`
  mutation AcknowledgeAlert($id: UUID!) {
    acknowledgeAlert(id: $id) {
      id
      isAcknowledged
      acknowledgedAt
    }
  }
`

export const MARK_DELIVERY_PICKED_UP = gql`
  mutation MarkDeliveryPickedUp($id: UUID!) {
    markDeliveryPickedUp(id: $id) {
      id
      status
    }
  }
`

export const MARK_DELIVERY_COMPLETE = gql`
  mutation MarkDeliveryComplete($id: UUID!) {
    markDeliveryComplete(id: $id) {
      id
      status
      actualDelivery
    }
  }
`
