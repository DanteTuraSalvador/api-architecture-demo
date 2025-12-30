using DemoApi.Domain.Entities;

namespace DemoApi.Application.Interfaces;

public interface ITelemetryService
{
    Task<IEnumerable<Telemetry>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Telemetry>> GetByDeviceIdAsync(Guid deviceId);
    Task<Telemetry> RecordAsync(Telemetry telemetry);
    Task<IEnumerable<Telemetry>> GetLatestByVehicleAsync(Guid vehicleId, int count = 10);
    IAsyncEnumerable<Telemetry> StreamTelemetryAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}
