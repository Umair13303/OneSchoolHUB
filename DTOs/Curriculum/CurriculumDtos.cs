namespace SchoolManagement.API.DTOs.Curriculum;

public class CoursePlanListDto
{
    public int CoursePlanId { get; set; }
    public int AcademicYearId { get; set; }
    public string YearLabel { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public int ChapterCount { get; set; }
    public int TopicCount { get; set; }
    public int CompletedTopicCount { get; set; }
}

public class CoursePlanDetailDto : CoursePlanListDto
{
    public List<CourseChapterDto> Chapters { get; set; } = new();
}

public class CreateCoursePlanDto
{
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCoursePlanDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CourseChapterDto
{
    public int CourseChapterId { get; set; }
    public int CoursePlanId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public List<CourseTopicDto> Topics { get; set; } = new();
    public List<CourseMaterialDto> Materials { get; set; } = new();
}

public class CreateCourseChapterDto
{
    public int CoursePlanId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCourseChapterDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CourseTopicDto
{
    public int CourseTopicId { get; set; }
    public int CourseChapterId { get; set; }
    public int? CoursePlanId { get; set; }
    public int? ClassId { get; set; }
    public int? SubjectId { get; set; }
    public int? AcademicYearId { get; set; }
    public string? ClassName { get; set; }
    public string? SubjectName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public string ProgressStatus { get; set; } = "NotStarted";
    public DateTime? CompletedAt { get; set; }
    public int? CompletedByTeacherId { get; set; }
    public string? CompletedByTeacherName { get; set; }
    public List<CourseMaterialDto> Materials { get; set; } = new();
    public List<CourseTopicActivityDto> Activities { get; set; } = new();
}

public class CreateCourseTopicDto
{
    public int CourseChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCourseTopicDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CourseMaterialDto
{
    public int CourseMaterialId { get; set; }
    public int? CourseChapterId { get; set; }
    public int? CourseTopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Url { get; set; }
    public int? FileStoreId { get; set; }
    public string? FileName { get; set; }
    public int SortOrder { get; set; }
}

public class CreateCourseMaterialDto
{
    public int? CourseChapterId { get; set; }
    public int? CourseTopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaterialType { get; set; } = "Other";
    public string? Content { get; set; }
    public string? Url { get; set; }
    public int? FileStoreId { get; set; }
}

public class UpdateCourseMaterialDto
{
    public string Title { get; set; } = string.Empty;
    public string MaterialType { get; set; } = "Other";
    public string? Content { get; set; }
    public string? Url { get; set; }
    public int? FileStoreId { get; set; }
}

public class CourseTopicActivityDto
{
    public int CourseTopicActivityId { get; set; }
    public int CourseTopicId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? InstructionText { get; set; }
    public int? ReferenceImageFileId { get; set; }
    public string? ReferenceImageName { get; set; }
    public int? ExampleImageFileId { get; set; }
    public string? ExampleImageName { get; set; }
    public int? WorksheetFileId { get; set; }
    public string? WorksheetFileName { get; set; }
    public string? ConfigJson { get; set; }
    public int SortOrder { get; set; }
    public bool CanvasEnabled { get; set; }
}

public class CreateCourseTopicActivityDto
{
    public int CourseTopicId { get; set; }
    public string ActivityType { get; set; } = "Drawing";
    public string Title { get; set; } = string.Empty;
    public string? InstructionText { get; set; }
    public int? ReferenceImageFileId { get; set; }
    public int? ExampleImageFileId { get; set; }
    public int? WorksheetFileId { get; set; }
    public string? ConfigJson { get; set; }
    public bool CanvasEnabled { get; set; }
}

public class UpdateCourseTopicActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string? InstructionText { get; set; }
    public int? ReferenceImageFileId { get; set; }
    public int? ExampleImageFileId { get; set; }
    public int? WorksheetFileId { get; set; }
    public string? ConfigJson { get; set; }
    public bool CanvasEnabled { get; set; }
}

public class ReorderDto
{
    public List<int> OrderedIds { get; set; } = new();
}

public class UpdateTopicProgressDto
{
    /// <summary>NotStarted | InProgress | Completed</summary>
    public string Status { get; set; } = "NotStarted";
}

public class CreateTeachingLogDto
{
    public DateOnly TeachingDate { get; set; }
    public int CourseTopicId { get; set; }
    /// <summary>NewTopic | Revision | Practice | Assessment</summary>
    public string TeachingType { get; set; } = "NewTopic";
    public string? Remarks { get; set; }
    public string? ExtraNotes { get; set; }
}

public class TeachingLogDto
{
    public int CourseTeachingLogId { get; set; }
    public DateOnly TeachingDate { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public int CourseTopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public string? ChapterTitle { get; set; }
    public string TeachingType { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? ExtraNotes { get; set; }
    public int? HomeworkId { get; set; }
    public string? HomeworkTitle { get; set; }
}

public class LinkHomeworkDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly AssignedDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int? FileId { get; set; }
}

public class CurriculumProgressSummaryDto
{
    public int CoursePlanId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string YearLabel { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TopicCount { get; set; }
    public int CompletedTopicCount { get; set; }
    public int InProgressTopicCount { get; set; }
    public int RemainingTopicCount { get; set; }
    public List<ChapterProgressDto> Chapters { get; set; } = new();
}

public class ChapterProgressDto
{
    public int CourseChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int TopicCount { get; set; }
    public int CompletedTopicCount { get; set; }
    public List<TopicProgressDto> Topics { get; set; } = new();
}

public class TopicProgressDto
{
    public int CourseTopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string Status { get; set; } = "NotStarted";
    public DateTime? CompletedAt { get; set; }
    public int? CompletedByTeacherId { get; set; }
    public string? CompletedByTeacherName { get; set; }
}
