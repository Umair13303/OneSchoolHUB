namespace SchoolManagement.API.Models;

public class FileStore
{
    public int FileId { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string StoredName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long? SizeBytes { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }

    /// <summary>Human-typed label for open-ended document lists (e.g. "Degree Certificate", "CNIC Copy"). Null for fixed single-slot types like photos.</summary>
    public string? Label { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public User? UploadedByUser { get; set; }
}
