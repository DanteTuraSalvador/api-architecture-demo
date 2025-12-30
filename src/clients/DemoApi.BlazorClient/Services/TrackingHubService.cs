using DemoApi.BlazorClient.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace DemoApi.BlazorClient.Services;

public class TrackingHubService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly string _hubUrl;

    public event Action<LocationUpdate>? OnLocationUpdated;
    public event Action<LocationUpdate>? OnVehicleMoved;
    public event Action<AlertNotification>? OnAlertReceived;
    public event Action<string>? OnConnectionStateChanged;

    public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
    public string ConnectionState => _hubConnection.State.ToString();

    public TrackingHubService(string hubUrl)
    {
        _hubUrl = hubUrl;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        _hubConnection.On<LocationUpdate>("LocationUpdated", update =>
        {
            OnLocationUpdated?.Invoke(update);
        });

        _hubConnection.On<LocationUpdate>("VehicleMoved", update =>
        {
            OnVehicleMoved?.Invoke(update);
        });

        _hubConnection.On<AlertNotification>("AlertReceived", alert =>
        {
            OnAlertReceived?.Invoke(alert);
        });

        _hubConnection.Reconnecting += error =>
        {
            OnConnectionStateChanged?.Invoke("Reconnecting...");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            OnConnectionStateChanged?.Invoke("Connected");
            return Task.CompletedTask;
        };

        _hubConnection.Closed += error =>
        {
            OnConnectionStateChanged?.Invoke("Disconnected");
            return Task.CompletedTask;
        };
    }

    public async Task StartAsync()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
            OnConnectionStateChanged?.Invoke("Connected");
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection.State != HubConnectionState.Disconnected)
        {
            await _hubConnection.StopAsync();
            OnConnectionStateChanged?.Invoke("Disconnected");
        }
    }

    public async Task SubscribeToVehicleAsync(Guid vehicleId)
    {
        await _hubConnection.InvokeAsync("SubscribeToVehicle", vehicleId);
    }

    public async Task UnsubscribeFromVehicleAsync(Guid vehicleId)
    {
        await _hubConnection.InvokeAsync("UnsubscribeFromVehicle", vehicleId);
    }

    public async Task SubscribeToFleetAsync(Guid fleetId)
    {
        await _hubConnection.InvokeAsync("SubscribeToFleet", fleetId);
    }

    public async Task SendLocationUpdateAsync(Guid vehicleId, double latitude, double longitude, double speed)
    {
        await _hubConnection.InvokeAsync("SendLocationUpdate", vehicleId, latitude, longitude, speed);
    }

    public async Task BroadcastAlertAsync(Guid vehicleId, string alertType, string message)
    {
        await _hubConnection.InvokeAsync("BroadcastAlert", vehicleId, alertType, message);
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}
