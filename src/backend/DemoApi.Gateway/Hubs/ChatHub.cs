using Microsoft.AspNetCore.SignalR;

namespace DemoApi.Gateway.Hubs;

/// <summary>
/// SignalR Hub for dispatcher-driver communication - demonstrates real-time chat
/// </summary>
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private static readonly Dictionary<string, UserInfo> _connectedUsers = new();

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Chat client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connectedUsers.TryGetValue(Context.ConnectionId, out var user))
        {
            _connectedUsers.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserLeft", user);
        }

        _logger.LogInformation("Chat client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join the chat with a username and role
    /// </summary>
    public async Task Join(string username, string role)
    {
        var user = new UserInfo
        {
            ConnectionId = Context.ConnectionId,
            Username = username,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        _connectedUsers[Context.ConnectionId] = user;

        // Add to role-based group
        await Groups.AddToGroupAsync(Context.ConnectionId, role);

        // Notify others
        await Clients.Others.SendAsync("UserJoined", user);

        // Send current users list to the new user
        await Clients.Caller.SendAsync("UsersOnline", _connectedUsers.Values.ToList());

        _logger.LogInformation("User {Username} ({Role}) joined chat", username, role);
    }

    /// <summary>
    /// Send a message to all users
    /// </summary>
    public async Task SendMessage(string message)
    {
        if (!_connectedUsers.TryGetValue(Context.ConnectionId, out var sender))
            return;

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            Sender = sender.Username,
            SenderRole = sender.Role,
            Content = message,
            Timestamp = DateTime.UtcNow
        };

        await Clients.All.SendAsync("MessageReceived", chatMessage);
        _logger.LogDebug("Message from {Sender}: {Message}", sender.Username, message);
    }

    /// <summary>
    /// Send a private message to a specific user
    /// </summary>
    public async Task SendPrivateMessage(string targetConnectionId, string message)
    {
        if (!_connectedUsers.TryGetValue(Context.ConnectionId, out var sender))
            return;

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            Sender = sender.Username,
            SenderRole = sender.Role,
            Content = message,
            Timestamp = DateTime.UtcNow,
            IsPrivate = true
        };

        await Clients.Client(targetConnectionId).SendAsync("PrivateMessageReceived", chatMessage);
        await Clients.Caller.SendAsync("PrivateMessageSent", chatMessage);
    }

    /// <summary>
    /// Send a message to all users with a specific role
    /// </summary>
    public async Task SendToRole(string role, string message)
    {
        if (!_connectedUsers.TryGetValue(Context.ConnectionId, out var sender))
            return;

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            Sender = sender.Username,
            SenderRole = sender.Role,
            Content = message,
            Timestamp = DateTime.UtcNow,
            TargetRole = role
        };

        await Clients.Group(role).SendAsync("RoleMessageReceived", chatMessage);
    }

    /// <summary>
    /// Get list of online users
    /// </summary>
    public Task<List<UserInfo>> GetOnlineUsers()
    {
        return Task.FromResult(_connectedUsers.Values.ToList());
    }
}

public record UserInfo
{
    public string ConnectionId { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime JoinedAt { get; init; }
}

public record ChatMessage
{
    public Guid Id { get; init; }
    public string Sender { get; init; } = string.Empty;
    public string SenderRole { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public bool IsPrivate { get; init; }
    public string? TargetRole { get; init; }
}
