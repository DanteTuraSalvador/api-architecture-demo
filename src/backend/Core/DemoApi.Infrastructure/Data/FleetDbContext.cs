using DemoApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Infrastructure.Data;

public class FleetDbContext : DbContext
{
    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options)
    {
    }

    public DbSet<Fleet> Fleets => Set<Fleet>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<VehicleDevice> VehicleDevices => Set<VehicleDevice>();
    public DbSet<Telemetry> TelemetryData => Set<Telemetry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fleet configuration
        modelBuilder.Entity<Fleet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.HasMany(e => e.Vehicles)
                  .WithOne(v => v.Fleet)
                  .HasForeignKey(v => v.FleetId);
        });

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VehicleNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(20);
            entity.HasOne(e => e.CurrentDriver)
                  .WithMany()
                  .HasForeignKey(e => e.CurrentDriverId)
                  .IsRequired(false);
            entity.HasMany(e => e.Devices)
                  .WithOne(d => d.Vehicle)
                  .HasForeignKey(d => d.VehicleId);
            entity.HasMany(e => e.Trips)
                  .WithOne(t => t.Vehicle)
                  .HasForeignKey(t => t.VehicleId);
            entity.HasMany(e => e.Alerts)
                  .WithOne(a => a.Vehicle)
                  .HasForeignKey(a => a.VehicleId);
        });

        // Driver configuration
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Ignore(e => e.FullName);
        });

        // Trip configuration
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.StartLocation);
            entity.OwnsOne(e => e.EndLocation);
            entity.HasOne(e => e.Driver)
                  .WithMany(d => d.Trips)
                  .HasForeignKey(e => e.DriverId);
            entity.HasMany(e => e.Deliveries)
                  .WithOne(d => d.Trip)
                  .HasForeignKey(d => d.TripId);
        });

        // Delivery configuration
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrackingNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RecipientName).HasMaxLength(200);
            entity.Property(e => e.RecipientPhone).HasMaxLength(20);
            entity.OwnsOne(e => e.PickupLocation);
            entity.OwnsOne(e => e.DeliveryLocation);
        });

        // Alert configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
        });

        // VehicleDevice configuration
        modelBuilder.Entity<VehicleDevice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(100);
            entity.HasMany(e => e.TelemetryData)
                  .WithOne(t => t.Device)
                  .HasForeignKey(t => t.DeviceId);
        });

        // Telemetry configuration
        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });
    }
}
