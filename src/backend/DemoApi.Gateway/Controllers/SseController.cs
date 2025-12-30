using System.Text.Json;
using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using DemoApi.Gateway.SSE;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

/// <summary>
/// Controller for Server-Sent Events - demonstrates one-way server push
/// </summary>
[ApiController]
[Route("sse")]
public class SseController : ControllerBase
{
    private readonly AlertStreamService _alertStream;
    private readonly IAlertService _alertService;
    private readonly ILogger<SseController> _logger;

    public SseController(
        AlertStreamService alertStream,
        IAlertService alertService,
        ILogger<SseController> logger)
    {
        _alertStream = alertStream;
        _alertService = alertService;
        _logger = logger;
    }

    /// <summary>
    /// Subscribe to the alert stream via Server-Sent Events
    /// </summary>
    [HttpGet("alerts")]
    public async Task GetAlertStream(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no");

        var clientId = Guid.NewGuid().ToString();
        var channel = _alertStream.Subscribe(clientId);

        _logger.LogInformation("SSE client connected: {ClientId}", clientId);

        try
        {
            // Send initial connection event
            await WriteEventAsync("connected", new { clientId, message = "Connected to alert stream" });

            // Stream alerts as they arrive
            await foreach (var alert in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await WriteEventAsync("alert", alert);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("SSE client disconnected: {ClientId}", clientId);
        }
        finally
        {
            _alertStream.Unsubscribe(clientId);
        }
    }

    /// <summary>
    /// Trigger a test alert (for demo purposes)
    /// </summary>
    [HttpPost("alerts/test")]
    public async Task<IActionResult> TriggerTestAlert([FromBody] TestAlertRequest request)
    {
        var alert = new Alert
        {
            Id = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            Type = request.Type,
            Level = request.Level,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow,
            IsAcknowledged = false
        };

        // Save to alert service
        await _alertService.CreateAsync(alert);

        // Broadcast to SSE clients
        await _alertStream.BroadcastAlertAsync(alert);

        return Ok(new { message = "Alert triggered and broadcast", alert });
    }

    /// <summary>
    /// Get stream statistics
    /// </summary>
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            connectedClients = _alertStream.GetClientCount(),
            timestamp = DateTime.UtcNow
        });
    }

    private async Task WriteEventAsync<T>(string eventType, T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await Response.WriteAsync($"event: {eventType}\n");
        await Response.WriteAsync($"data: {json}\n\n");
        await Response.Body.FlushAsync();
    }
}

public record TestAlertRequest
{
    public Guid VehicleId { get; init; }
    public AlertType Type { get; init; }
    public AlertLevel Level { get; init; }
    public string Message { get; init; } = string.Empty;
}
