namespace SchoolManagement.API.Models;

public class ChatConversation : BaseEntity
{
    public int     ChatConversationId { get; set; }
    public string? Name               { get; set; }
    public string  ConversationType   { get; set; } = "direct"; // direct | group
    public int?    ClassId            { get; set; }

    public ICollection<ChatConversationMember> Members  { get; set; } = new List<ChatConversationMember>();
    public ICollection<ChatMessage>            Messages { get; set; } = new List<ChatMessage>();
}

public class ChatConversationMember
{
    public int      MemberId       { get; set; }
    public int      ConversationId { get; set; }
    public int      UserId         { get; set; }
    public bool     IsAdmin        { get; set; } = false;
    public DateTime JoinedAt       { get; set; } = DateTime.UtcNow;

    public ChatConversation Conversation { get; set; } = null!;
    public User             User         { get; set; } = null!;
}
