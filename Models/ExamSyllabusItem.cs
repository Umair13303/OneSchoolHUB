namespace SchoolManagement.API.Models;

/// <summary>
/// Links a formal ExamPaper to Course Content chapters and/or topics.
/// Exactly one of CourseChapterId / CourseTopicId should be set per row.
/// </summary>
public class ExamSyllabusItem : BaseEntity
{
    public int ExamSyllabusItemId { get; set; }
    public int ExamPaperId { get; set; }
    public int? CourseChapterId { get; set; }
    public int? CourseTopicId { get; set; }

    public ExamPaper ExamPaper { get; set; } = null!;
    public CourseChapter? CourseChapter { get; set; }
    public CourseTopic? CourseTopic { get; set; }
}
