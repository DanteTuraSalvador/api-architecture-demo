using System.Collections.Concurrent;
using System.Threading.Channels;
using DemoApi.Domain.Entities;

namespace DemoApi.Gateway.SSE;

/// <summary>
/// Service for managing SSE alert streams - demonstrates Server-Sent Events
/// </summary>
public class AlertStreamService
{
    private readonly ConcurrentDictionary<string, Channel<Alert>> _clients = new();
    private readonly ILogger<AlertStreamService> _logger;

    public AlertStreamService(ILogger<AlertStreamService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Subscribe a client to the alert stream
    /// </summary>
    public Channel<Alert> Subscribe(string clientId)
    {
        var channel = Channel.CreateUnbounded<Alert>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _clients.TryAdd(clientId, channel);
        _logger.LogInformation("Client {ClientId} subscribed to alert stream. Total clients: {Count}",
            clientId, _clients.Count);

        return channel;
    }

    /// <summary>
    /// Unsubscribe a client from the alert stream
    /// </summary>
    public void Unsubscribe(string clientId)
    {
        if (_clients.TryRemove(clientId, out var channel))
        {
            channel.Writer.Complete();
            _logger.LogInformation("Client {ClientId} unsubscribed from alert stream. Total clients: {Count}",
                clientId, _clients.Count);
        }
    }

    /// <summary>
    /// Broadcast an alert to all subscribed clients
    /// </summary>
    public async Task BroadcastAlertAsync(Alert alert)
    {
        var tasks = _clients.Values.Select(channel =>
            channel.Writer.WriteAsync(alert).AsTask());

        await Task.WhenAll(tasks);

        _logger.LogDebug("Alert broadcast to {Count} clients: {AlertType}",
            _clients.Count, alert.Type);
    }

    /// <summary>
    /// Get the number of connected clients
    /// </summary>
    public int GetClientCount() => _clients.Count;
}
