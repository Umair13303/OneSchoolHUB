using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Assessment;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IClassroomAssessmentService
{
    Task<List<AssessmentResultLookupDto>> GetResultLookupsAsync();
    Task<List<ClassRosterStudentDto>> GetClassRosterAsync(int classId, int academicYearId);

    Task<ClassAssessmentDto> CreateAssessmentAsync(CreateClassAssessmentDto dto, int teacherId, bool isTeacherOnly);
    Task<ClassAssessmentDto?> GetAssessmentAsync(int id);
    Task<List<ClassAssessmentDto>> ListAssessmentsAsync(int? classId, int? subjectId, int? topicId, DateOnly? from, DateOnly? to);
    Task<bool> UpdateAssessmentAsync(int id, UpdateClassAssessmentDto dto, int userId, bool isTeacherOnly);
    Task<ClassAssessmentDto> SaveAssessmentResultsAsync(int assessmentId, BulkSaveClassAssessmentResultsDto dto, int userId, bool isTeacherOnly);

    Task<List<StudentTopicPerformanceDto>> BulkCreatePerformancesAsync(BulkCreateStudentTopicPerformanceDto dto, int teacherId, bool isTeacherOnly);
    Task<List<StudentTopicPerformanceDto>> ListPerformancesAsync(int? topicId, int? studentId, int? classId, DateOnly? from, DateOnly? to);

    Task<TopicAssessmentSummaryDto> GetTopicAssessmentSummaryAsync(int topicId);
    Task<List<StudentPerformanceTimelineItemDto>> GetStudentPerformanceTimelineAsync(int studentId, int? academicYearId, int? subjectId);
}

public class ClassroomAssessmentService : IClassroomAssessmentService
{
    private static readonly HashSet<string> ValidAssessmentTypes = new(StringComparer.OrdinalIgnoreCase)
        { "Quiz", "ClassTest", "OralTest", "Other" };
    private static readonly HashSet<string> ValidPerformanceTypes = new(StringComparer.OrdinalIgnoreCase)
        { "OralRecitation", "Participation", "Other" };

    private readonly AppDbContext _db;

    public ClassroomAssessmentService(AppDbContext db) => _db = db;

    public async Task<List<AssessmentResultLookupDto>> GetResultLookupsAsync()
    {
        return await _db.AssessmentResultLookups.AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.SortOrder)
            .Select(l => new AssessmentResultLookupDto
            {
                AssessmentResultLookupId = l.AssessmentResultLookupId,
                Code = l.Code,
                LabelEn = l.LabelEn,
                LabelUr = l.LabelUr,
                SortOrder = l.SortOrder
            }).ToListAsync();
    }

    public async Task<List<ClassRosterStudentDto>> GetClassRosterAsync(int classId, int academicYearId)
    {
        return await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.ClassId == classId && e.AcademicYearId == academicYearId
                        && e.Status == "Active" && !e.IsDeleted)
            .Include(e => e.Student)
            .OrderBy(e => e.Student.FirstName).ThenBy(e => e.Student.LastName)
            .Select(e => new ClassRosterStudentDto
            {
                StudentId = e.StudentId,
                StudentName = (e.Student.FirstName + " " + e.Student.LastName).Trim(),
                AdmissionNo = e.Student.AdmissionNo
            }).ToListAsync();
    }

    public async Task<ClassAssessmentDto> CreateAssessmentAsync(CreateClassAssessmentDto dto, int teacherId, bool isTeacherOnly)
    {
        var type = NormalizeAssessmentType(dto.AssessmentType);
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required.");

        CourseTopic? topic = null;
        if (dto.CourseTopicId is int tid)
        {
            topic = await _db.CourseTopics
                .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
                .FirstOrDefaultAsync(t => t.CourseTopicId == tid && !t.IsDeleted)
                ?? throw new ArgumentException("Topic not found.");
            var plan = topic.CourseChapter.CoursePlan;
            if (plan.IsDeleted || plan.Status != "Published")
                throw new ArgumentException("Assessments can only be recorded against Published course topics.");
            dto.ClassId = plan.ClassId;
            dto.SubjectId = plan.SubjectId;
            dto.AcademicYearId = plan.AcademicYearId;
        }

        await EnsureTeacherAccessAsync(teacherId, dto.ClassId, dto.SubjectId, isTeacherOnly);

        if (dto.CourseTeachingLogId is int logId)
        {
            var log = await _db.CourseTeachingLogs.FirstOrDefaultAsync(l => l.CourseTeachingLogId == logId && !l.IsDeleted)
                ?? throw new ArgumentException("Teaching log not found.");
            if (log.ClassId != dto.ClassId || log.SubjectId != dto.SubjectId)
                throw new ArgumentException("Teaching log does not match class/subject.");
        }

        var entity = new ClassAssessment
        {
            ClassId = dto.ClassId,
            SubjectId = dto.SubjectId,
            AcademicYearId = dto.AcademicYearId,
            CourseTopicId = dto.CourseTopicId,
            CourseTeachingLogId = dto.CourseTeachingLogId,
            TeacherId = teacherId,
            AssessmentDate = dto.AssessmentDate,
            Title = dto.Title.Trim(),
            AssessmentType = type,
            TotalMarks = dto.TotalMarks,
            TotalQuestions = dto.TotalQuestions,
            Notes = dto.Notes?.Trim(),
            CreatedBy = teacherId,
            CreatedAt = DateTime.UtcNow
        };
        _db.ClassAssessments.Add(entity);
        await _db.SaveChangesAsync();
        return (await GetAssessmentAsync(entity.ClassAssessmentId))!;
    }

    public async Task<ClassAssessmentDto?> GetAssessmentAsync(int id)
    {
        var a = await _db.ClassAssessments.AsNoTracking()
            .Include(x => x.Class).Include(x => x.Subject).Include(x => x.Teacher)
            .Include(x => x.CourseTopic)
            .Include(x => x.Results).ThenInclude(r => r.Student)
            .FirstOrDefaultAsync(x => x.ClassAssessmentId == id && !x.IsDeleted);
        return a is null ? null : await MapAssessmentAsync(a, includeResults: true);
    }

    public async Task<List<ClassAssessmentDto>> ListAssessmentsAsync(
        int? classId, int? subjectId, int? topicId, DateOnly? from, DateOnly? to)
    {
        var q = _db.ClassAssessments.AsNoTracking()
            .Include(x => x.Class).Include(x => x.Subject).Include(x => x.Teacher)
            .Include(x => x.CourseTopic)
            .Include(x => x.Results)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (classId is > 0) q = q.Where(x => x.ClassId == classId);
        if (subjectId is > 0) q = q.Where(x => x.SubjectId == subjectId);
        if (topicId is > 0) q = q.Where(x => x.CourseTopicId == topicId);
        if (from is not null) q = q.Where(x => x.AssessmentDate >= from);
        if (to is not null) q = q.Where(x => x.AssessmentDate <= to);

        var list = await q.OrderByDescending(x => x.AssessmentDate).ThenByDescending(x => x.CreatedAt).ToListAsync();
        var result = new List<ClassAssessmentDto>();
        foreach (var a in list)
            result.Add(await MapAssessmentAsync(a, includeResults: false));
        return result;
    }

    public async Task<bool> UpdateAssessmentAsync(int id, UpdateClassAssessmentDto dto, int userId, bool isTeacherOnly)
    {
        var a = await _db.ClassAssessments.FirstOrDefaultAsync(x => x.ClassAssessmentId == id && !x.IsDeleted);
        if (a is null) return false;
        await EnsureTeacherAccessAsync(userId, a.ClassId, a.SubjectId, isTeacherOnly);

        if (dto.AssessmentDate is not null) a.AssessmentDate = dto.AssessmentDate.Value;
        if (dto.Title is not null) a.Title = dto.Title.Trim();
        if (dto.AssessmentType is not null) a.AssessmentType = NormalizeAssessmentType(dto.AssessmentType);
        if (dto.TotalMarks is not null) a.TotalMarks = dto.TotalMarks;
        if (dto.TotalQuestions is not null) a.TotalQuestions = dto.TotalQuestions;
        if (dto.Notes is not null) a.Notes = dto.Notes.Trim();
        a.UpdatedBy = userId;
        a.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ClassAssessmentDto> SaveAssessmentResultsAsync(
        int assessmentId, BulkSaveClassAssessmentResultsDto dto, int userId, bool isTeacherOnly)
    {
        var a = await _db.ClassAssessments
            .Include(x => x.Results)
            .FirstOrDefaultAsync(x => x.ClassAssessmentId == assessmentId && !x.IsDeleted)
            ?? throw new ArgumentException("Assessment not found.");

        await EnsureTeacherAccessAsync(userId, a.ClassId, a.SubjectId, isTeacherOnly);

        var lookups = await GetLookupMapAsync();
        var enrolled = (await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.ClassId == a.ClassId && e.AcademicYearId == a.AcademicYearId
                        && e.Status == "Active" && !e.IsDeleted)
            .Select(e => e.StudentId).ToListAsync()).ToHashSet();

        foreach (var item in dto.Results)
        {
            if (!enrolled.Contains(item.StudentId))
                throw new ArgumentException($"Student {item.StudentId} is not enrolled in this class/year.");
            if (!string.IsNullOrWhiteSpace(item.Status) && !lookups.ContainsKey(item.Status))
                throw new ArgumentException($"Invalid status code: {item.Status}");

            var existing = a.Results.FirstOrDefault(r => r.StudentId == item.StudentId && !r.IsDeleted);
            if (existing is null)
            {
                existing = new ClassAssessmentResult
                {
                    ClassAssessmentId = a.ClassAssessmentId,
                    StudentId = item.StudentId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.ClassAssessmentResults.Add(existing);
                a.Results.Add(existing);
            }
            existing.ObtainedMarks = item.ObtainedMarks;
            existing.Status = string.IsNullOrWhiteSpace(item.Status) ? null : item.Status.Trim();
            existing.Remarks = item.Remarks?.Trim();
            existing.UpdatedBy = userId;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.IsDeleted = false;
        }

        a.UpdatedBy = userId;
        a.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return (await GetAssessmentAsync(assessmentId))!;
    }

    public async Task<List<StudentTopicPerformanceDto>> BulkCreatePerformancesAsync(
        BulkCreateStudentTopicPerformanceDto dto, int teacherId, bool isTeacherOnly)
    {
        if (dto.Items.Count == 0)
            throw new ArgumentException("At least one student performance item is required.");

        var type = NormalizePerformanceType(dto.PerformanceType);
        var topic = await _db.CourseTopics
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(t => t.CourseTopicId == dto.CourseTopicId && !t.IsDeleted)
            ?? throw new ArgumentException("Topic not found.");

        var plan = topic.CourseChapter.CoursePlan;
        if (plan.IsDeleted || plan.Status != "Published")
            throw new ArgumentException("Performance can only be recorded against Published course topics.");

        await EnsureTeacherAccessAsync(teacherId, plan.ClassId, plan.SubjectId, isTeacherOnly);

        var lookups = await GetLookupMapAsync();
        var enrolled = (await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.ClassId == plan.ClassId && e.AcademicYearId == plan.AcademicYearId
                        && e.Status == "Active" && !e.IsDeleted)
            .Select(e => e.StudentId).ToListAsync()).ToHashSet();

        foreach (var item in dto.Items)
        {
            if (!enrolled.Contains(item.StudentId))
                throw new ArgumentException($"Student {item.StudentId} is not enrolled in this class/year.");
            if (string.IsNullOrWhiteSpace(item.ResultStatus) || !lookups.ContainsKey(item.ResultStatus))
                throw new ArgumentException($"Invalid result status: {item.ResultStatus}");

            // Append-only: always insert a new row — never update prior history.
            var row = new StudentTopicPerformance
            {
                StudentId = item.StudentId,
                CourseTopicId = topic.CourseTopicId,
                ClassId = plan.ClassId,
                SubjectId = plan.SubjectId,
                AcademicYearId = plan.AcademicYearId,
                TeacherId = teacherId,
                PerformanceDate = dto.PerformanceDate,
                PerformanceType = type,
                ResultStatus = item.ResultStatus.Trim(),
                Remarks = item.Remarks?.Trim(),
                CreatedBy = teacherId,
                CreatedAt = DateTime.UtcNow
            };
            _db.StudentTopicPerformances.Add(row);
        }

        await _db.SaveChangesAsync();
        return (await ListPerformancesAsync(topic.CourseTopicId, null, null, dto.PerformanceDate, dto.PerformanceDate))
            .Where(r => r.TeacherId == teacherId)
            .Take(dto.Items.Count)
            .ToList();
    }

    public async Task<List<StudentTopicPerformanceDto>> ListPerformancesAsync(
        int? topicId, int? studentId, int? classId, DateOnly? from, DateOnly? to)
    {
        var q = _db.StudentTopicPerformances.AsNoTracking()
            .Include(p => p.Student).Include(p => p.Teacher).Include(p => p.CourseTopic).Include(p => p.Subject)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (topicId is > 0) q = q.Where(p => p.CourseTopicId == topicId);
        if (studentId is > 0) q = q.Where(p => p.StudentId == studentId);
        if (classId is > 0) q = q.Where(p => p.ClassId == classId);
        if (from is not null) q = q.Where(p => p.PerformanceDate >= from);
        if (to is not null) q = q.Where(p => p.PerformanceDate <= to);

        var rows = await q.OrderByDescending(p => p.PerformanceDate).ThenByDescending(p => p.CreatedAt).ToListAsync();
        var lookups = await GetLookupMapAsync();
        return rows.Select(p => MapPerformance(p, lookups)).ToList();
    }

    public async Task<TopicAssessmentSummaryDto> GetTopicAssessmentSummaryAsync(int topicId)
    {
        var assessments = await ListAssessmentsAsync(null, null, topicId, null, null);
        var performances = await ListPerformancesAsync(topicId, null, null, null, null);
        return new TopicAssessmentSummaryDto
        {
            CourseTopicId = topicId,
            ClassAssessmentCount = assessments.Count,
            StudentPerformanceCount = performances.Count,
            RecentAssessments = assessments.Take(10).ToList(),
            RecentPerformances = performances.Take(30).ToList()
        };
    }

    public async Task<List<StudentPerformanceTimelineItemDto>> GetStudentPerformanceTimelineAsync(
        int studentId, int? academicYearId, int? subjectId)
    {
        var items = new List<StudentPerformanceTimelineItemDto>();

        var enrollments = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.StudentId == studentId && e.Status == "Active" && !e.IsDeleted)
            .ToListAsync();
        if (academicYearId is > 0)
            enrollments = enrollments.Where(e => e.AcademicYearId == academicYearId).ToList();
        var classIds = enrollments.Select(e => e.ClassId).Distinct().ToList();

        // Teaching (class-level context for student's class)
        var teachingQ = _db.CourseTeachingLogs.AsNoTracking()
            .Include(l => l.Teacher).Include(l => l.CourseTopic).Include(l => l.Subject)
            .Where(l => !l.IsDeleted && classIds.Contains(l.ClassId));
        if (academicYearId is > 0) teachingQ = teachingQ.Where(l => l.AcademicYearId == academicYearId);
        if (subjectId is > 0) teachingQ = teachingQ.Where(l => l.SubjectId == subjectId);
        foreach (var l in await teachingQ.OrderByDescending(x => x.TeachingDate).Take(50).ToListAsync())
        {
            items.Add(new StudentPerformanceTimelineItemDto
            {
                Source = "Teaching",
                Date = l.TeachingDate,
                Title = $"{l.Subject.SubjectName} — {l.CourseTopic.Title}",
                Detail = l.Remarks,
                Result = l.TeachingType,
                TeacherName = l.Teacher.FullName,
                CourseTopicId = l.CourseTopicId,
                TopicTitle = l.CourseTopic.Title,
                RelatedId = l.CourseTeachingLogId
            });
        }

        // Sabaq / topic performance
        var perf = await ListPerformancesAsync(null, studentId, null, null, null);
        if (academicYearId is > 0) perf = perf.Where(p => p.AcademicYearId == academicYearId).ToList();
        if (subjectId is > 0) perf = perf.Where(p => p.SubjectId == subjectId).ToList();
        foreach (var p in perf)
        {
            items.Add(new StudentPerformanceTimelineItemDto
            {
                Source = "Sabaq",
                Date = p.PerformanceDate,
                Title = p.TopicTitle,
                Detail = p.Remarks,
                Result = p.ResultStatusLabelEn ?? p.ResultStatus,
                TeacherName = p.TeacherName,
                CourseTopicId = p.CourseTopicId,
                TopicTitle = p.TopicTitle,
                RelatedId = p.StudentTopicPerformanceId
            });
        }

        // Class assessment results for this student
        var caResults = await _db.ClassAssessmentResults.AsNoTracking()
            .Include(r => r.ClassAssessment).ThenInclude(a => a.Teacher)
            .Include(r => r.ClassAssessment).ThenInclude(a => a.CourseTopic)
            .Include(r => r.ClassAssessment).ThenInclude(a => a.Subject)
            .Where(r => !r.IsDeleted && r.StudentId == studentId)
            .ToListAsync();
        foreach (var r in caResults)
        {
            var a = r.ClassAssessment;
            if (academicYearId is > 0 && a.AcademicYearId != academicYearId) continue;
            if (subjectId is > 0 && a.SubjectId != subjectId) continue;
            items.Add(new StudentPerformanceTimelineItemDto
            {
                Source = "ClassAssessment",
                Date = a.AssessmentDate,
                Title = a.Title,
                Detail = r.Remarks,
                Result = r.ObtainedMarks.HasValue
                    ? $"{r.ObtainedMarks}/{(a.TotalMarks?.ToString() ?? "?")}"
                    : r.Status,
                TeacherName = a.Teacher.FullName,
                CourseTopicId = a.CourseTopicId,
                TopicTitle = a.CourseTopic?.Title,
                RelatedId = a.ClassAssessmentId
            });
        }

        // Formal exam results
        var examQ = _db.ExamResults.AsNoTracking()
            .Include(r => r.ExamPaper).ThenInclude(p => p.Subject)
            .Where(r => !r.IsDeleted && r.StudentId == studentId);
        if (academicYearId is > 0) examQ = examQ.Where(r => r.ExamPaper.AcademicYearId == academicYearId);
        if (subjectId is > 0) examQ = examQ.Where(r => r.ExamPaper.SubjectId == subjectId);
        foreach (var r in await examQ.Take(50).ToListAsync())
        {
            items.Add(new StudentPerformanceTimelineItemDto
            {
                Source = "Exam",
                Date = DateOnly.FromDateTime(r.EnteredAt ?? r.CreatedAt),
                Title = r.ExamPaper.Title,
                Detail = r.Remarks,
                Result = r.IsAbsent ? "Absent" : $"{r.ObtainedMarks}/{r.ExamPaper.TotalMarks} ({r.Grade})",
                RelatedId = r.ExamResultId
            });
        }

        return items.OrderByDescending(i => i.Date).ThenBy(i => i.Source).ToList();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task EnsureTeacherAccessAsync(int userId, int classId, int subjectId, bool isTeacherOnly)
    {
        if (!isTeacherOnly) return;
        var ok = await _db.ClassSubjects.AnyAsync(cs =>
            cs.TeacherId == userId && cs.ClassId == classId && cs.SubjectId == subjectId && cs.IsActive);
        if (!ok) throw new UnauthorizedAccessException("You are not assigned to this class/subject.");
    }

    private static string NormalizeAssessmentType(string type)
    {
        if (!ValidAssessmentTypes.Contains(type))
            throw new ArgumentException("Invalid assessment type. Use Quiz, ClassTest, OralTest, or Other.");
        return ValidAssessmentTypes.First(v => v.Equals(type, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizePerformanceType(string type)
    {
        if (!ValidPerformanceTypes.Contains(type))
            throw new ArgumentException("Invalid performance type. Use OralRecitation, Participation, or Other.");
        return ValidPerformanceTypes.First(v => v.Equals(type, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<Dictionary<string, AssessmentResultLookup>> GetLookupMapAsync()
    {
        var list = await _db.AssessmentResultLookups.AsNoTracking().Where(l => l.IsActive).ToListAsync();
        return list.ToDictionary(l => l.Code, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<ClassAssessmentDto> MapAssessmentAsync(ClassAssessment a, bool includeResults)
    {
        var lookups = await GetLookupMapAsync();
        var dto = new ClassAssessmentDto
        {
            ClassAssessmentId = a.ClassAssessmentId,
            ClassId = a.ClassId,
            ClassName = a.Class?.ClassName ?? "",
            Section = a.Class?.Section,
            SubjectId = a.SubjectId,
            SubjectName = a.Subject?.SubjectName ?? "",
            AcademicYearId = a.AcademicYearId,
            CourseTopicId = a.CourseTopicId,
            TopicTitle = a.CourseTopic?.Title,
            CourseTeachingLogId = a.CourseTeachingLogId,
            TeacherId = a.TeacherId,
            TeacherName = a.Teacher?.FullName ?? "",
            AssessmentDate = a.AssessmentDate,
            Title = a.Title,
            AssessmentType = a.AssessmentType,
            TotalMarks = a.TotalMarks,
            TotalQuestions = a.TotalQuestions,
            Notes = a.Notes,
            ResultCount = a.Results?.Count(r => !r.IsDeleted) ?? 0
        };

        if (includeResults && a.Results is not null)
        {
            dto.Results = a.Results.Where(r => !r.IsDeleted)
                .OrderBy(r => r.Student?.FirstName).ThenBy(r => r.Student?.LastName)
                .Select(r =>
                {
                    lookups.TryGetValue(r.Status ?? "", out var lk);
                    return new ClassAssessmentResultDto
                    {
                        ClassAssessmentResultId = r.ClassAssessmentResultId,
                        ClassAssessmentId = r.ClassAssessmentId,
                        StudentId = r.StudentId,
                        StudentName = r.Student is null ? "" : $"{r.Student.FirstName} {r.Student.LastName}".Trim(),
                        AdmissionNo = r.Student?.AdmissionNo ?? "",
                        ObtainedMarks = r.ObtainedMarks,
                        Status = r.Status,
                        StatusLabelEn = lk?.LabelEn,
                        StatusLabelUr = lk?.LabelUr,
                        Remarks = r.Remarks
                    };
                }).ToList();
        }
        return dto;
    }

    private static StudentTopicPerformanceDto MapPerformance(
        StudentTopicPerformance p, Dictionary<string, AssessmentResultLookup> lookups)
    {
        lookups.TryGetValue(p.ResultStatus, out var lk);
        return new StudentTopicPerformanceDto
        {
            StudentTopicPerformanceId = p.StudentTopicPerformanceId,
            StudentId = p.StudentId,
            StudentName = p.Student is null ? "" : $"{p.Student.FirstName} {p.Student.LastName}".Trim(),
            AdmissionNo = p.Student?.AdmissionNo ?? "",
            CourseTopicId = p.CourseTopicId,
            TopicTitle = p.CourseTopic?.Title ?? "",
            ClassId = p.ClassId,
            SubjectId = p.SubjectId,
            SubjectName = p.Subject?.SubjectName ?? "",
            AcademicYearId = p.AcademicYearId,
            TeacherId = p.TeacherId,
            TeacherName = p.Teacher?.FullName ?? "",
            PerformanceDate = p.PerformanceDate,
            PerformanceType = p.PerformanceType,
            ResultStatus = p.ResultStatus,
            ResultStatusLabelEn = lk?.LabelEn,
            ResultStatusLabelUr = lk?.LabelUr,
            Remarks = p.Remarks
        };
    }
}
