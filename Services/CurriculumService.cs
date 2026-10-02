using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Curriculum;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ICurriculumService
{
    Task<List<CoursePlanListDto>> GetPlansAsync(int? academicYearId, int? classId, int? subjectId, string? status);
    Task<CoursePlanDetailDto?> GetPlanAsync(int id);
    Task<CoursePlanListDto> CreatePlanAsync(CreateCoursePlanDto dto, int userId);
    Task<bool> UpdatePlanAsync(int id, UpdateCoursePlanDto dto, int userId);
    Task<bool> PublishPlanAsync(int id, int userId);
    Task<bool> ArchivePlanAsync(int id, int userId);
    Task<bool> SoftDeletePlanAsync(int id, int userId);

    Task<CourseChapterDto> CreateChapterAsync(CreateCourseChapterDto dto, int userId);
    Task<bool> UpdateChapterAsync(int id, UpdateCourseChapterDto dto, int userId);
    Task<bool> SoftDeleteChapterAsync(int id, int userId);
    Task<bool> ReorderChaptersAsync(int planId, ReorderDto dto, int userId);

    Task<CourseTopicDto> CreateTopicAsync(CreateCourseTopicDto dto, int userId);
    Task<bool> UpdateTopicAsync(int id, UpdateCourseTopicDto dto, int userId);
    Task<bool> SoftDeleteTopicAsync(int id, int userId);
    Task<bool> ReorderTopicsAsync(int chapterId, ReorderDto dto, int userId);

    Task<CourseMaterialDto> CreateMaterialAsync(CreateCourseMaterialDto dto, int userId);
    Task<bool> UpdateMaterialAsync(int id, UpdateCourseMaterialDto dto, int userId);
    Task<bool> SoftDeleteMaterialAsync(int id, int userId);

    Task<CourseTopicActivityDto> CreateActivityAsync(CreateCourseTopicActivityDto dto, int userId);
    Task<bool> UpdateActivityAsync(int id, UpdateCourseTopicActivityDto dto, int userId);
    Task<bool> SoftDeleteActivityAsync(int id, int userId);
    Task<bool> ReorderActivitiesAsync(int topicId, ReorderDto dto, int userId);

    Task<List<CoursePlanListDto>> GetMyCoursesAsync(int teacherId);
    Task<CoursePlanDetailDto?> GetWorkspaceAsync(int planId, int? teacherId, bool requirePublished);
    Task<CourseTopicDto?> GetTopicDetailAsync(int topicId, int? teacherId, bool requirePublished);
    Task<bool> UpdateTopicProgressAsync(int topicId, UpdateTopicProgressDto dto, int teacherId);

    Task<TeachingLogDto> CreateTeachingLogAsync(CreateTeachingLogDto dto, int teacherId);
    Task<List<TeachingLogDto>> GetTeachingLogsAsync(int? classId, DateOnly? date, int? subjectId, int? topicId, DateOnly? from, DateOnly? to);
    Task<TeachingLogDto> LinkHomeworkAsync(int teachingLogId, LinkHomeworkDto dto, int teacherId);

    Task<List<CurriculumProgressSummaryDto>> GetProgressSummariesAsync(int? academicYearId, int? classId, int? subjectId);
    Task<CurriculumProgressSummaryDto?> GetProgressDetailAsync(int planId);
}

public class CurriculumService : ICurriculumService
{
    private static readonly HashSet<string> ValidPlanStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Draft", "Published", "Archived" };
    private static readonly HashSet<string> ValidProgressStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "NotStarted", "InProgress", "Completed" };
    private static readonly HashSet<string> ValidTeachingTypes =
        new(StringComparer.OrdinalIgnoreCase) { "NewTopic", "Revision", "Practice", "Assessment" };
    private static readonly HashSet<string> ValidMaterialTypes =
        new(StringComparer.OrdinalIgnoreCase) { "Pdf", "Booklet", "Worksheet", "Notes", "Image", "Link", "Other" };
    private static readonly HashSet<string> ValidActivityTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Drawing", "Coloring", "Tracing", "Matching", "ConnectDots",
            "ShapeIdentification", "PictureIdentification", "DragArrange",
            "FillBlanks", "CircleSelect", "ImageQuestion", "Worksheet"
        };

    private readonly AppDbContext _db;

    public CurriculumService(AppDbContext db) => _db = db;

    // ── Plans ─────────────────────────────────────────────────────────────────

    public async Task<List<CoursePlanListDto>> GetPlansAsync(int? academicYearId, int? classId, int? subjectId, string? status)
    {
        var q = _db.CoursePlans.AsNoTracking()
            .Include(p => p.AcademicYear)
            .Include(p => p.Class)
            .Include(p => p.Subject)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Progress)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (academicYearId is > 0) q = q.Where(p => p.AcademicYearId == academicYearId);
        if (classId is > 0) q = q.Where(p => p.ClassId == classId);
        if (subjectId is > 0) q = q.Where(p => p.SubjectId == subjectId);
        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(p => p.Status == status);

        var plans = await q.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return plans.Select(MapPlanList).ToList();
    }

    public async Task<CoursePlanDetailDto?> GetPlanAsync(int id)
    {
        var plan = await LoadPlanGraphAsync(id);
        return plan is null ? null : MapPlanDetail(plan);
    }

    public async Task<CoursePlanListDto> CreatePlanAsync(CreateCoursePlanDto dto, int userId)
    {
        await ValidatePlanKeysAsync(dto.AcademicYearId, dto.ClassId, dto.SubjectId);

        var clash = await _db.CoursePlans.AnyAsync(p =>
            !p.IsDeleted &&
            p.Status != "Archived" &&
            p.AcademicYearId == dto.AcademicYearId &&
            p.ClassId == dto.ClassId &&
            p.SubjectId == dto.SubjectId);

        if (clash)
            throw new ArgumentException("An active Course Plan already exists for this Academic Year, Class, and Subject.");

        var title = string.IsNullOrWhiteSpace(dto.Title)
            ? await BuildDefaultTitleAsync(dto.AcademicYearId, dto.ClassId, dto.SubjectId)
            : dto.Title.Trim();

        var entity = new CoursePlan
        {
            AcademicYearId = dto.AcademicYearId,
            ClassId = dto.ClassId,
            SubjectId = dto.SubjectId,
            Title = title,
            Description = dto.Description?.Trim(),
            Status = "Draft",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.CoursePlans.Add(entity);
        await _db.SaveChangesAsync();
        return (await GetPlansAsync(null, null, null, null)).First(p => p.CoursePlanId == entity.CoursePlanId);
    }

    public async Task<bool> UpdatePlanAsync(int id, UpdateCoursePlanDto dto, int userId)
    {
        var plan = await _db.CoursePlans.FirstOrDefaultAsync(p => p.CoursePlanId == id && !p.IsDeleted);
        if (plan is null) return false;
        if (plan.Status == "Archived")
            throw new ArgumentException("Archived plans cannot be edited.");

        plan.Title = dto.Title.Trim();
        plan.Description = dto.Description?.Trim();
        plan.UpdatedBy = userId;
        plan.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishPlanAsync(int id, int userId)
    {
        var plan = await _db.CoursePlans.FirstOrDefaultAsync(p => p.CoursePlanId == id && !p.IsDeleted);
        if (plan is null) return false;
        if (plan.Status == "Archived")
            throw new ArgumentException("Cannot publish an archived plan.");

        plan.Status = "Published";
        plan.PublishedAt = DateTime.UtcNow;
        plan.UpdatedBy = userId;
        plan.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchivePlanAsync(int id, int userId)
    {
        var plan = await _db.CoursePlans.FirstOrDefaultAsync(p => p.CoursePlanId == id && !p.IsDeleted);
        if (plan is null) return false;

        plan.Status = "Archived";
        plan.UpdatedBy = userId;
        plan.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeletePlanAsync(int id, int userId)
    {
        var plan = await _db.CoursePlans.FirstOrDefaultAsync(p => p.CoursePlanId == id && !p.IsDeleted);
        if (plan is null) return false;

        plan.IsDeleted = true;
        plan.UpdatedBy = userId;
        plan.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Chapters ──────────────────────────────────────────────────────────────

    public async Task<CourseChapterDto> CreateChapterAsync(CreateCourseChapterDto dto, int userId)
    {
        var plan = await RequireEditablePlanAsync(dto.CoursePlanId);
        var maxOrder = await _db.CourseChapters
            .Where(c => c.CoursePlanId == plan.CoursePlanId && !c.IsDeleted)
            .Select(c => (int?)c.SortOrder).MaxAsync() ?? 0;

        var chapter = new CourseChapter
        {
            CoursePlanId = plan.CoursePlanId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            SortOrder = maxOrder + 1,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        _db.CourseChapters.Add(chapter);
        await _db.SaveChangesAsync();
        return MapChapter(chapter);
    }

    public async Task<bool> UpdateChapterAsync(int id, UpdateCourseChapterDto dto, int userId)
    {
        var chapter = await _db.CourseChapters.Include(c => c.CoursePlan)
            .FirstOrDefaultAsync(c => c.CourseChapterId == id && !c.IsDeleted);
        if (chapter is null) return false;
        EnsureEditable(chapter.CoursePlan);

        chapter.Title = dto.Title.Trim();
        chapter.Description = dto.Description?.Trim();
        chapter.UpdatedBy = userId;
        chapter.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteChapterAsync(int id, int userId)
    {
        var chapter = await _db.CourseChapters.Include(c => c.CoursePlan)
            .FirstOrDefaultAsync(c => c.CourseChapterId == id && !c.IsDeleted);
        if (chapter is null) return false;
        EnsureEditable(chapter.CoursePlan);

        chapter.IsDeleted = true;
        chapter.UpdatedBy = userId;
        chapter.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderChaptersAsync(int planId, ReorderDto dto, int userId)
    {
        await RequireEditablePlanAsync(planId);
        var chapters = await _db.CourseChapters
            .Where(c => c.CoursePlanId == planId && !c.IsDeleted)
            .ToListAsync();

        for (var i = 0; i < dto.OrderedIds.Count; i++)
        {
            var ch = chapters.FirstOrDefault(c => c.CourseChapterId == dto.OrderedIds[i]);
            if (ch is null) continue;
            ch.SortOrder = i + 1;
            ch.UpdatedBy = userId;
            ch.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Topics ────────────────────────────────────────────────────────────────

    public async Task<CourseTopicDto> CreateTopicAsync(CreateCourseTopicDto dto, int userId)
    {
        var chapter = await _db.CourseChapters.Include(c => c.CoursePlan)
            .FirstOrDefaultAsync(c => c.CourseChapterId == dto.CourseChapterId && !c.IsDeleted)
            ?? throw new ArgumentException("Chapter not found.");
        EnsureEditable(chapter.CoursePlan);

        var maxOrder = await _db.CourseTopics
            .Where(t => t.CourseChapterId == chapter.CourseChapterId && !t.IsDeleted)
            .Select(t => (int?)t.SortOrder).MaxAsync() ?? 0;

        var topic = new CourseTopic
        {
            CourseChapterId = chapter.CourseChapterId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            SortOrder = maxOrder + 1,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        _db.CourseTopics.Add(topic);
        await _db.SaveChangesAsync();

        _db.CourseTopicProgresses.Add(new CourseTopicProgress
        {
            CourseTopicId = topic.CourseTopicId,
            Status = "NotStarted",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return MapTopic(topic, null);
    }

    public async Task<bool> UpdateTopicAsync(int id, UpdateCourseTopicDto dto, int userId)
    {
        var topic = await _db.CourseTopics.Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(t => t.CourseTopicId == id && !t.IsDeleted);
        if (topic is null) return false;
        EnsureEditable(topic.CourseChapter.CoursePlan);

        topic.Title = dto.Title.Trim();
        topic.Description = dto.Description?.Trim();
        topic.UpdatedBy = userId;
        topic.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteTopicAsync(int id, int userId)
    {
        var topic = await _db.CourseTopics.Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(t => t.CourseTopicId == id && !t.IsDeleted);
        if (topic is null) return false;
        EnsureEditable(topic.CourseChapter.CoursePlan);

        topic.IsDeleted = true;
        topic.UpdatedBy = userId;
        topic.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderTopicsAsync(int chapterId, ReorderDto dto, int userId)
    {
        var chapter = await _db.CourseChapters.Include(c => c.CoursePlan)
            .FirstOrDefaultAsync(c => c.CourseChapterId == chapterId && !c.IsDeleted)
            ?? throw new ArgumentException("Chapter not found.");
        EnsureEditable(chapter.CoursePlan);

        var topics = await _db.CourseTopics
            .Where(t => t.CourseChapterId == chapterId && !t.IsDeleted)
            .ToListAsync();

        for (var i = 0; i < dto.OrderedIds.Count; i++)
        {
            var t = topics.FirstOrDefault(x => x.CourseTopicId == dto.OrderedIds[i]);
            if (t is null) continue;
            t.SortOrder = i + 1;
            t.UpdatedBy = userId;
            t.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Materials ─────────────────────────────────────────────────────────────

    public async Task<CourseMaterialDto> CreateMaterialAsync(CreateCourseMaterialDto dto, int userId)
    {
        if (dto.CourseChapterId is null && dto.CourseTopicId is null)
            throw new ArgumentException("Attach material to a chapter or a topic.");
        if (dto.CourseChapterId is not null && dto.CourseTopicId is not null)
            throw new ArgumentException("Material cannot belong to both a chapter and a topic.");

        CoursePlan plan;
        if (dto.CourseChapterId is not null)
        {
            var chapter = await _db.CourseChapters.Include(c => c.CoursePlan)
                .FirstOrDefaultAsync(c => c.CourseChapterId == dto.CourseChapterId && !c.IsDeleted)
                ?? throw new ArgumentException("Chapter not found.");
            plan = chapter.CoursePlan;
        }
        else
        {
            var topic = await _db.CourseTopics.Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
                .FirstOrDefaultAsync(t => t.CourseTopicId == dto.CourseTopicId && !t.IsDeleted)
                ?? throw new ArgumentException("Topic not found.");
            plan = topic.CourseChapter.CoursePlan;
        }
        EnsureEditable(plan);

        var materialType = NormalizeMaterialType(dto.MaterialType);
        var maxOrder = await _db.CourseMaterials
            .Where(m => !m.IsDeleted &&
                        m.CourseChapterId == dto.CourseChapterId &&
                        m.CourseTopicId == dto.CourseTopicId)
            .Select(m => (int?)m.SortOrder).MaxAsync() ?? 0;

        var material = new CourseMaterial
        {
            CourseChapterId = dto.CourseChapterId,
            CourseTopicId = dto.CourseTopicId,
            Title = dto.Title.Trim(),
            MaterialType = materialType,
            Content = string.IsNullOrWhiteSpace(dto.Content) ? null : dto.Content.Trim(),
            Url = string.IsNullOrWhiteSpace(dto.Url) ? null : dto.Url.Trim(),
            FileStoreId = dto.FileStoreId,
            SortOrder = maxOrder + 1,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        _db.CourseMaterials.Add(material);
        await _db.SaveChangesAsync();

        if (material.FileStoreId is int fid)
        {
            var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == fid && !f.IsDeleted);
            if (file is not null)
            {
                file.EntityType = "CourseMaterial";
                file.EntityId = material.CourseMaterialId;
            }
            await _db.SaveChangesAsync();
        }

        return await MapMaterialAsync(material);
    }

    public async Task<bool> UpdateMaterialAsync(int id, UpdateCourseMaterialDto dto, int userId)
    {
        var material = await LoadMaterialWithPlanAsync(id);
        if (material is null) return false;
        EnsureEditable(GetPlanFromMaterial(material));

        material.Title = dto.Title.Trim();
        material.MaterialType = NormalizeMaterialType(dto.MaterialType);
        material.Content = string.IsNullOrWhiteSpace(dto.Content) ? null : dto.Content.Trim();
        material.Url = string.IsNullOrWhiteSpace(dto.Url) ? null : dto.Url.Trim();
        material.FileStoreId = dto.FileStoreId;
        material.UpdatedBy = userId;
        material.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteMaterialAsync(int id, int userId)
    {
        var material = await LoadMaterialWithPlanAsync(id);
        if (material is null) return false;
        EnsureEditable(GetPlanFromMaterial(material));

        material.IsDeleted = true;
        material.UpdatedBy = userId;
        material.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Topic Activities (early-grade structured activities) ──────────────────

    public async Task<CourseTopicActivityDto> CreateActivityAsync(CreateCourseTopicActivityDto dto, int userId)
    {
        var topic = await _db.CourseTopics
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(t => t.CourseTopicId == dto.CourseTopicId && !t.IsDeleted)
            ?? throw new ArgumentException("Topic not found.");
        EnsureEditable(topic.CourseChapter.CoursePlan);

        var activityType = NormalizeActivityType(dto.ActivityType);
        var maxOrder = await _db.CourseTopicActivities
            .Where(a => a.CourseTopicId == topic.CourseTopicId && !a.IsDeleted)
            .Select(a => (int?)a.SortOrder).MaxAsync() ?? 0;

        var canvasDefault = activityType is "Drawing" or "Coloring" or "Tracing" or "ConnectDots";

        var activity = new CourseTopicActivity
        {
            CourseTopicId = topic.CourseTopicId,
            ActivityType = activityType,
            Title = dto.Title.Trim(),
            InstructionText = dto.InstructionText?.Trim(),
            ReferenceImageFileId = dto.ReferenceImageFileId,
            ExampleImageFileId = dto.ExampleImageFileId,
            WorksheetFileId = dto.WorksheetFileId,
            ConfigJson = string.IsNullOrWhiteSpace(dto.ConfigJson) ? null : dto.ConfigJson.Trim(),
            CanvasEnabled = canvasDefault || dto.CanvasEnabled,
            SortOrder = maxOrder + 1,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        _db.CourseTopicActivities.Add(activity);
        await _db.SaveChangesAsync();

        await TagActivityFilesAsync(activity);
        await _db.SaveChangesAsync();

        return await MapActivityAsync(activity);
    }

    public async Task<bool> UpdateActivityAsync(int id, UpdateCourseTopicActivityDto dto, int userId)
    {
        var activity = await _db.CourseTopicActivities
            .Include(a => a.CourseTopic).ThenInclude(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(a => a.CourseTopicActivityId == id && !a.IsDeleted);
        if (activity is null) return false;
        EnsureEditable(activity.CourseTopic.CourseChapter.CoursePlan);

        activity.Title = dto.Title.Trim();
        activity.InstructionText = dto.InstructionText?.Trim();
        activity.ReferenceImageFileId = dto.ReferenceImageFileId;
        activity.ExampleImageFileId = dto.ExampleImageFileId;
        activity.WorksheetFileId = dto.WorksheetFileId;
        activity.ConfigJson = string.IsNullOrWhiteSpace(dto.ConfigJson) ? null : dto.ConfigJson.Trim();
        activity.CanvasEnabled = dto.CanvasEnabled;
        activity.UpdatedBy = userId;
        activity.UpdatedAt = DateTime.UtcNow;

        await TagActivityFilesAsync(activity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteActivityAsync(int id, int userId)
    {
        var activity = await _db.CourseTopicActivities
            .Include(a => a.CourseTopic).ThenInclude(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(a => a.CourseTopicActivityId == id && !a.IsDeleted);
        if (activity is null) return false;
        EnsureEditable(activity.CourseTopic.CourseChapter.CoursePlan);

        activity.IsDeleted = true;
        activity.UpdatedBy = userId;
        activity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderActivitiesAsync(int topicId, ReorderDto dto, int userId)
    {
        var topic = await _db.CourseTopics
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(t => t.CourseTopicId == topicId && !t.IsDeleted)
            ?? throw new ArgumentException("Topic not found.");
        EnsureEditable(topic.CourseChapter.CoursePlan);

        var activities = await _db.CourseTopicActivities
            .Where(a => a.CourseTopicId == topicId && !a.IsDeleted)
            .ToListAsync();

        for (var i = 0; i < dto.OrderedIds.Count; i++)
        {
            var a = activities.FirstOrDefault(x => x.CourseTopicActivityId == dto.OrderedIds[i]);
            if (a is null) continue;
            a.SortOrder = i + 1;
            a.UpdatedBy = userId;
            a.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task TagActivityFilesAsync(CourseTopicActivity activity)
    {
        async Task Tag(int? fileId, string label)
        {
            if (fileId is not int fid) return;
            var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == fid && !f.IsDeleted);
            if (file is null) return;
            file.EntityType = "CourseTopicActivity";
            file.EntityId = activity.CourseTopicActivityId;
            file.Label = label;
        }

        await Tag(activity.ReferenceImageFileId, "reference");
        await Tag(activity.ExampleImageFileId, "example");
        await Tag(activity.WorksheetFileId, "worksheet");
    }

    // ── Teacher access ────────────────────────────────────────────────────────

    public async Task<List<CoursePlanListDto>> GetMyCoursesAsync(int teacherId)
    {
        var assignments = await _db.ClassSubjects
            .Where(cs => cs.TeacherId == teacherId && cs.IsActive)
            .Select(cs => new { cs.ClassId, cs.SubjectId })
            .ToListAsync();

        if (assignments.Count == 0) return new();

        var classIds = assignments.Select(a => a.ClassId).Distinct().ToList();
        var subjectIds = assignments.Select(a => a.SubjectId).Distinct().ToList();

        var plans = await _db.CoursePlans.AsNoTracking()
            .Include(p => p.AcademicYear)
            .Include(p => p.Class)
            .Include(p => p.Subject)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Progress)
            .Where(p => !p.IsDeleted && p.Status == "Published"
                        && classIds.Contains(p.ClassId)
                        && subjectIds.Contains(p.SubjectId))
            .ToListAsync();

        var assignedPairs = assignments.Select(a => (a.ClassId, a.SubjectId)).ToHashSet();
        return plans
            .Where(p => assignedPairs.Contains((p.ClassId, p.SubjectId)))
            .Select(MapPlanList)
            .OrderBy(p => p.ClassName).ThenBy(p => p.SubjectName)
            .ToList();
    }

    public async Task<CoursePlanDetailDto?> GetWorkspaceAsync(int planId, int? teacherId, bool requirePublished)
    {
        var plan = await LoadPlanGraphAsync(planId);
        if (plan is null) return null;
        if (requirePublished && plan.Status != "Published") return null;

        if (teacherId is int tid)
        {
            var allowed = await IsTeacherAssignedAsync(tid, plan.ClassId, plan.SubjectId);
            if (!allowed) return null;
        }

        return MapPlanDetail(plan);
    }

    public async Task<CourseTopicDto?> GetTopicDetailAsync(int topicId, int? teacherId, bool requirePublished)
    {
        var topic = await _db.CourseTopics.AsNoTracking()
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
                .ThenInclude(p => p.Class)
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
                .ThenInclude(p => p.Subject)
            .Include(t => t.Progress).ThenInclude(p => p!.CompletedByTeacher)
            .Include(t => t.Materials.Where(m => !m.IsDeleted)).ThenInclude(m => m.FileStore)
            .Include(t => t.Activities.Where(a => !a.IsDeleted)).ThenInclude(a => a.ReferenceImage)
            .Include(t => t.Activities.Where(a => !a.IsDeleted)).ThenInclude(a => a.ExampleImage)
            .Include(t => t.Activities.Where(a => !a.IsDeleted)).ThenInclude(a => a.WorksheetFile)
            .FirstOrDefaultAsync(t => t.CourseTopicId == topicId && !t.IsDeleted);

        if (topic is null || topic.CourseChapter.IsDeleted || topic.CourseChapter.CoursePlan.IsDeleted)
            return null;

        var plan = topic.CourseChapter.CoursePlan;
        if (requirePublished && plan.Status != "Published") return null;

        if (teacherId is int tid)
        {
            var allowed = await IsTeacherAssignedAsync(tid, plan.ClassId, plan.SubjectId);
            if (!allowed) return null;
        }

        return EnrichTopic(MapTopic(topic, topic.Progress), plan);
    }

    private static CourseTopicDto EnrichTopic(CourseTopicDto dto, CoursePlan plan)
    {
        dto.CoursePlanId = plan.CoursePlanId;
        dto.ClassId = plan.ClassId;
        dto.SubjectId = plan.SubjectId;
        dto.AcademicYearId = plan.AcademicYearId;
        dto.ClassName = plan.Class?.ClassName;
        dto.SubjectName = plan.Subject?.SubjectName;
        return dto;
    }

    public async Task<bool> UpdateTopicProgressAsync(int topicId, UpdateTopicProgressDto dto, int teacherId)
    {
        if (!ValidProgressStatuses.Contains(dto.Status))
            throw new ArgumentException("Invalid progress status. Use NotStarted, InProgress, or Completed.");

        var topic = await _db.CourseTopics
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .Include(t => t.Progress)
            .FirstOrDefaultAsync(t => t.CourseTopicId == topicId && !t.IsDeleted)
            ?? throw new ArgumentException("Topic not found.");

        var plan = topic.CourseChapter.CoursePlan;
        if (plan.IsDeleted || plan.Status != "Published")
            throw new ArgumentException("Progress can only be updated on Published course plans.");

        if (!await IsTeacherAssignedAsync(teacherId, plan.ClassId, plan.SubjectId))
            throw new UnauthorizedAccessException("You are not assigned to this class/subject.");

        var progress = topic.Progress;
        if (progress is null)
        {
            progress = new CourseTopicProgress
            {
                CourseTopicId = topicId,
                Status = "NotStarted",
                CreatedBy = teacherId,
                CreatedAt = DateTime.UtcNow
            };
            _db.CourseTopicProgresses.Add(progress);
        }

        var normalized = NormalizeProgressStatus(dto.Status);
        progress.Status = normalized;
        progress.LastUpdatedByTeacherId = teacherId;
        progress.UpdatedBy = teacherId;
        progress.UpdatedAt = DateTime.UtcNow;

        if (normalized == "Completed")
        {
            progress.CompletedAt ??= DateTime.UtcNow;
            progress.CompletedByTeacherId ??= teacherId;
        }
        else
        {
            // Confirmed requirement: leaving Completed clears CompletedAt
            progress.CompletedAt = null;
            progress.CompletedByTeacherId = null;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    // ── Daily Teaching ────────────────────────────────────────────────────────

    public async Task<TeachingLogDto> CreateTeachingLogAsync(CreateTeachingLogDto dto, int teacherId)
    {
        if (!ValidTeachingTypes.Contains(dto.TeachingType))
            throw new ArgumentException("Invalid teaching type. Use NewTopic, Revision, Practice, or Assessment.");

        var topic = await _db.CourseTopics
            .Include(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .Include(t => t.Progress)
            .FirstOrDefaultAsync(t => t.CourseTopicId == dto.CourseTopicId && !t.IsDeleted)
            ?? throw new ArgumentException("Topic not found.");

        var plan = topic.CourseChapter.CoursePlan;
        if (plan.IsDeleted || plan.Status != "Published")
            throw new ArgumentException("Teaching can only be recorded against Published course plans.");

        if (!await IsTeacherAssignedAsync(teacherId, plan.ClassId, plan.SubjectId))
            throw new UnauthorizedAccessException("You are not assigned to this class/subject.");

        // Teaching history is separate from completion. Only move NotStarted → InProgress;
        // never auto-complete and never alter an existing Completed topic.
        var progress = topic.Progress;
        if (progress is null)
        {
            progress = new CourseTopicProgress
            {
                CourseTopicId = topic.CourseTopicId,
                Status = "InProgress",
                LastUpdatedByTeacherId = teacherId,
                CreatedBy = teacherId,
                CreatedAt = DateTime.UtcNow
            };
            _db.CourseTopicProgresses.Add(progress);
        }
        else if (string.Equals(progress.Status, "NotStarted", StringComparison.OrdinalIgnoreCase))
        {
            progress.Status = "InProgress";
            progress.LastUpdatedByTeacherId = teacherId;
            progress.UpdatedBy = teacherId;
            progress.UpdatedAt = DateTime.UtcNow;
        }

        var log = new CourseTeachingLog
        {
            TeachingDate = dto.TeachingDate,
            TeacherId = teacherId,
            ClassId = plan.ClassId,
            SubjectId = plan.SubjectId,
            AcademicYearId = plan.AcademicYearId,
            CourseTopicId = topic.CourseTopicId,
            TeachingType = NormalizeTeachingType(dto.TeachingType),
            Remarks = dto.Remarks?.Trim(),
            ExtraNotes = dto.ExtraNotes?.Trim(),
            CreatedBy = teacherId,
            CreatedAt = DateTime.UtcNow
        };
        _db.CourseTeachingLogs.Add(log);
        await _db.SaveChangesAsync();

        return (await GetTeachingLogsAsync(null, null, null, topic.CourseTopicId, null, null))
            .First(l => l.CourseTeachingLogId == log.CourseTeachingLogId);
    }

    public async Task<List<TeachingLogDto>> GetTeachingLogsAsync(
        int? classId, DateOnly? date, int? subjectId, int? topicId, DateOnly? from, DateOnly? to)
    {
        var q = _db.CourseTeachingLogs.AsNoTracking()
            .Include(l => l.Teacher)
            .Include(l => l.Class)
            .Include(l => l.Subject)
            .Include(l => l.CourseTopic).ThenInclude(t => t.CourseChapter)
            .Include(l => l.Homework)
            .Where(l => !l.IsDeleted)
            .AsQueryable();

        if (classId is > 0) q = q.Where(l => l.ClassId == classId);
        if (subjectId is > 0) q = q.Where(l => l.SubjectId == subjectId);
        if (topicId is > 0) q = q.Where(l => l.CourseTopicId == topicId);
        if (date is not null) q = q.Where(l => l.TeachingDate == date);
        if (from is not null) q = q.Where(l => l.TeachingDate >= from);
        if (to is not null) q = q.Where(l => l.TeachingDate <= to);

        var logs = await q.OrderByDescending(l => l.TeachingDate)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync();

        return logs.Select(MapTeachingLog).ToList();
    }

    public async Task<TeachingLogDto> LinkHomeworkAsync(int teachingLogId, LinkHomeworkDto dto, int teacherId)
    {
        var log = await _db.CourseTeachingLogs
            .Include(l => l.CourseTopic).ThenInclude(t => t.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(l => l.CourseTeachingLogId == teachingLogId && !l.IsDeleted)
            ?? throw new ArgumentException("Teaching log not found.");

        if (log.TeacherId != teacherId)
        {
            // Load role for admin/principal override
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == teacherId);
            var role = user?.Role?.RoleName ?? "";
            if (role is not ("admin" or "principal") && log.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Only the teaching teacher (or admin/principal) can link homework.");
        }

        if (dto.DueDate < dto.AssignedDate)
            throw new ArgumentException("DueDate cannot be before AssignedDate.");

        var homework = new Homework
        {
            ClassId = log.ClassId,
            SubjectId = log.SubjectId,
            TeacherId = teacherId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            AssignedDate = dto.AssignedDate,
            DueDate = dto.DueDate,
            FileId = dto.FileId,
            CourseTopicId = log.CourseTopicId,
            CreatedBy = teacherId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Homeworks.Add(homework);
        await _db.SaveChangesAsync();

        log.HomeworkId = homework.HomeworkId;
        log.UpdatedBy = teacherId;
        log.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return (await GetTeachingLogsAsync(null, null, null, log.CourseTopicId, null, null))
            .First(l => l.CourseTeachingLogId == log.CourseTeachingLogId);
    }

    // ── Progress dashboards ───────────────────────────────────────────────────

    public async Task<List<CurriculumProgressSummaryDto>> GetProgressSummariesAsync(
        int? academicYearId, int? classId, int? subjectId)
    {
        var plans = await GetPlansAsync(academicYearId, classId, subjectId, "Published");
        var result = new List<CurriculumProgressSummaryDto>();
        foreach (var p in plans)
        {
            var detail = await GetProgressDetailAsync(p.CoursePlanId);
            if (detail is not null) result.Add(detail);
        }
        return result;
    }

    public async Task<CurriculumProgressSummaryDto?> GetProgressDetailAsync(int planId)
    {
        var plan = await LoadPlanGraphAsync(planId);
        if (plan is null) return null;

        var chapters = plan.Chapters.Where(c => !c.IsDeleted).OrderBy(c => c.SortOrder).ToList();
        var allTopics = chapters.SelectMany(c => c.Topics.Where(t => !t.IsDeleted)).ToList();
        var completed = allTopics.Count(t => t.Progress?.Status == "Completed");
        var inProgress = allTopics.Count(t => t.Progress?.Status == "InProgress");

        return new CurriculumProgressSummaryDto
        {
            CoursePlanId = plan.CoursePlanId,
            Title = plan.Title,
            YearLabel = plan.AcademicYear.YearLabel,
            ClassName = plan.Class.ClassName,
            Section = plan.Class.Section,
            SubjectName = plan.Subject.SubjectName,
            Status = plan.Status,
            TopicCount = allTopics.Count,
            CompletedTopicCount = completed,
            InProgressTopicCount = inProgress,
            RemainingTopicCount = allTopics.Count - completed,
            Chapters = chapters.Select(c =>
            {
                var topics = c.Topics.Where(t => !t.IsDeleted).OrderBy(t => t.SortOrder).ToList();
                return new ChapterProgressDto
                {
                    CourseChapterId = c.CourseChapterId,
                    Title = c.Title,
                    SortOrder = c.SortOrder,
                    TopicCount = topics.Count,
                    CompletedTopicCount = topics.Count(t => t.Progress?.Status == "Completed"),
                    Topics = topics.Select(t => new TopicProgressDto
                    {
                        CourseTopicId = t.CourseTopicId,
                        Title = t.Title,
                        SortOrder = t.SortOrder,
                        Status = t.Progress?.Status ?? "NotStarted",
                        CompletedAt = t.Progress?.CompletedAt,
                        CompletedByTeacherId = t.Progress?.CompletedByTeacherId,
                        CompletedByTeacherName = t.Progress?.CompletedByTeacher?.FullName
                    }).ToList()
                };
            }).ToList()
        };
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<CoursePlan?> LoadPlanGraphAsync(int id)
    {
        return await _db.CoursePlans.AsNoTracking()
            .Include(p => p.AcademicYear)
            .Include(p => p.Class)
            .Include(p => p.Subject)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Progress)
                        .ThenInclude(pr => pr!.CompletedByTeacher)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Materials.Where(m => !m.IsDeleted))
                        .ThenInclude(m => m.FileStore)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Activities.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ReferenceImage)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Activities.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.ExampleImage)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Topics.Where(t => !t.IsDeleted))
                    .ThenInclude(t => t.Activities.Where(a => !a.IsDeleted))
                        .ThenInclude(a => a.WorksheetFile)
            .Include(p => p.Chapters.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Materials.Where(m => !m.IsDeleted))
                    .ThenInclude(m => m.FileStore)
            .FirstOrDefaultAsync(p => p.CoursePlanId == id && !p.IsDeleted);
    }

    private async Task ValidatePlanKeysAsync(int yearId, int classId, int subjectId)
    {
        if (!await _db.AcademicYears.AnyAsync(y => y.AcademicYearId == yearId && !y.IsDeleted))
            throw new ArgumentException("Academic year not found.");
        if (!await _db.Classes.AnyAsync(c => c.ClassId == classId && !c.IsDeleted))
            throw new ArgumentException("Class not found.");
        if (!await _db.Subjects.AnyAsync(s => s.SubjectId == subjectId && !s.IsDeleted))
            throw new ArgumentException("Subject not found.");
    }

    private async Task<string> BuildDefaultTitleAsync(int yearId, int classId, int subjectId)
    {
        var year = await _db.AcademicYears.AsNoTracking().FirstAsync(y => y.AcademicYearId == yearId);
        var cls = await _db.Classes.AsNoTracking().FirstAsync(c => c.ClassId == classId);
        var sub = await _db.Subjects.AsNoTracking().FirstAsync(s => s.SubjectId == subjectId);
        var section = string.IsNullOrWhiteSpace(cls.Section) ? "" : $"-{cls.Section}";
        return $"{sub.SubjectName} — {cls.ClassName}{section} ({year.YearLabel})";
    }

    private async Task<CoursePlan> RequireEditablePlanAsync(int planId)
    {
        var plan = await _db.CoursePlans.FirstOrDefaultAsync(p => p.CoursePlanId == planId && !p.IsDeleted)
            ?? throw new ArgumentException("Course plan not found.");
        EnsureEditable(plan);
        return plan;
    }

    private static void EnsureEditable(CoursePlan plan)
    {
        if (plan.Status == "Archived")
            throw new ArgumentException("Archived plans cannot be modified.");
    }

    private async Task<bool> IsTeacherAssignedAsync(int teacherId, int classId, int subjectId)
    {
        return await _db.ClassSubjects.AnyAsync(cs =>
            cs.TeacherId == teacherId &&
            cs.ClassId == classId &&
            cs.SubjectId == subjectId &&
            cs.IsActive);
    }

    private async Task<CourseMaterial?> LoadMaterialWithPlanAsync(int id)
    {
        return await _db.CourseMaterials
            .Include(m => m.CourseChapter).ThenInclude(c => c!.CoursePlan)
            .Include(m => m.CourseTopic).ThenInclude(t => t!.CourseChapter).ThenInclude(c => c.CoursePlan)
            .FirstOrDefaultAsync(m => m.CourseMaterialId == id && !m.IsDeleted);
    }

    private static CoursePlan GetPlanFromMaterial(CourseMaterial m) =>
        m.CourseChapter?.CoursePlan
        ?? m.CourseTopic?.CourseChapter.CoursePlan
        ?? throw new InvalidOperationException("Material has no parent plan.");

    private static string NormalizeMaterialType(string? type)
    {
        var t = string.IsNullOrWhiteSpace(type) ? "Other" : type.Trim();
        if (!ValidMaterialTypes.Contains(t))
            throw new ArgumentException("Invalid material type.");
        return ValidMaterialTypes.First(v => v.Equals(t, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeProgressStatus(string status) =>
        ValidProgressStatuses.First(v => v.Equals(status, StringComparison.OrdinalIgnoreCase));

    private static string NormalizeTeachingType(string type) =>
        ValidTeachingTypes.First(v => v.Equals(type, StringComparison.OrdinalIgnoreCase));

    private static CoursePlanListDto MapPlanList(CoursePlan p)
    {
        var topics = p.Chapters.Where(c => !c.IsDeleted)
            .SelectMany(c => c.Topics.Where(t => !t.IsDeleted)).ToList();
        return new CoursePlanListDto
        {
            CoursePlanId = p.CoursePlanId,
            AcademicYearId = p.AcademicYearId,
            YearLabel = p.AcademicYear?.YearLabel ?? "",
            ClassId = p.ClassId,
            ClassName = p.Class?.ClassName ?? "",
            Section = p.Class?.Section,
            SubjectId = p.SubjectId,
            SubjectName = p.Subject?.SubjectName ?? "",
            Title = p.Title,
            Description = p.Description,
            Status = p.Status,
            PublishedAt = p.PublishedAt,
            ChapterCount = p.Chapters.Count(c => !c.IsDeleted),
            TopicCount = topics.Count,
            CompletedTopicCount = topics.Count(t => t.Progress?.Status == "Completed")
        };
    }

    private static CoursePlanDetailDto MapPlanDetail(CoursePlan p)
    {
        var list = MapPlanList(p);
        return new CoursePlanDetailDto
        {
            CoursePlanId = list.CoursePlanId,
            AcademicYearId = list.AcademicYearId,
            YearLabel = list.YearLabel,
            ClassId = list.ClassId,
            ClassName = list.ClassName,
            Section = list.Section,
            SubjectId = list.SubjectId,
            SubjectName = list.SubjectName,
            Title = list.Title,
            Description = list.Description,
            Status = list.Status,
            PublishedAt = list.PublishedAt,
            ChapterCount = list.ChapterCount,
            TopicCount = list.TopicCount,
            CompletedTopicCount = list.CompletedTopicCount,
            Chapters = p.Chapters.Where(c => !c.IsDeleted).OrderBy(c => c.SortOrder)
                .Select(MapChapter).ToList()
        };
    }

    private static CourseChapterDto MapChapter(CourseChapter c) => new()
    {
        CourseChapterId = c.CourseChapterId,
        CoursePlanId = c.CoursePlanId,
        Title = c.Title,
        Description = c.Description,
        SortOrder = c.SortOrder,
        Topics = (c.Topics ?? new List<CourseTopic>())
            .Where(t => !t.IsDeleted).OrderBy(t => t.SortOrder)
            .Select(t => MapTopic(t, t.Progress)).ToList(),
        Materials = (c.Materials ?? new List<CourseMaterial>())
            .Where(m => !m.IsDeleted).OrderBy(m => m.SortOrder)
            .Select(m => MapMaterialSync(m)).ToList()
    };

    private static string NormalizeActivityType(string? type)
    {
        var t = string.IsNullOrWhiteSpace(type) ? "Drawing" : type.Trim();
        if (!ValidActivityTypes.Contains(t))
            throw new ArgumentException("Invalid activity type.");
        return ValidActivityTypes.First(v => v.Equals(t, StringComparison.OrdinalIgnoreCase));
    }

    private static CourseTopicDto MapTopic(CourseTopic t, CourseTopicProgress? progress) => new()
    {
        CourseTopicId = t.CourseTopicId,
        CourseChapterId = t.CourseChapterId,
        Title = t.Title,
        Description = t.Description,
        SortOrder = t.SortOrder,
        ProgressStatus = progress?.Status ?? "NotStarted",
        CompletedAt = progress?.CompletedAt,
        CompletedByTeacherId = progress?.CompletedByTeacherId,
        CompletedByTeacherName = progress?.CompletedByTeacher?.FullName,
        Materials = (t.Materials ?? new List<CourseMaterial>())
            .Where(m => !m.IsDeleted).OrderBy(m => m.SortOrder)
            .Select(MapMaterialSync).ToList(),
        Activities = (t.Activities ?? new List<CourseTopicActivity>())
            .Where(a => !a.IsDeleted).OrderBy(a => a.SortOrder)
            .Select(MapActivitySync).ToList()
    };

    private static CourseTopicActivityDto MapActivitySync(CourseTopicActivity a) => new()
    {
        CourseTopicActivityId = a.CourseTopicActivityId,
        CourseTopicId = a.CourseTopicId,
        ActivityType = a.ActivityType,
        Title = a.Title,
        InstructionText = a.InstructionText,
        ReferenceImageFileId = a.ReferenceImageFileId,
        ReferenceImageName = a.ReferenceImage?.OriginalName,
        ExampleImageFileId = a.ExampleImageFileId,
        ExampleImageName = a.ExampleImage?.OriginalName,
        WorksheetFileId = a.WorksheetFileId,
        WorksheetFileName = a.WorksheetFile?.OriginalName,
        ConfigJson = a.ConfigJson,
        SortOrder = a.SortOrder,
        CanvasEnabled = a.CanvasEnabled
    };

    private async Task<CourseTopicActivityDto> MapActivityAsync(CourseTopicActivity a)
    {
        if (a.ReferenceImageFileId is int r && a.ReferenceImage is null)
            a.ReferenceImage = await _db.FileStores.AsNoTracking().FirstOrDefaultAsync(f => f.FileId == r);
        if (a.ExampleImageFileId is int e && a.ExampleImage is null)
            a.ExampleImage = await _db.FileStores.AsNoTracking().FirstOrDefaultAsync(f => f.FileId == e);
        if (a.WorksheetFileId is int w && a.WorksheetFile is null)
            a.WorksheetFile = await _db.FileStores.AsNoTracking().FirstOrDefaultAsync(f => f.FileId == w);
        return MapActivitySync(a);
    }

    private static CourseMaterialDto MapMaterialSync(CourseMaterial m) => new()
    {
        CourseMaterialId = m.CourseMaterialId,
        CourseChapterId = m.CourseChapterId,
        CourseTopicId = m.CourseTopicId,
        Title = m.Title,
        MaterialType = m.MaterialType,
        Content = m.Content,
        Url = m.Url,
        FileStoreId = m.FileStoreId,
        FileName = m.FileStore?.OriginalName,
        SortOrder = m.SortOrder
    };

    private async Task<CourseMaterialDto> MapMaterialAsync(CourseMaterial m)
    {
        if (m.FileStoreId is int fid && m.FileStore is null)
            m.FileStore = await _db.FileStores.AsNoTracking().FirstOrDefaultAsync(f => f.FileId == fid);
        return MapMaterialSync(m);
    }

    private static TeachingLogDto MapTeachingLog(CourseTeachingLog l) => new()
    {
        CourseTeachingLogId = l.CourseTeachingLogId,
        TeachingDate = l.TeachingDate,
        TeacherId = l.TeacherId,
        TeacherName = l.Teacher?.FullName ?? "",
        ClassId = l.ClassId,
        ClassName = l.Class?.ClassName ?? "",
        Section = l.Class?.Section,
        SubjectId = l.SubjectId,
        SubjectName = l.Subject?.SubjectName ?? "",
        AcademicYearId = l.AcademicYearId,
        CourseTopicId = l.CourseTopicId,
        TopicTitle = l.CourseTopic?.Title ?? "",
        ChapterTitle = l.CourseTopic?.CourseChapter?.Title,
        TeachingType = l.TeachingType,
        Remarks = l.Remarks,
        ExtraNotes = l.ExtraNotes,
        HomeworkId = l.HomeworkId,
        HomeworkTitle = l.Homework?.Title
    };
}
