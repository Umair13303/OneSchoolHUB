using SchoolManagement.API.Models;

namespace SchoolManagement.API.DTOs.Assessment;

public class AssessmentResultLookupDto
{
    public int AssessmentResultLookupId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string LabelEn { get; set; } = string.Empty;
    public string LabelUr { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class ClassRosterStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
}

public class ClassAssessmentResultDto
{
    public int ClassAssessmentResultId { get; set; }
    public int ClassAssessmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public decimal? ObtainedMarks { get; set; }
    public string? Status { get; set; }
    public string? StatusLabelEn { get; set; }
    public string? StatusLabelUr { get; set; }
    public string? Remarks { get; set; }
}

public class ClassAssessmentDto
{
    public int ClassAssessmentId { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public int? CourseTopicId { get; set; }
    public string? TopicTitle { get; set; }
    public int? CourseTeachingLogId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateOnly AssessmentDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AssessmentType { get; set; } = string.Empty;
    public int? TotalMarks { get; set; }
    public int? TotalQuestions { get; set; }
    public string? Notes { get; set; }
    public int ResultCount { get; set; }
    public List<ClassAssessmentResultDto> Results { get; set; } = new();
}

public class CreateClassAssessmentDto
{
    public int? CourseTopicId { get; set; }
    public int? CourseTeachingLogId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int AcademicYearId { get; set; }
    public DateOnly AssessmentDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AssessmentType { get; set; } = "Quiz";
    public int? TotalMarks { get; set; }
    public int? TotalQuestions { get; set; }
    public string? Notes { get; set; }
}

public class UpdateClassAssessmentDto
{
    public DateOnly? AssessmentDate { get; set; }
    public string? Title { get; set; }
    public string? AssessmentType { get; set; }
    public int? TotalMarks { get; set; }
    public int? TotalQuestions { get; set; }
    public string? Notes { get; set; }
}

public class UpsertClassAssessmentResultItemDto
{
    public int StudentId { get; set; }
    public decimal? ObtainedMarks { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }
}

public class BulkSaveClassAssessmentResultsDto
{
    public List<UpsertClassAssessmentResultItemDto> Results { get; set; } = new();
}

public class StudentTopicPerformanceDto
{
    public int StudentTopicPerformanceId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public int CourseTopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateOnly PerformanceDate { get; set; }
    public string PerformanceType { get; set; } = string.Empty;
    public string ResultStatus { get; set; } = string.Empty;
    public string? ResultStatusLabelEn { get; set; }
    public string? ResultStatusLabelUr { get; set; }
    public string? Remarks { get; set; }
}

public class CreateStudentTopicPerformanceItemDto
{
    public int StudentId { get; set; }
    public string ResultStatus { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class BulkCreateStudentTopicPerformanceDto
{
    public int CourseTopicId { get; set; }
    public DateOnly PerformanceDate { get; set; }
    public string PerformanceType { get; set; } = "OralRecitation";
    public List<CreateStudentTopicPerformanceItemDto> Items { get; set; } = new();
}

public class TopicAssessmentSummaryDto
{
    public int CourseTopicId { get; set; }
    public int ClassAssessmentCount { get; set; }
    public int StudentPerformanceCount { get; set; }
    public List<ClassAssessmentDto> RecentAssessments { get; set; } = new();
    public List<StudentTopicPerformanceDto> RecentPerformances { get; set; } = new();
}

public class StudentPerformanceTimelineItemDto
{
    public string Source { get; set; } = string.Empty; // Teaching | ClassAssessment | Sabaq | Exam | Homework
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public string? Result { get; set; }
    public string? TeacherName { get; set; }
    public int? CourseTopicId { get; set; }
    public string? TopicTitle { get; set; }
    public int? RelatedId { get; set; }
}

public class ExamSyllabusItemDto
{
    public int ExamSyllabusItemId { get; set; }
    public int ExamPaperId { get; set; }
    public int? CourseChapterId { get; set; }
    public string? ChapterTitle { get; set; }
    public int? CourseTopicId { get; set; }
    public string? TopicTitle { get; set; }
}

public class ExamSyllabusDto
{
    public int ExamPaperId { get; set; }
    public int? CoursePlanId { get; set; }
    public string? CoursePlanTitle { get; set; }
    public string? SyllabusNote { get; set; }
    public List<ExamSyllabusItemDto> Items { get; set; } = new();
}

public class SaveExamSyllabusItemDto
{
    public int? CourseChapterId { get; set; }
    public int? CourseTopicId { get; set; }
}

public class SaveExamSyllabusDto
{
    public List<SaveExamSyllabusItemDto> Items { get; set; } = new();
}

public class QuestionBankOptionDto
{
    public int QuestionBankOptionId { get; set; }
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}

public class QuestionBankItemDto
{
    public int QuestionBankItemId { get; set; }
    public int CourseTopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public int QuestionTypeId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int Marks { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool? IsTrue { get; set; }
    public string? QuestionNote { get; set; }
    public bool IsActive { get; set; }
    public List<QuestionBankOptionDto> Options { get; set; } = new();
}

public class CreateQuestionBankOptionDto
{
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}

public class CreateQuestionBankItemDto
{
    public int CourseTopicId { get; set; }
    public QuestionType QuestionType { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int Marks { get; set; } = 1;
    public string? CorrectAnswer { get; set; }
    public bool? IsTrue { get; set; }
    public string? QuestionNote { get; set; }
    public List<CreateQuestionBankOptionDto> Options { get; set; } = new();
}

public class UpdateQuestionBankItemDto
{
    public string? QuestionText { get; set; }
    public string? Language { get; set; }
    public int? Marks { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool? IsTrue { get; set; }
    public string? QuestionNote { get; set; }
    public bool? IsActive { get; set; }
    public List<CreateQuestionBankOptionDto>? Options { get; set; }
}

public class CopyQuestionBankToPaperDto
{
    public int ExamPaperId { get; set; }
    public int? ExamPaperSectionId { get; set; }
    public List<int> QuestionBankItemIds { get; set; } = new();
}
