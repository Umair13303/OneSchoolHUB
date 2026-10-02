namespace SchoolManagement.API.Models;

/// <summary>
/// Material attached at chapter or topic level.
/// Exactly one of CourseChapterId / CourseTopicId should be set.
/// Files reuse FileStore (EntityType = CourseMaterial).
/// </summary>
public class CourseMaterial : BaseEntity
{
    public int CourseMaterialId { get; set; }
    public int? CourseChapterId { get; set; }
    public int? CourseTopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>Pdf | Booklet | Worksheet | Notes | Image | Link | Other</summary>
    public string MaterialType { get; set; } = "Other";
    /// <summary>Written notes / lesson text (plain text; use for short explanations).</summary>
    public string? Content { get; set; }
    public string? Url { get; set; }
    public int? FileStoreId { get; set; }
    public int SortOrder { get; set; }

    public CourseChapter? CourseChapter { get; set; }
    public CourseTopic? CourseTopic { get; set; }
    public FileStore? FileStore { get; set; }
}
