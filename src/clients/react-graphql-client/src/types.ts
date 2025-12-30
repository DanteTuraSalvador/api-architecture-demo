export const VehicleType = {
  TRUCK: 'TRUCK',
  VAN: 'VAN',
  CAR: 'CAR',
  MOTORCYCLE: 'MOTORCYCLE',
  BUS: 'BUS',
} as const
export type VehicleType = typeof VehicleType[keyof typeof VehicleType]

export const VehicleStatus = {
  AVAILABLE: 'AVAILABLE',
  IN_TRANSIT: 'IN_TRANSIT',
  MAINTENANCE: 'MAINTENANCE',
  OUT_OF_SERVICE: 'OUT_OF_SERVICE',
} as const
export type VehicleStatus = typeof VehicleStatus[keyof typeof VehicleStatus]

export const DriverStatus = {
  AVAILABLE: 'AVAILABLE',
  ON_DUTY: 'ON_DUTY',
  OFF_DUTY: 'OFF_DUTY',
  ON_BREAK: 'ON_BREAK',
} as const
export type DriverStatus = typeof DriverStatus[keyof typeof DriverStatus]

export const TripStatus = {
  SCHEDULED: 'SCHEDULED',
  IN_PROGRESS: 'IN_PROGRESS',
  COMPLETED: 'COMPLETED',
  CANCELLED: 'CANCELLED',
} as const
export type TripStatus = typeof TripStatus[keyof typeof TripStatus]

export const AlertType = {
  SPEEDING: 'SPEEDING',
  HARSH_BRAKING: 'HARSH_BRAKING',
  LOW_FUEL: 'LOW_FUEL',
  ENGINE_WARNING: 'ENGINE_WARNING',
  GEOFENCE_VIOLATION: 'GEOFENCE_VIOLATION',
} as const
export type AlertType = typeof AlertType[keyof typeof AlertType]

export const AlertLevel = {
  INFO: 'INFO',
  WARNING: 'WARNING',
  CRITICAL: 'CRITICAL',
} as const
export type AlertLevel = typeof AlertLevel[keyof typeof AlertLevel]

export interface Vehicle {
  id: string
  vehicleNumber: string
  licensePlate: string
  type: VehicleType
  status: VehicleStatus
  fleetId: string
  mileage: number
  lastMaintenance: string
  createdAt?: string
}

export interface Driver {
  id: string
  firstName: string
  lastName: string
  licenseNumber: string
  phoneNumber: string
  email: string
  status: DriverStatus
  createdAt?: string
}

export interface Trip {
  id: string
  vehicleId: string
  driverId: string
  startTime: string
  endTime?: string
  distance: number
  status: TripStatus
}

export interface Alert {
  id: string
  vehicleId: string
  type: AlertType
  level: AlertLevel
  message: string
  createdAt: string
  isAcknowledged: boolean
  acknowledgedAt?: string
}

export interface Fleet {
  id: string
  name: string
  description: string
  createdAt: string
  vehicles?: Vehicle[]
}

export interface Delivery {
  id: string
  trackingNumber: string
  recipientName: string
  recipientPhone: string
  deliveryAddress: string
  status: string
  estimatedDelivery: string
  actualDelivery?: string
}

export interface CreateVehicleInput {
  vehicleNumber: string
  licensePlate: string
  type: VehicleType
  fleetId: string
  mileage?: number
}

export interface CreateDriverInput {
  firstName: string
  lastName: string
  licenseNumber: string
  phoneNumber: string
  email: string
}

export interface CreateAlertInput {
  vehicleId: string
  type: AlertType
  level: AlertLevel
  message: string
}
