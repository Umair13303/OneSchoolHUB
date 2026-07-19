namespace SchoolManagement.API.Models;

public class ChatMessage : BaseEntity
{
    public int      ChatMessageId  { get; set; }
    public int      ConversationId { get; set; }
    public int      SenderId       { get; set; }
    public string   Content        { get; set; } = string.Empty;
    public DateTime SentAt         { get; set; } = DateTime.UtcNow;
    public string?  AttachmentUrl  { get; set; }
    public string?  AttachmentName { get; set; }
    public string?  AttachmentType { get; set; }  // "image" | "pdf" | "file"
    public long?    AttachmentSize { get; set; }

    public ChatConversation             Conversation { get; set; } = null!;
    public User                         Sender       { get; set; } = null!;
    public ICollection<ChatMessageRead> Reads        { get; set; } = new List<ChatMessageRead>();
}

public class ChatMessageRead
{
    public int      ReadId    { get; set; }
    public int      MessageId { get; set; }
    public int      UserId    { get; set; }
    public DateTime ReadAt    { get; set; } = DateTime.UtcNow;

    public ChatMessage Message { get; set; } = null!;
    public User        User    { get; set; } = null!;
}
