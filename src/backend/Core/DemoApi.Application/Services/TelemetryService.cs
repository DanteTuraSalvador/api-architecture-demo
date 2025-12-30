using System.Runtime.CompilerServices;
using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;

namespace DemoApi.Application.Services;

public class TelemetryService : ITelemetryService
{
    private readonly List<Telemetry> _telemetryData = new();
    private readonly Random _random = new();

    public Task<IEnumerable<Telemetry>> GetByVehicleIdAsync(Guid vehicleId)
    {
        var data = _telemetryData.Where(t => t.VehicleId == vehicleId).OrderByDescending(t => t.Timestamp);
        return Task.FromResult<IEnumerable<Telemetry>>(data.ToList());
    }

    public Task<IEnumerable<Telemetry>> GetByDeviceIdAsync(Guid deviceId)
    {
        var data = _telemetryData.Where(t => t.DeviceId == deviceId).OrderByDescending(t => t.Timestamp);
        return Task.FromResult<IEnumerable<Telemetry>>(data.ToList());
    }

    public Task<Telemetry> RecordAsync(Telemetry telemetry)
    {
        telemetry.Id = Guid.NewGuid();
        telemetry.Timestamp = DateTime.UtcNow;
        _telemetryData.Add(telemetry);
        return Task.FromResult(telemetry);
    }

    public Task<IEnumerable<Telemetry>> GetLatestByVehicleAsync(Guid vehicleId, int count = 10)
    {
        var data = _telemetryData
            .Where(t => t.VehicleId == vehicleId)
            .OrderByDescending(t => t.Timestamp)
            .Take(count);
        return Task.FromResult<IEnumerable<Telemetry>>(data.ToList());
    }

    public async IAsyncEnumerable<Telemetry> StreamTelemetryAsync(
        Guid vehicleId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var telemetry = new Telemetry
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                DeviceId = Guid.NewGuid(),
                Type = (TelemetryType)_random.Next(0, 6),
                Value = _random.NextDouble() * 100,
                Unit = GetUnitForType((TelemetryType)_random.Next(0, 6)),
                Timestamp = DateTime.UtcNow
            };

            yield return telemetry;

            await Task.Delay(1000, cancellationToken);
        }
    }

    private static string GetUnitForType(TelemetryType type)
    {
        return type switch
        {
            TelemetryType.Location => "degrees",
            TelemetryType.Speed => "km/h",
            TelemetryType.FuelLevel => "%",
            TelemetryType.EngineTemperature => "°C",
            TelemetryType.BatteryVoltage => "V",
            TelemetryType.Odometer => "km",
            _ => "unit"
        };
    }
}
