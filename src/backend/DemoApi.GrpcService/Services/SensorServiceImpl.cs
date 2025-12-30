using DemoApi.GrpcService.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace DemoApi.GrpcService.Services;

/// <summary>
/// gRPC service for sensor telemetry - demonstrates high-performance streaming
/// </summary>
public class SensorServiceImpl : SensorService.SensorServiceBase
{
    private readonly ILogger<SensorServiceImpl> _logger;
    private readonly Random _random = new();

    public SensorServiceImpl(ILogger<SensorServiceImpl> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Unary call: Get current telemetry for a vehicle
    /// </summary>
    public override Task<TelemetryData> GetCurrentTelemetry(VehicleRequest request, ServerCallContext context)
    {
        _logger.LogInformation("GetCurrentTelemetry called for vehicle: {VehicleId}", request.VehicleId);

        var telemetry = GenerateTelemetry(request.VehicleId, TelemetryType.Speed);
        return Task.FromResult(telemetry);
    }

    /// <summary>
    /// Server streaming: Stream telemetry data for a vehicle
    /// </summary>
    public override async Task StreamTelemetry(
        VehicleRequest request,
        IServerStreamWriter<TelemetryData> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("StreamTelemetry started for vehicle: {VehicleId}", request.VehicleId);

        var interval = request.IntervalMs > 0 ? request.IntervalMs : 1000;
        var types = System.Enum.GetValues<TelemetryType>().Where(t => t != TelemetryType.Unspecified).ToArray();

        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var type = types[_random.Next(types.Length)];
                var telemetry = GenerateTelemetry(request.VehicleId, type);

                await responseStream.WriteAsync(telemetry);
                _logger.LogDebug("Sent telemetry: {Type} = {Value} {Unit}",
                    telemetry.Type, telemetry.Value, telemetry.Unit);

                await Task.Delay(interval, context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("StreamTelemetry cancelled for vehicle: {VehicleId}", request.VehicleId);
        }
    }

    /// <summary>
    /// Client streaming: Receive batch of telemetry readings
    /// </summary>
    public override async Task<BatchResult> SendTelemetryBatch(
        IAsyncStreamReader<TelemetryData> requestStream,
        ServerCallContext context)
    {
        var receivedCount = 0;
        var processedCount = 0;

        _logger.LogInformation("SendTelemetryBatch started");

        await foreach (var telemetry in requestStream.ReadAllAsync(context.CancellationToken))
        {
            receivedCount++;

            // Simulate processing
            if (IsValidTelemetry(telemetry))
            {
                processedCount++;
                _logger.LogDebug("Processed telemetry: {VehicleId} - {Type} = {Value}",
                    telemetry.VehicleId, telemetry.Type, telemetry.Value);
            }
        }

        _logger.LogInformation("SendTelemetryBatch completed: {Received} received, {Processed} processed",
            receivedCount, processedCount);

        return new BatchResult
        {
            ReceivedCount = receivedCount,
            ProcessedCount = processedCount,
            Success = processedCount > 0,
            Message = $"Processed {processedCount} of {receivedCount} telemetry readings"
        };
    }

    /// <summary>
    /// Bidirectional streaming: Real-time telemetry exchange
    /// </summary>
    public override async Task TelemetryExchange(
        IAsyncStreamReader<TelemetryData> requestStream,
        IServerStreamWriter<TelemetryData> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("TelemetryExchange started");

        var readTask = Task.Run(async () =>
        {
            await foreach (var telemetry in requestStream.ReadAllAsync(context.CancellationToken))
            {
                _logger.LogDebug("Received from client: {VehicleId} - {Type}",
                    telemetry.VehicleId, telemetry.Type);

                // Echo back with processed flag
                var response = new TelemetryData
                {
                    Id = Guid.NewGuid().ToString(),
                    VehicleId = telemetry.VehicleId,
                    DeviceId = "server-processed",
                    Type = telemetry.Type,
                    Value = telemetry.Value * 1.0, // Just echo the value
                    Unit = telemetry.Unit,
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                };

                await responseStream.WriteAsync(response);
            }
        }, context.CancellationToken);

        await readTask;
        _logger.LogInformation("TelemetryExchange completed");
    }

    /// <summary>
    /// Get vehicle location history
    /// </summary>
    public override async Task GetLocationHistory(
        LocationHistoryRequest request,
        IServerStreamWriter<LocationData> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("GetLocationHistory called for vehicle: {VehicleId}", request.VehicleId);

        // Generate simulated location history
        var startLat = 40.7128;
        var startLng = -74.0060;

        for (int i = 0; i < 50 && !context.CancellationToken.IsCancellationRequested; i++)
        {
            var location = new LocationData
            {
                VehicleId = request.VehicleId,
                Latitude = startLat + (_random.NextDouble() - 0.5) * 0.01,
                Longitude = startLng + (_random.NextDouble() - 0.5) * 0.01,
                Speed = 20 + _random.NextDouble() * 60,
                Heading = _random.NextDouble() * 360,
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(-50 + i))
            };

            await responseStream.WriteAsync(location);
            await Task.Delay(100, context.CancellationToken);
        }

        _logger.LogInformation("GetLocationHistory completed");
    }

    private TelemetryData GenerateTelemetry(string vehicleId, TelemetryType type)
    {
        var (value, unit) = type switch
        {
            TelemetryType.Location => (0, "degrees"),
            TelemetryType.Speed => (_random.NextDouble() * 120, "km/h"),
            TelemetryType.FuelLevel => (20 + _random.NextDouble() * 80, "%"),
            TelemetryType.EngineTemperature => (70 + _random.NextDouble() * 40, "°C"),
            TelemetryType.BatteryVoltage => (11 + _random.NextDouble() * 3, "V"),
            TelemetryType.Odometer => (10000 + _random.NextDouble() * 100000, "km"),
            _ => (0, "unit")
        };

        return new TelemetryData
        {
            Id = Guid.NewGuid().ToString(),
            VehicleId = vehicleId,
            DeviceId = $"device-{vehicleId[..8]}",
            Type = type,
            Value = Math.Round(value, 2),
            Unit = unit,
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        };
    }

    private static bool IsValidTelemetry(TelemetryData telemetry)
    {
        return !string.IsNullOrEmpty(telemetry.VehicleId) &&
               telemetry.Type != TelemetryType.Unspecified;
    }
}
