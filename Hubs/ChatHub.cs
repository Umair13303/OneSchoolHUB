using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private static readonly Dictionary<int, HashSet<string>> _connections = new();
    private static readonly object _lock = new();

    private readonly AppDbContext _db;
    public ChatHub(AppDbContext db) => _db = db;

    public static bool IsOnline(int userId)
    {
        lock (_lock) { return _connections.TryGetValue(userId, out var c) && c.Count > 0; }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId == 0) return;

        lock (_lock)
        {
            if (!_connections.ContainsKey(userId)) _connections[userId] = new HashSet<string>();
            _connections[userId].Add(Context.ConnectionId);
        }

        // Join SignalR groups for all conversations this user is part of
        var convIds = await _db.ChatConversationMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.ConversationId)
            .ToListAsync();
        foreach (var cid in convIds)
            await Groups.AddToGroupAsync(Context.ConnectionId, ConvGroup(cid));

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        lock (_lock)
        {
            if (_connections.TryGetValue(userId, out var c))
            {
                c.Remove(Context.ConnectionId);
                if (c.Count == 0) _connections.Remove(userId);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int conversationId, string content)
    {
        var senderId    = GetUserId();
        var instituteId = GetInstituteId();
        if (senderId == 0 || string.IsNullOrWhiteSpace(content)) return;

        var isMember = await _db.ChatConversationMembers
            .AnyAsync(m => m.ConversationId == conversationId && m.UserId == senderId);
        if (!isMember) return;

        var sender = await _db.Users.FindAsync(senderId);
        if (sender == null) return;

        var msg = new ChatMessage
        {
            ConversationId = conversationId,
            SenderId       = senderId,
            Content        = content.Trim(),
            SentAt         = DateTime.UtcNow,
            InstituteId    = instituteId ?? 0
        };
        _db.ChatMessages.Add(msg);
        await _db.SaveChangesAsync();

        var dto = new ChatMessageDto
        {
            ChatMessageId  = msg.ChatMessageId,
            ConversationId = conversationId,
            SenderId       = senderId,
            SenderName     = sender.FullName,
            Content        = msg.Content,
            SentAt         = msg.SentAt,
            IsRead         = false
        };

        await Clients.Group(ConvGroup(conversationId)).SendAsync("ReceiveMessage", dto);
    }

    // Called when a new group is created so all members join the SignalR group
    public async Task JoinConversation(int conversationId)
    {
        var userId = GetUserId();
        var isMember = await _db.ChatConversationMembers
            .AnyAsync(m => m.ConversationId == conversationId && m.UserId == userId);
        if (isMember)
            await Groups.AddToGroupAsync(Context.ConnectionId, ConvGroup(conversationId));
    }

    private static string ConvGroup(int convId) => $"conv_{convId}";

    private int GetUserId()
    {
        var v = Context.User?.FindFirst("userId")?.Value
             ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(v, out var id) ? id : 0;
    }

    private int? GetInstituteId()
    {
        var v = Context.User?.FindFirst("instituteId")?.Value;
        return int.TryParse(v, out var id) ? id : null;
    }
}
