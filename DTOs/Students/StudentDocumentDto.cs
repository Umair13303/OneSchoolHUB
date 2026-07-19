namespace SchoolManagement.API.DTOs.Students;

/// <summary>
/// One file attached to a student. Resolved from <c>FileStore</c> rows
/// where <c>EntityType</c> tags the kind of document
/// (e.g. "student-photo", "birth-certificate", "prev-school-cert") and
/// <c>EntityId</c> equals the StudentId.
/// </summary>
public class StudentDocumentDto
{
    public int      FileId       { get; set; }
    public string   EntityType   { get; set; } = string.Empty;
    public string   OriginalName { get; set; } = string.Empty;
    public string?  FileType     { get; set; }
    public long?    SizeBytes    { get; set; }
    public DateTime UploadedAt   { get; set; }

    /// <summary>Public URL the Angular UI uses to download/preview this file (served by the FileServer).</summary>
    public string FileUrl { get; set; } = string.Empty;
}
