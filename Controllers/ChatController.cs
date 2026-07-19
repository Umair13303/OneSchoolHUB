using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs;
using SchoolManagement.API.Hubs;
using SchoolManagement.API.Models;
using System.IO;

namespace SchoolManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHubContext<ChatHub> _hub;

    public ChatController(AppDbContext db, IHubContext<ChatHub> hub)
    {
        _db  = db;
        _hub = hub;
    }

    // ── GET /api/chat/conversations ─────────────────────────────────────────
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var me = GetUserId();

        var convs = await _db.ChatConversationMembers
            .Where(m => m.UserId == me)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Members)
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.Role)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Messages)
                    .ThenInclude(msg => msg.Reads)
            .Select(m => m.Conversation)
            .ToListAsync();

        var dtos = convs.Select(c =>
        {
            var lastMsg = c.Messages.OrderByDescending(x => x.SentAt).FirstOrDefault();
            var unread  = c.Messages.Count(x => x.SenderId != me && !x.Reads.Any(r => r.UserId == me));

            var otherMember = c.ConversationType == "direct"
                ? c.Members.FirstOrDefault(m => m.UserId != me)
                : null;

            var name = c.ConversationType == "direct"
                ? otherMember?.User?.FullName ?? "Unknown"
                : (c.Name ?? "Group");

            return new ConversationDto
            {
                ConversationId   = c.ChatConversationId,
                Name             = name,
                ConversationType = c.ConversationType,
                ClassId          = c.ClassId,
                LastMessage      = lastMsg?.Content ?? "",
                LastMessageAt    = lastMsg?.SentAt,
                UnreadCount      = unread,
                IsOnline         = otherMember != null && ChatHub.IsOnline(otherMember.UserId),
                OtherUserId      = otherMember?.UserId,
                Members          = c.Members.Select(cm => new ConversationMemberDto
                {
                    UserId   = cm.UserId,
                    FullName = cm.User?.FullName ?? "",
                    RoleName = cm.User?.Role?.RoleName ?? "",
                    IsAdmin  = cm.IsAdmin,
                    IsOnline = ChatHub.IsOnline(cm.UserId)
                }).ToList()
            };
        })
        .OrderByDescending(c => c.LastMessageAt)
        .ToList();

        return Ok(dtos);
    }

    // ── POST /api/chat/dm ───────────────────────────────────────────────────
    [HttpPost("dm")]
    public async Task<IActionResult> StartDm([FromBody] StartDmDto dto)
    {
        var me = GetUserId();
        var instituteId = GetInstituteId();

        // Check if DM already exists between these two users
        var myConvIds = await _db.ChatConversationMembers
            .Where(m => m.UserId == me)
            .Select(m => m.ConversationId)
            .ToListAsync();

        var existing = await _db.ChatConversationMembers
            .Where(m => m.UserId == dto.TargetUserId &&
                   myConvIds.Contains(m.ConversationId) &&
                   m.Conversation.ConversationType == "direct")
            .Select(m => m.ConversationId)
            .FirstOrDefaultAsync();

        if (existing != 0)
            return Ok(new { conversationId = existing });

        var conv = new ChatConversation { ConversationType = "direct", InstituteId = instituteId };
        _db.ChatConversations.Add(conv);
        await _db.SaveChangesAsync();

        _db.ChatConversationMembers.AddRange(
            new ChatConversationMember { ConversationId = conv.ChatConversationId, UserId = me,             IsAdmin = true },
            new ChatConversationMember { ConversationId = conv.ChatConversationId, UserId = dto.TargetUserId }
        );
        await _db.SaveChangesAsync();

        return Ok(new { conversationId = conv.ChatConversationId });
    }

    // ── POST /api/chat/groups ───────────────────────────────────────────────
    [HttpPost("groups")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        var me = GetUserId();
        var instituteId = GetInstituteId();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Group name is required.");

        var conv = new ChatConversation
        {
            Name             = dto.Name.Trim(),
            ConversationType = "group",
            InstituteId      = instituteId
        };
        _db.ChatConversations.Add(conv);
        await _db.SaveChangesAsync();

        var memberIds = dto.MemberIds.Distinct().ToList();
        if (!memberIds.Contains(me)) memberIds.Add(me);

        _db.ChatConversationMembers.AddRange(memberIds.Select(uid => new ChatConversationMember
        {
            ConversationId = conv.ChatConversationId,
            UserId         = uid,
            IsAdmin        = uid == me
        }));
        await _db.SaveChangesAsync();

        // Tell all online members to join the SignalR group
        foreach (var uid in memberIds)
            await _hub.Clients.User(uid.ToString()).SendAsync("JoinConversation", conv.ChatConversationId);

        return Ok(new { conversationId = conv.ChatConversationId });
    }

    // ── POST /api/chat/groups/broadcast ─────────────────────────────────────
    // Auto-creates or finds a staff / class-parents group and returns conversationId
    [HttpPost("groups/broadcast")]
    public async Task<IActionResult> GetOrCreateBroadcastGroup([FromBody] BroadcastGroupDto dto)
    {
        var me = GetUserId();
        var instituteId = GetInstituteId();

        if (dto.Type == "staff")
        {
            var existing = await _db.ChatConversations
                .Where(c => c.ConversationType == "group" && c.Name == "All Staff" && c.InstituteId == instituteId)
                .FirstOrDefaultAsync();

            if (existing != null)
                return Ok(new { conversationId = existing.ChatConversationId });

            var staffIds = await _db.Users
                .Where(u => !u.IsDeleted && u.InstituteId == instituteId &&
                       u.Role != null && u.Role.RoleName != "student" && u.Role.RoleName != "parent")
                .Select(u => u.UserId)
                .ToListAsync();

            var conv = new ChatConversation { Name = "All Staff", ConversationType = "group", InstituteId = instituteId };
            _db.ChatConversations.Add(conv);
            await _db.SaveChangesAsync();

            _db.ChatConversationMembers.AddRange(staffIds.Select(uid => new ChatConversationMember
            {
                ConversationId = conv.ChatConversationId,
                UserId         = uid,
                IsAdmin        = uid == me
            }));
            await _db.SaveChangesAsync();

            return Ok(new { conversationId = conv.ChatConversationId });
        }

        if (dto.Type == "class-parents" && dto.ClassId.HasValue)
        {
            var existing = await _db.ChatConversations
                .Where(c => c.ConversationType == "group" && c.ClassId == dto.ClassId && c.InstituteId == instituteId)
                .FirstOrDefaultAsync();

            if (existing != null)
                return Ok(new { conversationId = existing.ChatConversationId });

            var className = await _db.Classes
                .Where(c => c.ClassId == dto.ClassId)
                .Select(c => c.ClassName)
                .FirstOrDefaultAsync() ?? $"Class {dto.ClassId}";

            // Parents of enrolled students via StudentGuardian link
            var studentIds = await _db.StudentClassEnrollments
                .Where(e => e.ClassId == dto.ClassId)
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync();

            var parentIds = await _db.StudentGuardians
                .Where(g => studentIds.Contains(g.StudentId) && g.UserId != null && !g.IsDeleted)
                .Select(g => g.UserId!.Value)
                .Distinct()
                .ToListAsync();

            var conv = new ChatConversation
            {
                Name             = $"{className} - Parents",
                ConversationType = "group",
                ClassId          = dto.ClassId,
                InstituteId      = instituteId
            };
            _db.ChatConversations.Add(conv);
            await _db.SaveChangesAsync();

            var memberIds = parentIds.ToList();
            if (!memberIds.Contains(me)) memberIds.Add(me);

            _db.ChatConversationMembers.AddRange(memberIds.Select(uid => new ChatConversationMember
            {
                ConversationId = conv.ChatConversationId,
                UserId         = uid,
                IsAdmin        = uid == me
            }));
            await _db.SaveChangesAsync();

            return Ok(new { conversationId = conv.ChatConversationId });
        }

        return BadRequest("Invalid broadcast type.");
    }

    // ── GET /api/chat/conversations/{id}/messages ───────────────────────────
    [HttpGet("conversations/{id:int}/messages")]
    public async Task<IActionResult> GetMessages(int id, [FromQuery] int page = 1, [FromQuery] int size = 50)
    {
        var me = GetUserId();

        var isMember = await _db.ChatConversationMembers.AnyAsync(m => m.ConversationId == id && m.UserId == me);
        if (!isMember) return Forbid();

        var msgs = await _db.ChatMessages
            .Where(m => m.ConversationId == id)
            .Include(m => m.Sender)
            .Include(m => m.Reads)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        // Mark unread as read
        var unreadIds = msgs
            .Where(m => m.SenderId != me && !m.Reads.Any(r => r.UserId == me))
            .Select(m => m.ChatMessageId)
            .ToList();
        if (unreadIds.Any())
        {
            _db.ChatMessageReads.AddRange(unreadIds.Select(mid => new ChatMessageRead { MessageId = mid, UserId = me }));
            await _db.SaveChangesAsync();
        }

        var dtos = msgs.OrderBy(m => m.SentAt).Select(m => new ChatMessageDto
        {
            ChatMessageId  = m.ChatMessageId,
            ConversationId = m.ConversationId,
            SenderId       = m.SenderId,
            SenderName     = m.Sender?.FullName ?? "",
            Content        = m.Content,
            SentAt         = m.SentAt,
            IsRead         = m.Reads.Any(r => r.UserId != m.SenderId),
            AttachmentUrl  = m.AttachmentUrl,
            AttachmentName = m.AttachmentName,
            AttachmentType = m.AttachmentType,
            AttachmentSize = m.AttachmentSize
        });

        return Ok(dtos);
    }

    // ── GET /api/chat/unread ────────────────────────────────────────────────
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var me = GetUserId();
        var myConvIds = await _db.ChatConversationMembers
            .Where(m => m.UserId == me).Select(m => m.ConversationId).ToListAsync();

        var count = await _db.ChatMessages
            .Where(m => myConvIds.Contains(m.ConversationId) &&
                   m.SenderId != me && !m.Reads.Any(r => r.UserId == me))
            .CountAsync();

        return Ok(new { count });
    }

    // ── GET /api/chat/users ─────────────────────────────────────────────────
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var me = GetUserId();
        var instituteId = GetInstituteId();

        var users = await _db.Users
            .Where(u => !u.IsDeleted && u.InstituteId == instituteId && u.UserId != me)
            .Include(u => u.Role)
            .Select(u => new ChatUserDto
            {
                UserId   = u.UserId,
                FullName = u.FullName,
                RoleName = u.Role != null ? u.Role.RoleName : "",
                IsOnline = ChatHub.IsOnline(u.UserId)
            })
            .ToListAsync();

        return Ok(users);
    }

    // ── POST /api/chat/upload ───────────────────────────────────────────────
    [HttpPost("upload")]
    [RequestSizeLimit(20 * 1024 * 1024)] // 20 MB
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file.");

        var ext       = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed   = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip" };
        if (!allowed.Contains(ext)) return BadRequest("File type not allowed.");

        var instituteId = GetInstituteId() ?? 0;
        var folder      = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "chat-attachments", instituteId.ToString());
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(folder, fileName);
        await using (var stream = System.IO.File.Create(fullPath))
            await file.CopyToAsync(stream);

        var url            = $"/chat-attachments/{instituteId}/{fileName}";
        var isImage        = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext);
        var attachmentType = isImage ? "image" : ext == ".pdf" ? "pdf" : "file";

        return Ok(new { url, name = file.FileName, type = attachmentType, size = file.Length });
    }

    // ── POST /api/chat/conversations/{id}/send ─────────────────────────────
    // REST fallback for sending messages with attachments
    [HttpPost("conversations/{id:int}/send")]
    public async Task<IActionResult> SendMessage(int id, [FromBody] SendMessageWithAttachmentDto dto)
    {
        var me = GetUserId();
        var member = await _db.ChatConversationMembers
            .FirstOrDefaultAsync(m => m.ConversationId == id && m.UserId == me);
        if (member == null) return Forbid();

        var sender = await _db.Users.FindAsync(me);

        var msg = new ChatMessage
        {
            ConversationId = id,
            SenderId       = me,
            Content        = dto.Content ?? "",
            SentAt         = DateTime.UtcNow,
            AttachmentUrl  = dto.AttachmentUrl,
            AttachmentName = dto.AttachmentName,
            AttachmentType = dto.AttachmentType,
            AttachmentSize = dto.AttachmentSize,
            InstituteId    = GetInstituteId() ?? 0
        };
        _db.ChatMessages.Add(msg);
        await _db.SaveChangesAsync();

        var msgDto = new ChatMessageDto
        {
            ChatMessageId  = msg.ChatMessageId,
            ConversationId = msg.ConversationId,
            SenderId       = msg.SenderId,
            SenderName     = sender?.FullName ?? "",
            Content        = msg.Content,
            SentAt         = msg.SentAt,
            IsRead         = false,
            AttachmentUrl  = msg.AttachmentUrl,
            AttachmentName = msg.AttachmentName,
            AttachmentType = msg.AttachmentType,
            AttachmentSize = msg.AttachmentSize
        };

        await _hub.Clients.Group($"conv_{id}").SendAsync("ReceiveMessage", msgDto);
        return Ok(msgDto);
    }

    // ── DELETE /api/chat/conversations/{id} ────────────────────────────────
    [HttpDelete("conversations/{id:int}")]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        var me = GetUserId();

        var conv = await _db.ChatConversations
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.ChatConversationId == id);

        if (conv == null) return NotFound();

        var member = conv.Members.FirstOrDefault(m => m.UserId == me);
        if (member == null) return Forbid();

        // For groups: only admin can delete; for DMs: either member can delete
        if (conv.ConversationType == "group" && !member.IsAdmin)
            return Forbid();

        conv.IsDeleted = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ── DELETE /api/chat/messages/{id} ─────────────────────────────────────
    [HttpDelete("messages/{id:int}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var me = GetUserId();

        var msg = await _db.ChatMessages.FindAsync(id);
        if (msg == null) return NotFound();
        if (msg.SenderId != me) return Forbid();

        msg.IsDeleted = true;
        await _db.SaveChangesAsync();

        // Notify members that message was deleted
        await _hub.Clients.Group($"conv_{msg.ConversationId}")
            .SendAsync("MessageDeleted", new { messageId = id, conversationId = msg.ConversationId });

        return NoContent();
    }

    private int GetUserId()
    {
        var v = User.FindFirst("userId")?.Value
             ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(v, out var id) ? id : 0;
    }

    private int? GetInstituteId()
    {
        var v = User.FindFirst("instituteId")?.Value;
        return int.TryParse(v, out var id) ? id : null;
    }
}

public class BroadcastGroupDto
{
    public string Type    { get; set; } = string.Empty;
    public int?   ClassId { get; set; }
}
