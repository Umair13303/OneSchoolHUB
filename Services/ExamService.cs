using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Exam;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IExamService
{
    // ── Papers ─────────────────────────────────────────────────────────────────
    Task<ExamPaperDto>           CreatePaperAsync(CreateExamPaperDto dto, int userId);
    Task<ExamPaperDto?>          GetPaperByIdAsync(int paperId);
    Task<List<ExamPaperListDto>> GetPapersAsync(int? classId, int? subjectId, int? academicYearId, ExamType? examType);
    Task<bool>                   UpdatePaperAsync(int paperId, UpdateExamPaperDto dto, int userId);
    Task<bool>                   DeletePaperAsync(int paperId, int userId);
    Task<ExamPaperDto>           PublishPaperAsync(int paperId, int userId);

    // ── Schedules ─────────────────────────────────────────────────────────────
    Task<ExamScheduleDto>        CreateScheduleAsync(CreateExamScheduleDto dto, int userId);
    Task<bool>                   UpdateScheduleAsync(int scheduleId, UpdateExamScheduleDto dto, int userId);
    Task<bool>                   DeleteScheduleAsync(int scheduleId, int userId);
    Task<List<ExamScheduleDto>>  GetSchedulesAsync(int? classId, DateOnly? fromDate, DateOnly? toDate);

    // ── Results ───────────────────────────────────────────────────────────────
    Task<ExamResultDto>          EnterResultAsync(EnterExamResultDto dto, int userId);
    Task<bool>                   BulkEnterResultsAsync(BulkEnterExamResultDto dto, int userId);
    Task<ClassResultSummaryDto>  GetClassResultSummaryAsync(int paperId);
    Task<List<ExamResultDto>>    GetStudentResultsAsync(int studentId, int? academicYearId);
    Task<StudentResultCardDto>   GetStudentResultCardAsync(int studentId, int academicYearId, ExamType examType);
    Task                         RecalculateRanksAsync(int paperId);

    // ── Questions ─────────────────────────────────────────────────────────────
    Task<List<ExamQuestionDto>>  GetQuestionsAsync(int paperId);
    Task<ExamQuestionDto>        AddQuestionAsync(CreateExamQuestionDto dto, int userId);
    Task<bool>                   UpdateQuestionAsync(int questionId, UpdateExamQuestionDto dto, int userId);
    Task<bool>                   DeleteQuestionAsync(int questionId, int userId);
    Task<List<ExamQuestionDto>>  SavePaperQuestionsAsync(SavePaperQuestionsDto dto, int userId);
}

public class ExamService : IExamService
{
    private readonly AppDbContext _db;
    public ExamService(AppDbContext db) => _db = db;

    // ═══════════════════════════════════════════════════════════════════════════
    // PAPERS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<ExamPaperDto> CreatePaperAsync(CreateExamPaperDto dto, int userId)
    {
        // Validate foreign keys
        if (!await _db.AcademicYears.AnyAsync(a => a.AcademicYearId == dto.AcademicYearId))
            throw new ArgumentException("Invalid AcademicYearId.");
        if (!await _db.Classes.AnyAsync(c => c.ClassId == dto.ClassId))
            throw new ArgumentException("Invalid ClassId.");
        if (!await _db.Subjects.AnyAsync(s => s.SubjectId == dto.SubjectId))
            throw new ArgumentException("Invalid SubjectId.");

        if (dto.PassMarks > dto.TotalMarks)
            throw new ArgumentException("PassMarks cannot exceed TotalMarks.");

        // Section marks must sum to TotalMarks (when sections provided)
        if (dto.Sections.Count > 0)
        {
            var sectionSum = dto.Sections.Sum(s => s.AllocatedMarks);
            if (sectionSum != dto.TotalMarks)
                throw new ArgumentException($"Section marks sum ({sectionSum}) must equal TotalMarks ({dto.TotalMarks}).");
        }

        // Auto-generate title if not provided
        var examTypeLabel = dto.ExamType.ToString().Replace("MonthlyTest", "Monthly Test").Replace("MidTerm", "Mid-Term").Replace("PreBoard", "Pre-Board").Replace("FinalExam", "Final Exam");
        var cls    = await _db.Classes.FindAsync(dto.ClassId);
        var subj   = await _db.Subjects.FindAsync(dto.SubjectId);
        var title  = string.IsNullOrWhiteSpace(dto.Title)
            ? $"{examTypeLabel} – {subj!.SubjectName} – {cls!.ClassName}"
            : dto.Title.Trim();

        var paper = new ExamPaper
        {
            AcademicYearId  = dto.AcademicYearId,
            ClassId         = dto.ClassId,
            SubjectId       = dto.SubjectId,
            CreatedByUserId = userId,
            ExamType        = dto.ExamType,
            ClassGroup      = dto.ClassGroup,
            Title           = title,
            TotalMarks      = dto.TotalMarks,
            PassMarks       = dto.PassMarks,
            DurationMinutes = dto.DurationMinutes,
            Instructions    = dto.Instructions?.Trim(),
            SyllabusNote    = dto.SyllabusNote?.Trim(),
            IsDraft         = dto.IsDraft,
            IsLocked        = false,
            CreatedBy       = userId,
            CreatedAt       = DateTime.UtcNow
        };

        _db.ExamPapers.Add(paper);
        await _db.SaveChangesAsync();

        // Sections
        if (dto.Sections.Count > 0)
            await ReplaceSectionsAsync(paper.ExamPaperId, dto.Sections);

        return (await GetPaperByIdAsync(paper.ExamPaperId))!;
    }

    public async Task<ExamPaperDto?> GetPaperByIdAsync(int paperId)
    {
        var p = await _db.ExamPapers
            .Include(x => x.AcademicYear)
            .Include(x => x.Class)
            .Include(x => x.Subject)
            .Include(x => x.CreatedByUser)
            .Include(x => x.Sections.OrderBy(s => s.SortOrder))
            .Include(x => x.Schedules)
                .ThenInclude(sc => sc.InvigilatorUser)
            .FirstOrDefaultAsync(x => x.ExamPaperId == paperId && !x.IsDeleted);

        return p is null ? null : MapPaper(p);
    }

    public async Task<List<ExamPaperListDto>> GetPapersAsync(int? classId, int? subjectId, int? academicYearId, ExamType? examType)
    {
        var q = _db.ExamPapers
            .Include(x => x.Class)
            .Include(x => x.Subject)
            .Include(x => x.AcademicYear)
            .Include(x => x.Schedules)
            .Where(x => !x.IsDeleted);

        if (classId.HasValue)        q = q.Where(x => x.ClassId == classId.Value);
        if (subjectId.HasValue)      q = q.Where(x => x.SubjectId == subjectId.Value);
        if (academicYearId.HasValue) q = q.Where(x => x.AcademicYearId == academicYearId.Value);
        if (examType.HasValue)       q = q.Where(x => x.ExamType == examType.Value);

        var list = await q.OrderByDescending(x => x.CreatedAt).ToListAsync();

        return list.Select(p => new ExamPaperListDto
        {
            ExamPaperId   = p.ExamPaperId,
            Title         = p.Title,
            ExamType      = FormatExamType(p.ExamType),
            ClassName     = p.Class.ClassName,
            SubjectName   = p.Subject.SubjectName,
            TotalMarks    = p.TotalMarks,
            PassMarks     = p.PassMarks,
            IsDraft       = p.IsDraft,
            IsLocked      = p.IsLocked,
            AcademicYear  = p.AcademicYear.YearLabel,
            ScheduledDate = p.Schedules.Any() ? p.Schedules.Min(s => s.ExamDate) : null
        }).ToList();
    }

    public async Task<bool> UpdatePaperAsync(int paperId, UpdateExamPaperDto dto, int userId)
    {
        var paper = await _db.ExamPapers.FirstOrDefaultAsync(p => p.ExamPaperId == paperId && !p.IsDeleted);
        if (paper is null)  return false;
        if (paper.IsLocked) throw new InvalidOperationException("Paper is locked. Cannot edit after results are entered.");

        if (dto.Title != null)           paper.Title           = dto.Title.Trim();
        if (dto.TotalMarks.HasValue)     paper.TotalMarks      = dto.TotalMarks.Value;
        if (dto.PassMarks.HasValue)      paper.PassMarks       = dto.PassMarks.Value;
        if (dto.DurationMinutes.HasValue) paper.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.Instructions != null)    paper.Instructions    = dto.Instructions.Trim();
        if (dto.SyllabusNote != null)    paper.SyllabusNote    = dto.SyllabusNote.Trim();
        if (dto.IsDraft.HasValue)        paper.IsDraft         = dto.IsDraft.Value;

        paper.UpdatedBy = userId;
        paper.UpdatedAt = DateTime.UtcNow;

        if (dto.Sections != null)
            await ReplaceSectionsAsync(paperId, dto.Sections);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePaperAsync(int paperId, int userId)
    {
        var paper = await _db.ExamPapers.FirstOrDefaultAsync(p => p.ExamPaperId == paperId && !p.IsDeleted);
        if (paper is null)  return false;
        if (paper.IsLocked) throw new InvalidOperationException("Cannot delete a paper that has results entered.");

        paper.IsDeleted = true;
        paper.UpdatedBy = userId;
        paper.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ExamPaperDto> PublishPaperAsync(int paperId, int userId)
    {
        var paper = await _db.ExamPapers.FirstOrDefaultAsync(p => p.ExamPaperId == paperId && !p.IsDeleted);
        if (paper is null) throw new ArgumentException("Paper not found.");

        paper.IsDraft   = false;
        paper.UpdatedBy = userId;
        paper.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return (await GetPaperByIdAsync(paperId))!;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SCHEDULES
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<ExamScheduleDto> CreateScheduleAsync(CreateExamScheduleDto dto, int userId)
    {
        if (!await _db.ExamPapers.AnyAsync(p => p.ExamPaperId == dto.ExamPaperId && !p.IsDeleted))
            throw new ArgumentException("Invalid ExamPaperId.");

        var schedule = new ExamSchedule
        {
            ExamPaperId       = dto.ExamPaperId,
            ExamDate          = dto.ExamDate,
            StartTime         = dto.StartTime,
            EndTime           = dto.EndTime,
            RoomOrHall        = dto.RoomOrHall?.Trim(),
            InvigilatorUserId = dto.InvigilatorUserId,
            Status            = "Scheduled",
            Remarks           = dto.Remarks?.Trim(),
            CreatedBy         = userId,
            CreatedAt         = DateTime.UtcNow
        };

        _db.ExamSchedules.Add(schedule);
        await _db.SaveChangesAsync();

        return await GetScheduleDtoAsync(schedule.ExamScheduleId);
    }

    public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateExamScheduleDto dto, int userId)
    {
        var schedule = await _db.ExamSchedules.FirstOrDefaultAsync(s => s.ExamScheduleId == scheduleId && !s.IsDeleted);
        if (schedule is null) return false;

        if (dto.ExamDate.HasValue)          schedule.ExamDate          = dto.ExamDate.Value;
        if (dto.StartTime.HasValue)         schedule.StartTime         = dto.StartTime.Value;
        if (dto.EndTime.HasValue)           schedule.EndTime           = dto.EndTime.Value;
        if (dto.RoomOrHall != null)         schedule.RoomOrHall        = dto.RoomOrHall.Trim();
        if (dto.InvigilatorUserId.HasValue) schedule.InvigilatorUserId = dto.InvigilatorUserId.Value;
        if (dto.Status != null)             schedule.Status            = dto.Status;
        if (dto.Remarks != null)            schedule.Remarks           = dto.Remarks.Trim();

        schedule.UpdatedBy = userId;
        schedule.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteScheduleAsync(int scheduleId, int userId)
    {
        var schedule = await _db.ExamSchedules.FirstOrDefaultAsync(s => s.ExamScheduleId == scheduleId && !s.IsDeleted);
        if (schedule is null) return false;

        schedule.IsDeleted = true;
        schedule.UpdatedBy = userId;
        schedule.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ExamScheduleDto>> GetSchedulesAsync(int? classId, DateOnly? fromDate, DateOnly? toDate)
    {
        var q = _db.ExamSchedules
            .Include(s => s.ExamPaper).ThenInclude(p => p.Class)
            .Include(s => s.InvigilatorUser)
            .Where(s => !s.IsDeleted);

        if (classId.HasValue)  q = q.Where(s => s.ExamPaper.ClassId == classId.Value);
        if (fromDate.HasValue) q = q.Where(s => s.ExamDate >= fromDate.Value);
        if (toDate.HasValue)   q = q.Where(s => s.ExamDate <= toDate.Value);

        var list = await q.OrderBy(s => s.ExamDate).ThenBy(s => s.StartTime).ToListAsync();
        return list.Select(MapSchedule).ToList();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RESULTS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<ExamResultDto> EnterResultAsync(EnterExamResultDto dto, int userId)
    {
        var paper = await _db.ExamPapers.FirstOrDefaultAsync(p => p.ExamPaperId == dto.ExamPaperId && !p.IsDeleted);
        if (paper is null) throw new ArgumentException("Exam paper not found.");

        if (!await _db.Students.AnyAsync(s => s.StudentId == dto.StudentId))
            throw new ArgumentException("Student not found.");

        if (!dto.IsAbsent && dto.ObtainedMarks > paper.TotalMarks)
            throw new ArgumentException($"ObtainedMarks ({dto.ObtainedMarks}) cannot exceed TotalMarks ({paper.TotalMarks}).");

        // Upsert
        var existing = await _db.ExamResults
            .FirstOrDefaultAsync(r => r.ExamPaperId == dto.ExamPaperId && r.StudentId == dto.StudentId);

        decimal obtained = dto.IsAbsent ? 0 : dto.ObtainedMarks;
        bool    isPass   = !dto.IsAbsent && obtained >= paper.PassMarks;
        decimal pct      = paper.TotalMarks > 0 ? Math.Round(obtained / paper.TotalMarks * 100, 2) : 0;
        string  grade    = CalculateGrade(pct, isPass);

        if (existing is null)
        {
            existing = new ExamResult
            {
                ExamPaperId      = dto.ExamPaperId,
                StudentId        = dto.StudentId,
                CreatedBy        = userId,
                CreatedAt        = DateTime.UtcNow
            };
            _db.ExamResults.Add(existing);
        }

        existing.ObtainedMarks    = obtained;
        existing.SectionMarksJson = dto.SectionMarksJson;
        existing.IsAbsent         = dto.IsAbsent;
        existing.IsPass           = isPass;
        existing.Percentage       = pct;
        existing.Grade            = grade;
        existing.Remarks          = dto.Remarks;
        existing.EnteredByUserId  = userId;
        existing.EnteredAt        = DateTime.UtcNow;
        existing.UpdatedBy        = userId;
        existing.UpdatedAt        = DateTime.UtcNow;

        // Lock paper once results start coming in
        if (!paper.IsLocked)
        {
            paper.IsLocked  = true;
            paper.UpdatedBy = userId;
            paper.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return await BuildResultDtoAsync(existing.ExamResultId);
    }

    public async Task<bool> BulkEnterResultsAsync(BulkEnterExamResultDto dto, int userId)
    {
        foreach (var row in dto.Results)
        {
            await EnterResultAsync(new EnterExamResultDto
            {
                ExamPaperId      = dto.ExamPaperId,
                StudentId        = row.StudentId,
                ObtainedMarks    = row.ObtainedMarks,
                SectionMarksJson = row.SectionMarksJson,
                IsAbsent         = row.IsAbsent,
                Remarks          = row.Remarks
            }, userId);
        }
        await RecalculateRanksAsync(dto.ExamPaperId);
        return true;
    }

    public async Task<ClassResultSummaryDto> GetClassResultSummaryAsync(int paperId)
    {
        var paper = await _db.ExamPapers
            .Include(p => p.Class)
            .Include(p => p.Subject)
            .Include(p => p.Results).ThenInclude(r => r.Student)
            .Include(p => p.Results).ThenInclude(r => r.EnteredByUser)
            .FirstOrDefaultAsync(p => p.ExamPaperId == paperId && !p.IsDeleted);

        if (paper is null) throw new ArgumentException("Exam paper not found.");

        var results = paper.Results.ToList();
        var appeared = results.Where(r => !r.IsAbsent).ToList();

        return new ClassResultSummaryDto
        {
            ExamPaperId    = paper.ExamPaperId,
            ExamTitle      = paper.Title,
            ClassName      = paper.Class.ClassName,
            SubjectName    = paper.Subject.SubjectName,
            TotalStudents  = results.Count,
            Appeared       = appeared.Count,
            Passed         = results.Count(r => r.IsPass),
            Failed         = appeared.Count(r => !r.IsPass),
            Absent         = results.Count(r => r.IsAbsent),
            HighestMarks   = appeared.Any() ? appeared.Max(r => r.ObtainedMarks) : null,
            LowestMarks    = appeared.Any() ? appeared.Min(r => r.ObtainedMarks) : null,
            AverageMarks   = appeared.Any() ? Math.Round(appeared.Average(r => r.ObtainedMarks), 2) : null,
            PassPercentage = appeared.Any() ? Math.Round((decimal)results.Count(r => r.IsPass) / appeared.Count * 100, 2) : null,
            StudentResults = results.Select(r => MapResult(r, paper)).ToList()
        };
    }

    public async Task<List<ExamResultDto>> GetStudentResultsAsync(int studentId, int? academicYearId)
    {
        var q = _db.ExamResults
            .Include(r => r.ExamPaper).ThenInclude(p => p.Class)
            .Include(r => r.ExamPaper).ThenInclude(p => p.Subject)
            .Include(r => r.Student)
            .Include(r => r.EnteredByUser)
            .Where(r => r.StudentId == studentId && !r.ExamPaper.IsDeleted);

        if (academicYearId.HasValue)
            q = q.Where(r => r.ExamPaper.AcademicYearId == academicYearId.Value);

        var list = await q.OrderByDescending(r => r.EnteredAt).ToListAsync();
        return list.Select(r => MapResult(r, r.ExamPaper)).ToList();
    }

    public async Task<StudentResultCardDto> GetStudentResultCardAsync(int studentId, int academicYearId, ExamType examType)
    {
        var student = await _db.Students
            .Include(s => s.Enrollments).ThenInclude(e => e.Class)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student is null) throw new ArgumentException("Student not found.");

        var year = await _db.AcademicYears.FindAsync(academicYearId);

        var results = await _db.ExamResults
            .Include(r => r.ExamPaper).ThenInclude(p => p.Subject)
            .Where(r => r.StudentId == studentId
                     && r.ExamPaper.AcademicYearId == academicYearId
                     && r.ExamPaper.ExamType == examType
                     && !r.ExamPaper.IsDeleted)
            .ToListAsync();

        var enrollment = student.Enrollments
            .FirstOrDefault(e => e.AcademicYearId == academicYearId);

        decimal grandTotal = results.Sum(r => r.ObtainedMarks);
        int     totalMax   = results.Sum(r => r.ExamPaper.TotalMarks);
        decimal pct        = totalMax > 0 ? Math.Round(grandTotal / totalMax * 100, 2) : 0;

        return new StudentResultCardDto
        {
            StudentId    = studentId,
            StudentName  = $"{student.FirstName} {student.LastName}",
            AdmissionNo  = student.AdmissionNo,
            ClassName    = enrollment?.Class.ClassName ?? "—",
            AcademicYear = year?.YearLabel ?? "—",
            SubjectResults = results.Select(r => new SubjectResultRowDto
            {
                SubjectName   = r.ExamPaper.Subject.SubjectName,
                ExamType      = FormatExamType(r.ExamPaper.ExamType),
                TotalMarks    = r.ExamPaper.TotalMarks,
                ObtainedMarks = r.ObtainedMarks,
                Percentage    = r.Percentage,
                Grade         = r.Grade,
                IsAbsent      = r.IsAbsent,
                IsPass        = r.IsPass
            }).ToList(),
            GrandTotal   = grandTotal,
            Percentage   = pct,
            OverallGrade = CalculateGrade(pct, pct >= 33),
            ClassRank    = null   // calculated separately if needed
        };
    }

    public async Task RecalculateRanksAsync(int paperId)
    {
        var results = await _db.ExamResults
            .Where(r => r.ExamPaperId == paperId && !r.IsAbsent)
            .OrderByDescending(r => r.ObtainedMarks)
            .ToListAsync();

        int rank = 1;
        for (int i = 0; i < results.Count; i++)
        {
            if (i > 0 && results[i].ObtainedMarks < results[i - 1].ObtainedMarks)
                rank = i + 1;
            results[i].ClassRank = rank;
        }
        await _db.SaveChangesAsync();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // PRIVATE HELPERS
    // ═══════════════════════════════════════════════════════════════════════════

    private async Task ReplaceSectionsAsync(int paperId, List<CreateExamPaperSectionDto> sections)
    {
        var old = _db.ExamPaperSections.Where(s => s.ExamPaperId == paperId);
        _db.ExamPaperSections.RemoveRange(old);

        foreach (var s in sections)
        {
            _db.ExamPaperSections.Add(new ExamPaperSection
            {
                ExamPaperId      = paperId,
                SectionName      = s.SectionName.Trim(),
                SectionType      = s.SectionType,
                AllocatedMarks   = s.AllocatedMarks,
                TotalQuestions   = s.TotalQuestions,
                AttemptQuestions = s.AttemptQuestions,
                MarksPerQuestion = s.MarksPerQuestion,
                SectionNote      = s.SectionNote?.Trim(),
                SortOrder        = s.SortOrder
            });
        }
        await _db.SaveChangesAsync();
    }

    private async Task<ExamScheduleDto> GetScheduleDtoAsync(int scheduleId)
    {
        var s = await _db.ExamSchedules
            .Include(x => x.InvigilatorUser)
            .FirstAsync(x => x.ExamScheduleId == scheduleId);
        return MapSchedule(s);
    }

    private async Task<ExamResultDto> BuildResultDtoAsync(int resultId)
    {
        var r = await _db.ExamResults
            .Include(x => x.ExamPaper)
            .Include(x => x.Student)
            .Include(x => x.EnteredByUser)
            .FirstAsync(x => x.ExamResultId == resultId);
        return MapResult(r, r.ExamPaper);
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static ExamPaperDto MapPaper(ExamPaper p) => new()
    {
        ExamPaperId      = p.ExamPaperId,
        AcademicYearId   = p.AcademicYearId,
        AcademicYearName = p.AcademicYear.YearLabel,
        ClassId          = p.ClassId,
        ClassName        = p.Class.ClassName,
        SubjectId        = p.SubjectId,
        SubjectName      = p.Subject.SubjectName,
        ExamType         = FormatExamType(p.ExamType),
        ExamTypeId       = (int)p.ExamType,
        ClassGroup       = p.ClassGroup.ToString(),
        ClassGroupId     = (int)p.ClassGroup,
        Title            = p.Title,
        TotalMarks       = p.TotalMarks,
        PassMarks        = p.PassMarks,
        DurationMinutes  = p.DurationMinutes,
        Instructions     = p.Instructions,
        SyllabusNote     = p.SyllabusNote,
        IsDraft          = p.IsDraft,
        IsLocked         = p.IsLocked,
        CreatedByName    = p.CreatedByUser.FullName,
        CreatedAt        = p.CreatedAt,
        Sections         = p.Sections.Select(s => new ExamPaperSectionDto
        {
            ExamPaperSectionId = s.ExamPaperSectionId,
            SectionName        = s.SectionName,
            SectionType        = s.SectionType,
            AllocatedMarks     = s.AllocatedMarks,
            TotalQuestions     = s.TotalQuestions,
            AttemptQuestions   = s.AttemptQuestions,
            MarksPerQuestion   = s.MarksPerQuestion,
            SectionNote        = s.SectionNote,
            SortOrder          = s.SortOrder
        }).ToList(),
        Schedules = p.Schedules.Where(s => !s.IsDeleted).Select(MapSchedule).ToList()
    };

    private static ExamScheduleDto MapSchedule(ExamSchedule s) => new()
    {
        ExamScheduleId    = s.ExamScheduleId,
        ExamPaperId       = s.ExamPaperId,
        ExamDate          = s.ExamDate,
        StartTime         = s.StartTime,
        EndTime           = s.EndTime,
        RoomOrHall        = s.RoomOrHall,
        InvigilatorUserId = s.InvigilatorUserId,
        InvigilatorName   = s.InvigilatorUser?.FullName,
        Status            = s.Status,
        Remarks           = s.Remarks
    };

    private static ExamResultDto MapResult(ExamResult r, ExamPaper p) => new()
    {
        ExamResultId     = r.ExamResultId,
        ExamPaperId      = r.ExamPaperId,
        ExamTitle        = p.Title,
        StudentId        = r.StudentId,
        StudentName      = $"{r.Student.FirstName} {r.Student.LastName}",
        AdmissionNo      = r.Student.AdmissionNo,
        ObtainedMarks    = r.ObtainedMarks,
        TotalMarks       = p.TotalMarks,
        PassMarks        = p.PassMarks,
        Percentage       = r.Percentage,
        Grade            = r.Grade,
        IsAbsent         = r.IsAbsent,
        IsPass           = r.IsPass,
        ClassRank        = r.ClassRank,
        Remarks          = r.Remarks,
        SectionMarksJson = r.SectionMarksJson,
        EnteredByName    = r.EnteredByUser?.FullName,
        EnteredAt        = r.EnteredAt
    };

    // ── Grade calculation (Pakistan grading scale) ────────────────────────────
    /// <summary>
    /// Pakistan grading scale used across Punjab, Federal, Sindh & KPK boards.
    /// A+ ≥ 90 | A ≥ 80 | B ≥ 65 | C ≥ 50 | D ≥ 40 | E ≥ 33 | F < 33
    /// </summary>
    private static string CalculateGrade(decimal percentage, bool isPass)
    {
        if (!isPass)   return "F";
        return percentage switch
        {
            >= 90 => "A+",
            >= 80 => "A",
            >= 65 => "B",
            >= 50 => "C",
            >= 40 => "D",
            >= 33 => "E",
            _     => "F"
        };
    }

    private static string FormatExamType(ExamType et) => et switch
    {
        ExamType.Quiz        => "Quiz",
        ExamType.MonthlyTest => "Monthly Test",
        ExamType.MidTerm     => "Mid-Term",
        ExamType.PreBoard    => "Pre-Board",
        ExamType.FinalExam   => "Final Exam",
        _                    => et.ToString()
    };

    // ═══════════════════════════════════════════════════════════════════════════
    // QUESTIONS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<List<ExamQuestionDto>> GetQuestionsAsync(int paperId)
    {
        var questions = await _db.ExamQuestions
            .Where(q => q.ExamPaperId == paperId)
            .Include(q => q.Options)
            .Include(q => q.ExamPaperSection)
            .OrderBy(q => q.SortOrder)
            .ToListAsync();

        return questions.Select(MapQuestion).ToList();
    }

    public async Task<ExamQuestionDto> AddQuestionAsync(CreateExamQuestionDto dto, int userId)
    {
        ValidateQuestion(dto.QuestionType, dto.Options, dto.IsTrue);

        var q = new ExamQuestion
        {
            ExamPaperId        = dto.ExamPaperId,
            ExamPaperSectionId = dto.ExamPaperSectionId,
            QuestionType       = dto.QuestionType,
            QuestionText       = dto.QuestionText,
            Language           = dto.Language ?? "en",
            Marks              = dto.Marks,
            SortOrder          = dto.SortOrder,
            CorrectAnswer      = dto.CorrectAnswer,
            IsTrue             = dto.IsTrue,
            QuestionNote       = dto.QuestionNote,
            CreatedBy          = userId,
            CreatedAt          = DateTime.UtcNow
        };

        foreach (var (opt, i) in dto.Options.Select((o, i) => (o, i)))
            q.Options.Add(new ExamQuestionOption
            {
                OptionLabel = opt.OptionLabel,
                OptionText  = opt.OptionText,
                IsCorrect   = opt.IsCorrect,
                SortOrder   = opt.SortOrder == 0 ? i : opt.SortOrder,
                CreatedBy   = userId,
                CreatedAt   = DateTime.UtcNow
            });

        _db.ExamQuestions.Add(q);
        await _db.SaveChangesAsync();

        await _db.Entry(q).Reference(x => x.ExamPaperSection).LoadAsync();
        return MapQuestion(q);
    }

    public async Task<bool> UpdateQuestionAsync(int questionId, UpdateExamQuestionDto dto, int userId)
    {
        var q = await _db.ExamQuestions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.ExamQuestionId == questionId);
        if (q is null) return false;

        if (dto.ExamPaperSectionId.HasValue) q.ExamPaperSectionId = dto.ExamPaperSectionId;
        if (dto.QuestionText  != null) q.QuestionText  = dto.QuestionText;
        if (dto.Language      != null) q.Language      = dto.Language;
        if (dto.Marks.HasValue)        q.Marks         = dto.Marks.Value;
        if (dto.SortOrder.HasValue)    q.SortOrder     = dto.SortOrder.Value;
        if (dto.CorrectAnswer != null) q.CorrectAnswer = dto.CorrectAnswer;
        if (dto.IsTrue.HasValue)       q.IsTrue        = dto.IsTrue;
        if (dto.QuestionNote  != null) q.QuestionNote  = dto.QuestionNote;
        q.UpdatedBy = userId;
        q.UpdatedAt = DateTime.UtcNow;

        if (dto.Options != null)
        {
            _db.ExamQuestionOptions.RemoveRange(q.Options);
            foreach (var (opt, i) in dto.Options.Select((o, i) => (o, i)))
                q.Options.Add(new ExamQuestionOption
                {
                    OptionLabel = opt.OptionLabel,
                    OptionText  = opt.OptionText,
                    IsCorrect   = opt.IsCorrect,
                    SortOrder   = opt.SortOrder == 0 ? i : opt.SortOrder,
                    CreatedBy   = userId,
                    CreatedAt   = DateTime.UtcNow
                });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteQuestionAsync(int questionId, int userId)
    {
        var q = await _db.ExamQuestions.FindAsync(questionId);
        if (q is null) return false;
        q.IsDeleted = true;
        q.UpdatedBy = userId;
        q.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ExamQuestionDto>> SavePaperQuestionsAsync(SavePaperQuestionsDto dto, int userId)
    {
        // Delete existing questions for this paper
        var existing = await _db.ExamQuestions
            .Include(q => q.Options)
            .Where(q => q.ExamPaperId == dto.ExamPaperId)
            .ToListAsync();
        _db.ExamQuestions.RemoveRange(existing);

        var newQuestions = new List<ExamQuestion>();
        foreach (var (qDto, i) in dto.Questions.Select((q, i) => (q, i)))
        {
            ValidateQuestion(qDto.QuestionType, qDto.Options, qDto.IsTrue);
            var q = new ExamQuestion
            {
                ExamPaperId        = dto.ExamPaperId,
                ExamPaperSectionId = qDto.ExamPaperSectionId,
                QuestionType       = qDto.QuestionType,
                QuestionText       = qDto.QuestionText,
                Language           = qDto.Language ?? "en",
                Marks              = qDto.Marks,
                SortOrder          = qDto.SortOrder == 0 ? i + 1 : qDto.SortOrder,
                CorrectAnswer      = qDto.CorrectAnswer,
                IsTrue             = qDto.IsTrue,
                QuestionNote       = qDto.QuestionNote,
                CreatedBy          = userId,
                CreatedAt          = DateTime.UtcNow
            };
            foreach (var (opt, j) in qDto.Options.Select((o, j) => (o, j)))
                q.Options.Add(new ExamQuestionOption
                {
                    OptionLabel = opt.OptionLabel,
                    OptionText  = opt.OptionText,
                    IsCorrect   = opt.IsCorrect,
                    SortOrder   = opt.SortOrder == 0 ? j : opt.SortOrder,
                    CreatedBy   = userId,
                    CreatedAt   = DateTime.UtcNow
                });
            newQuestions.Add(q);
        }

        _db.ExamQuestions.AddRange(newQuestions);
        await _db.SaveChangesAsync();

        return newQuestions.Select(MapQuestion).ToList();
    }

    private static void ValidateQuestion(QuestionType type, List<CreateExamQuestionOptionDto> options, bool? isTrue)
    {
        if (type == QuestionType.MultipleChoice)
        {
            if (options.Count < 2)
                throw new ArgumentException("MultipleChoice questions require at least 2 options.");
            if (!options.Any(o => o.IsCorrect))
                throw new ArgumentException("MultipleChoice questions must have exactly one correct option.");
        }
        if (type == QuestionType.TrueFalse && isTrue is null)
            throw new ArgumentException("TrueFalse questions require IsTrue to be set.");
    }

    private static ExamQuestionDto MapQuestion(ExamQuestion q) => new()
    {
        ExamQuestionId     = q.ExamQuestionId,
        ExamPaperId        = q.ExamPaperId,
        ExamPaperSectionId = q.ExamPaperSectionId,
        SectionName        = q.ExamPaperSection?.SectionName,
        QuestionTypeId     = (int)q.QuestionType,
        QuestionType       = q.QuestionType switch
        {
            Models.QuestionType.MultipleChoice => "Multiple Choice",
            Models.QuestionType.TrueFalse      => "True / False",
            Models.QuestionType.FillInBlanks   => "Fill in the Blanks",
            Models.QuestionType.ShortQuestion  => "Short Question",
            Models.QuestionType.LongQuestion   => "Long Question",
            _                                  => q.QuestionType.ToString()
        },
        QuestionText   = q.QuestionText,
        Language       = q.Language,
        Marks          = q.Marks,
        SortOrder      = q.SortOrder,
        CorrectAnswer  = q.CorrectAnswer,
        IsTrue         = q.IsTrue,
        QuestionNote   = q.QuestionNote,
        Options        = q.Options.OrderBy(o => o.SortOrder).Select(o => new ExamQuestionOptionDto
        {
            ExamQuestionOptionId = o.ExamQuestionOptionId,
            OptionLabel          = o.OptionLabel,
            OptionText           = o.OptionText,
            IsCorrect            = o.IsCorrect,
            SortOrder            = o.SortOrder
        }).ToList()
    };
}
