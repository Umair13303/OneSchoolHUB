namespace SchoolManagement.API.Models;

/// <summary>
/// Structured early-grade learning activity attached to a CourseTopic.
/// ConfigJson holds type-specific structure (matching pairs, options, etc.).
/// Images/files reuse FileStore. Future student submissions can reference CourseTopicActivityId.
/// </summary>
public class CourseTopicActivity : BaseEntity
{
    public int CourseTopicActivityId { get; set; }
    public int CourseTopicId { get; set; }

    /// <summary>
    /// Drawing | Coloring | Tracing | Matching | ConnectDots | ShapeIdentification |
    /// PictureIdentification | DragArrange | FillBlanks | CircleSelect | ImageQuestion | Worksheet
    /// </summary>
    public string ActivityType { get; set; } = "Drawing";

    public string Title { get; set; } = string.Empty;
    public string? InstructionText { get; set; }

    /// <summary>Main reference / prompt image (FileStore).</summary>
    public int? ReferenceImageFileId { get; set; }

    /// <summary>Optional example / answer key image.</summary>
    public int? ExampleImageFileId { get; set; }

    /// <summary>Worksheet PDF/doc attachment when ActivityType = Worksheet.</summary>
    public int? WorksheetFileId { get; set; }

    /// <summary>Type-specific structured config (JSON).</summary>
    public string? ConfigJson { get; set; }

    public int SortOrder { get; set; }

    /// <summary>When true, future student apps may show a drawing/interaction canvas.</summary>
    public bool CanvasEnabled { get; set; }

    public CourseTopic CourseTopic { get; set; } = null!;
    public FileStore? ReferenceImage { get; set; }
    public FileStore? ExampleImage { get; set; }
    public FileStore? WorksheetFile { get; set; }
}
