namespace SchoolManagement.API.DTOs;

public class ConversationDto
{
    public int      ConversationId   { get; set; }
    public string   Name             { get; set; } = string.Empty;
    public string   ConversationType { get; set; } = "direct";
    public int?     ClassId          { get; set; }
    public string   LastMessage      { get; set; } = string.Empty;
    public DateTime? LastMessageAt   { get; set; }
    public int      UnreadCount      { get; set; }
    public bool     IsOnline         { get; set; }
    public int?     OtherUserId      { get; set; }  // for DMs
    public List<ConversationMemberDto> Members { get; set; } = new();
}

public class ConversationMemberDto
{
    public int    UserId   { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool   IsAdmin  { get; set; }
    public bool   IsOnline { get; set; }
}

public class ChatMessageDto
{
    public int      ChatMessageId  { get; set; }
    public int      ConversationId { get; set; }
    public int      SenderId       { get; set; }
    public string   SenderName     { get; set; } = string.Empty;
    public string   Content        { get; set; } = string.Empty;
    public DateTime SentAt         { get; set; }
    public bool     IsRead         { get; set; }
    public string?  AttachmentUrl  { get; set; }
    public string?  AttachmentName { get; set; }
    public string?  AttachmentType { get; set; }
    public long?    AttachmentSize { get; set; }
}

public class SendMessageDto
{
    public int    ConversationId { get; set; }
    public string Content        { get; set; } = string.Empty;
}

public class CreateGroupDto
{
    public string   Name    { get; set; } = string.Empty;
    public List<int> MemberIds { get; set; } = new();
}

public class StartDmDto
{
    public int TargetUserId { get; set; }
}

public class ChatUserDto
{
    public int    UserId   { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool   IsOnline { get; set; }
}

public class SendMessageWithAttachmentDto
{
    public string? Content        { get; set; }
    public string? AttachmentUrl  { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentType { get; set; }
    public long?   AttachmentSize { get; set; }
}
