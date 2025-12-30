using Microsoft.AspNetCore.SignalR;

namespace DemoApi.Gateway.Hubs;

/// <summary>
/// SignalR Hub for WebRTC signaling - demonstrates P2P connection setup
/// </summary>
public class SignalingHub : Hub
{
    private readonly ILogger<SignalingHub> _logger;
    private static readonly Dictionary<string, PeerInfo> _peers = new();

    public SignalingHub(ILogger<SignalingHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Signaling client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_peers.TryGetValue(Context.ConnectionId, out var peer))
        {
            _peers.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("PeerLeft", peer.PeerId);
        }

        _logger.LogInformation("Signaling client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Register as a peer in the signaling network
    /// </summary>
    public async Task Register(string peerId, string displayName)
    {
        var peer = new PeerInfo
        {
            ConnectionId = Context.ConnectionId,
            PeerId = peerId,
            DisplayName = displayName,
            JoinedAt = DateTime.UtcNow
        };

        _peers[Context.ConnectionId] = peer;

        // Notify others about new peer
        await Clients.Others.SendAsync("PeerJoined", peer);

        // Send list of existing peers to the new peer
        var existingPeers = _peers.Values.Where(p => p.ConnectionId != Context.ConnectionId).ToList();
        await Clients.Caller.SendAsync("PeersList", existingPeers);

        _logger.LogInformation("Peer registered: {PeerId} ({DisplayName})", peerId, displayName);
    }

    /// <summary>
    /// Send an SDP offer to a specific peer
    /// </summary>
    public async Task SendOffer(string targetPeerId, string sdpOffer)
    {
        var targetConnection = _peers.Values.FirstOrDefault(p => p.PeerId == targetPeerId);
        if (targetConnection == null)
        {
            await Clients.Caller.SendAsync("Error", $"Peer {targetPeerId} not found");
            return;
        }

        var senderPeer = _peers.GetValueOrDefault(Context.ConnectionId);
        if (senderPeer == null) return;

        await Clients.Client(targetConnection.ConnectionId).SendAsync("OfferReceived", new
        {
            FromPeerId = senderPeer.PeerId,
            FromDisplayName = senderPeer.DisplayName,
            SdpOffer = sdpOffer
        });

        _logger.LogDebug("Offer sent from {From} to {To}", senderPeer.PeerId, targetPeerId);
    }

    /// <summary>
    /// Send an SDP answer back to a peer
    /// </summary>
    public async Task SendAnswer(string targetPeerId, string sdpAnswer)
    {
        var targetConnection = _peers.Values.FirstOrDefault(p => p.PeerId == targetPeerId);
        if (targetConnection == null)
        {
            await Clients.Caller.SendAsync("Error", $"Peer {targetPeerId} not found");
            return;
        }

        var senderPeer = _peers.GetValueOrDefault(Context.ConnectionId);
        if (senderPeer == null) return;

        await Clients.Client(targetConnection.ConnectionId).SendAsync("AnswerReceived", new
        {
            FromPeerId = senderPeer.PeerId,
            SdpAnswer = sdpAnswer
        });

        _logger.LogDebug("Answer sent from {From} to {To}", senderPeer.PeerId, targetPeerId);
    }

    /// <summary>
    /// Send an ICE candidate to a specific peer
    /// </summary>
    public async Task SendIceCandidate(string targetPeerId, string candidate, string sdpMid, int sdpMLineIndex)
    {
        var targetConnection = _peers.Values.FirstOrDefault(p => p.PeerId == targetPeerId);
        if (targetConnection == null) return;

        var senderPeer = _peers.GetValueOrDefault(Context.ConnectionId);
        if (senderPeer == null) return;

        await Clients.Client(targetConnection.ConnectionId).SendAsync("IceCandidateReceived", new
        {
            FromPeerId = senderPeer.PeerId,
            Candidate = candidate,
            SdpMid = sdpMid,
            SdpMLineIndex = sdpMLineIndex
        });
    }

    /// <summary>
    /// Get list of available peers
    /// </summary>
    public Task<List<PeerInfo>> GetPeers()
    {
        return Task.FromResult(_peers.Values
            .Where(p => p.ConnectionId != Context.ConnectionId)
            .ToList());
    }
}

public record PeerInfo
{
    public string ConnectionId { get; init; } = string.Empty;
    public string PeerId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public DateTime JoinedAt { get; init; }
}
