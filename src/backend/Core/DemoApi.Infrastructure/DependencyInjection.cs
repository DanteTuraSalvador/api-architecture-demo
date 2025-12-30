using DemoApi.Application.Interfaces;
using DemoApi.Application.Services;
using DemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<FleetDbContext>(options =>
            options.UseInMemoryDatabase("FleetManagementDb"));

        // Register services as singletons for in-memory storage to persist data
        services.AddSingleton<IVehicleService, VehicleService>();
        services.AddSingleton<IDriverService, DriverService>();
        services.AddSingleton<ITripService, TripService>();
        services.AddSingleton<IDeliveryService, DeliveryService>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<IFleetService, FleetService>();
        services.AddSingleton<ITelemetryService, TelemetryService>();

        return services;
    }

    public static void SeedDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FleetDbContext>();
        DbSeeder.SeedData(context);
    }
}
