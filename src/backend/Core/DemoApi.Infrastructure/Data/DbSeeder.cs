using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Infrastructure.Data;

public static class DbSeeder
{
    public static void SeedData(FleetDbContext context)
    {
        if (context.Fleets.Any()) return;

        var fleetId = Guid.NewGuid();
        var fleet = new Fleet
        {
            Id = fleetId,
            Name = "Main Fleet",
            CompanyName = "Demo Logistics Inc.",
            CreatedAt = DateTime.UtcNow
        };
        context.Fleets.Add(fleet);

        // Seed Drivers
        var drivers = new List<Driver>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith",
                LicenseNumber = "DL-001-2024",
                PhoneNumber = "+1-555-0101",
                Email = "john.smith@demo.com",
                Status = DriverStatus.Available,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Sarah",
                LastName = "Johnson",
                LicenseNumber = "DL-002-2024",
                PhoneNumber = "+1-555-0102",
                Email = "sarah.johnson@demo.com",
                Status = DriverStatus.OnDuty,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Mike",
                LastName = "Williams",
                LicenseNumber = "DL-003-2024",
                PhoneNumber = "+1-555-0103",
                Email = "mike.williams@demo.com",
                Status = DriverStatus.Available,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Emily",
                LastName = "Brown",
                LicenseNumber = "DL-004-2024",
                PhoneNumber = "+1-555-0104",
                Email = "emily.brown@demo.com",
                Status = DriverStatus.OffDuty,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "David",
                LastName = "Garcia",
                LicenseNumber = "DL-005-2024",
                PhoneNumber = "+1-555-0105",
                Email = "david.garcia@demo.com",
                Status = DriverStatus.OnBreak,
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Drivers.AddRange(drivers);

        // Seed Vehicles
        var vehicles = new List<Vehicle>
        {
            new()
            {
                Id = Guid.NewGuid(),
                VehicleNumber = "TRUCK-001",
                LicensePlate = "ABC-1234",
                Type = VehicleType.Truck,
                Status = VehicleStatus.Available,
                FleetId = fleetId,
                Mileage = 45000,
                LastMaintenance = DateTime.UtcNow.AddDays(-30),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleNumber = "VAN-001",
                LicensePlate = "DEF-5678",
                Type = VehicleType.Van,
                Status = VehicleStatus.InTransit,
                CurrentDriverId = drivers[1].Id,
                FleetId = fleetId,
                Mileage = 32000,
                LastMaintenance = DateTime.UtcNow.AddDays(-15),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleNumber = "TRUCK-002",
                LicensePlate = "GHI-9012",
                Type = VehicleType.Truck,
                Status = VehicleStatus.Maintenance,
                FleetId = fleetId,
                Mileage = 78000,
                LastMaintenance = DateTime.UtcNow.AddDays(-60),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleNumber = "CAR-001",
                LicensePlate = "JKL-3456",
                Type = VehicleType.Car,
                Status = VehicleStatus.Available,
                FleetId = fleetId,
                Mileage = 15000,
                LastMaintenance = DateTime.UtcNow.AddDays(-7),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleNumber = "BUS-001",
                LicensePlate = "MNO-7890",
                Type = VehicleType.Bus,
                Status = VehicleStatus.OutOfService,
                FleetId = fleetId,
                Mileage = 120000,
                LastMaintenance = DateTime.UtcNow.AddDays(-90),
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Vehicles.AddRange(vehicles);

        // Seed Alerts
        var alerts = new List<Alert>
        {
            new()
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicles[1].Id,
                Type = AlertType.Speeding,
                Level = AlertLevel.Warning,
                Message = "Vehicle exceeded speed limit by 15 km/h",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                IsAcknowledged = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicles[2].Id,
                Type = AlertType.MaintenanceDue,
                Level = AlertLevel.Critical,
                Message = "Scheduled maintenance overdue by 30 days",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                IsAcknowledged = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicles[0].Id,
                Type = AlertType.LowFuel,
                Level = AlertLevel.Info,
                Message = "Fuel level at 25%",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                IsAcknowledged = true,
                AcknowledgedAt = DateTime.UtcNow.AddMinutes(-45)
            },
            new()
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicles[4].Id,
                Type = AlertType.EngineWarning,
                Level = AlertLevel.Critical,
                Message = "Engine temperature exceeds normal range",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsAcknowledged = true,
                AcknowledgedAt = DateTime.UtcNow.AddDays(-1).AddHours(1)
            }
        };
        context.Alerts.AddRange(alerts);

        // Seed a Trip with Deliveries
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicles[1].Id,
            DriverId = drivers[1].Id,
            StartTime = DateTime.UtcNow.AddHours(-3),
            Status = TripStatus.InProgress,
            StartLocation = new Location
            {
                Latitude = 40.7128,
                Longitude = -74.0060,
                Address = "123 Main St, New York, NY",
                Timestamp = DateTime.UtcNow.AddHours(-3)
            },
            DistanceTraveled = 45.5,
            FuelConsumed = 12.3,
            CreatedAt = DateTime.UtcNow.AddHours(-3)
        };
        context.Trips.Add(trip);

        var deliveries = new List<Delivery>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                TrackingNumber = "TRK-20241227-ABC123",
                Status = DeliveryStatus.Delivered,
                RecipientName = "Alice Cooper",
                RecipientPhone = "+1-555-0201",
                PickupLocation = new Location
                {
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    Address = "123 Main St, New York, NY",
                    Timestamp = DateTime.UtcNow.AddHours(-3)
                },
                DeliveryLocation = new Location
                {
                    Latitude = 40.7580,
                    Longitude = -73.9855,
                    Address = "456 Broadway, New York, NY",
                    Timestamp = DateTime.UtcNow.AddHours(-2)
                },
                PickupTime = DateTime.UtcNow.AddHours(-3),
                DeliveryTime = DateTime.UtcNow.AddHours(-2),
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            },
            new()
            {
                Id = Guid.NewGuid(),
                TripId = trip.Id,
                TrackingNumber = "TRK-20241227-DEF456",
                Status = DeliveryStatus.InTransit,
                RecipientName = "Bob Wilson",
                RecipientPhone = "+1-555-0202",
                PickupLocation = new Location
                {
                    Latitude = 40.7580,
                    Longitude = -73.9855,
                    Address = "456 Broadway, New York, NY",
                    Timestamp = DateTime.UtcNow.AddHours(-1)
                },
                DeliveryLocation = new Location
                {
                    Latitude = 40.7484,
                    Longitude = -73.9857,
                    Address = "789 Fifth Ave, New York, NY",
                    Timestamp = DateTime.UtcNow
                },
                PickupTime = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            }
        };
        context.Deliveries.AddRange(deliveries);

        context.SaveChanges();
    }
}
