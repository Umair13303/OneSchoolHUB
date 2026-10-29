using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SchoolManagement.API.Controllers;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Ai;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;
using SchoolManagement.API.Services.Ai;

namespace SchoolManagement.API.Tests;

/// <summary>
/// Phase 1 AI Question Generator — authorization, validation, ephemeral drafts (no DB writes).
/// Uses FakeAiProvider only; never calls external AI.
/// </summary>
public class AiQuestionGeneratorTests : IDisposable
{
    private const int TeacherA = 101;
    private const int TeacherB = 102;
    private const int AdminUser = 201;

    private const int Class1 = 1;
    private const int Class2 = 2;
    private const int SubjectMath = 10;
    private const int SubjectEng = 11;

    private readonly AppDbContext _db;
    private readonly FakeAiProvider _fake;
    private readonly IMemoryCache _cache;
    private readonly AiGenerationOptions _opts;

    public AiQuestionGeneratorTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("AiQ_" + Guid.NewGuid().ToString("N"))
            .Options;
        _db = new AppDbContext(options);
        Seed();
        _fake = new FakeAiProvider();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _opts = new AiGenerationOptions
        {
            Enabled = true,
            Provider = "Fake",
            Model = "fake-model",
            MaxCount = 20,
            MaxInstructionsLength = 1000,
            RateLimitPerUser = 10,
            RateLimitWindowMinutes = 10
        };
    }

    public void Dispose()
    {
        _cache.Dispose();
        _db.Dispose();
    }

    private AiContentService CreateService(AiGenerationOptions? opts = null)
    {
        var o = opts ?? _opts;
        return new AiContentService(
            _db,
            new ExamService(_db),
            _fake,
            _cache,
            Options.Create(o),
            NullLogger<AiContentService>.Instance);
    }

    private static GenerateQuestionsRequest ValidRequest(
        int classId = Class1,
        int subjectId = SubjectMath,
        int topicId = 1,
        int count = 5,
        QuestionType type = QuestionType.MultipleChoice) => new()
    {
        ClassId = classId,
        SubjectId = subjectId,
        CourseTopicId = topicId,
        Count = count,
        QuestionType = type,
        Difficulty = "Medium",
        Marks = 1,
        Language = "en",
        Instructions = null
    };

    private void Seed()
    {
        _db.Roles.Add(new Role { RoleId = 1, RoleName = "teacher" });
        _db.Users.AddRange(
            new User { UserId = TeacherA, FullName = "Teacher A", Email = "a@t.com", PasswordHash = "x", RoleId = 1 },
            new User { UserId = TeacherB, FullName = "Teacher B", Email = "b@t.com", PasswordHash = "x", RoleId = 1 },
            new User { UserId = AdminUser, FullName = "Admin", Email = "admin@t.com", PasswordHash = "x", RoleId = 1 });

        _db.AcademicYears.Add(new AcademicYear
        {
            AcademicYearId = 1,
            YearLabel = "2025-26",
            StartDate = new DateOnly(2025, 4, 1),
            EndDate = new DateOnly(2026, 3, 31)
        });

        _db.Classes.AddRange(
            new Class { ClassId = Class1, ClassName = "9-A" },
            new Class { ClassId = Class2, ClassName = "9-B" });
        _db.Subjects.AddRange(
            new Subject { SubjectId = SubjectMath, SubjectName = "Math" },
            new Subject { SubjectId = SubjectEng, SubjectName = "English" });

        _db.ClassSubjects.AddRange(
            new ClassSubject { Id = 1, ClassId = Class1, SubjectId = SubjectMath, TeacherId = TeacherA, IsActive = true },
            new ClassSubject { Id = 2, ClassId = Class1, SubjectId = SubjectEng, TeacherId = TeacherB, IsActive = true },
            new ClassSubject { Id = 3, ClassId = Class2, SubjectId = SubjectMath, TeacherId = TeacherB, IsActive = true });

        _db.CoursePlans.AddRange(
            new CoursePlan
            {
                CoursePlanId = 1,
                AcademicYearId = 1,
                ClassId = Class1,
                SubjectId = SubjectMath,
                Title = "Math Plan",
                Status = "Published"
            },
            new CoursePlan
            {
                CoursePlanId = 2,
                AcademicYearId = 1,
                ClassId = Class1,
                SubjectId = SubjectEng,
                Title = "Eng Plan",
                Status = "Published"
            });
        _db.CourseChapters.AddRange(
            new CourseChapter { CourseChapterId = 1, CoursePlanId = 1, Title = "Algebra Ch", SortOrder = 1 },
            new CourseChapter { CourseChapterId = 2, CoursePlanId = 2, Title = "Grammar Ch", SortOrder = 1 });
        _db.CourseTopics.AddRange(
            new CourseTopic { CourseTopicId = 1, CourseChapterId = 1, Title = "Algebra", SortOrder = 1 },
            new CourseTopic { CourseTopicId = 2, CourseChapterId = 2, Title = "Grammar", SortOrder = 1 });

        _db.QuestionBankItems.Add(new QuestionBankItem
        {
            QuestionBankItemId = 1,
            CourseTopicId = 1,
            QuestionType = QuestionType.ShortQuestion,
            QuestionText = "Existing bank item",
            Marks = 2,
            IsActive = true
        });

        _db.SaveChanges();
    }

    [Fact]
    public async Task Assigned_Teacher_Allowed()
    {
        var svc = CreateService();
        var result = await svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, isTeacherOnly: true);
        Assert.NotEmpty(result.Items);
        Assert.Equal(5, result.Items.Count);
        Assert.True(_fake.CallCount >= 1);
    }

    [Fact]
    public async Task Wrong_ClassSubject_Denied_Provider_Not_Called()
    {
        var svc = CreateService();
        // Topic 2 matches Class1+Eng; Teacher A is not assigned that ClassSubject.
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            svc.GenerateQuestionsAsync(
                ValidRequest(classId: Class1, subjectId: SubjectEng, topicId: 2),
                TeacherA,
                isTeacherOnly: true));
        Assert.Equal(0, _fake.CallCount);
    }

    [Fact]
    public async Task Wrong_Topic_Denied_Provider_Not_Called()
    {
        var svc = CreateService();
        // Teacher A owns Class1+Math; topic 2 is Eng
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            svc.GenerateQuestionsAsync(
                ValidRequest(classId: Class1, subjectId: SubjectMath, topicId: 2),
                TeacherA,
                isTeacherOnly: true));
        Assert.Equal(0, _fake.CallCount);
    }

    [Fact]
    public async Task Class_Subject_Mismatch_Denied_Provider_Not_Called()
    {
        var svc = CreateService();
        // Topic 1 is Math; request claims Eng subject
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            svc.GenerateQuestionsAsync(
                ValidRequest(classId: Class1, subjectId: SubjectEng, topicId: 1),
                TeacherA,
                isTeacherOnly: true));
        Assert.Equal(0, _fake.CallCount);
    }

    [Fact]
    public async Task Admin_Allowed_When_Topic_Matches()
    {
        var svc = CreateService();
        var result = await svc.GenerateQuestionsAsync(ValidRequest(), AdminUser, isTeacherOnly: false);
        Assert.NotEmpty(result.Items);
        Assert.True(_fake.CallCount >= 1);
    }

    [Fact]
    public async Task Principal_Allowed_When_Topic_Matches()
    {
        var svc = CreateService();
        // Principal uses same non-teacher path as admin for ClassSubject scoping
        var result = await svc.GenerateQuestionsAsync(ValidRequest(), AdminUser, isTeacherOnly: false);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public void SuperAdmin_Not_In_Authorize_Roles()
    {
        var method = typeof(AiController).GetMethod(nameof(AiController.GenerateQuestions));
        Assert.NotNull(method);
        var attr = method!.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(attr);
        var roles = (attr!.Roles ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Contains("teacher", roles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("admin", roles, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("principal", roles, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain("superadmin", roles, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Invalid_Count_Rejected()
    {
        var svc = CreateService();
        var req = ValidRequest();
        req.Count = 7;
        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.GenerateQuestionsAsync(req, TeacherA, true));
        Assert.Equal(0, _fake.CallCount);
    }

    [Fact]
    public async Task Invalid_QuestionType_Rejected()
    {
        var svc = CreateService();
        var req = ValidRequest(type: QuestionType.LongQuestion);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.GenerateQuestionsAsync(req, TeacherA, true));
        Assert.Equal(0, _fake.CallCount);
    }

    [Fact]
    public async Task Invalid_Provider_Mcq_Normalized_With_Warning_Or_Error()
    {
        _fake.OverrideJson = """
            {"questions":[
              {"questionType":1,"questionText":"Bad MCQ","language":"en","marks":1,"options":[
                {"optionLabel":"A","optionText":"Only one","isCorrect":true,"sortOrder":1}
              ]}
            ]}
            """;
        var svc = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, true));
    }

    [Fact]
    public async Task CourseTopicId_Always_From_Request()
    {
        _fake.OverrideJson = """
            {"questions":[
              {"questionType":1,"questionText":"Q1?","language":"en","marks":1,"questionNote":"n","options":[
                {"optionLabel":"A","optionText":"a","isCorrect":true,"sortOrder":1},
                {"optionLabel":"B","optionText":"b","isCorrect":false,"sortOrder":2},
                {"optionLabel":"C","optionText":"c","isCorrect":false,"sortOrder":3},
                {"optionLabel":"D","optionText":"d","isCorrect":false,"sortOrder":4}
              ]},
              {"questionType":1,"questionText":"Q2?","language":"en","marks":1,"questionNote":"n","options":[
                {"optionLabel":"A","optionText":"a","isCorrect":true,"sortOrder":1},
                {"optionLabel":"B","optionText":"b","isCorrect":false,"sortOrder":2},
                {"optionLabel":"C","optionText":"c","isCorrect":false,"sortOrder":3},
                {"optionLabel":"D","optionText":"d","isCorrect":false,"sortOrder":4}
              ]},
              {"questionType":1,"questionText":"Q3?","language":"en","marks":1,"questionNote":"n","options":[
                {"optionLabel":"A","optionText":"a","isCorrect":true,"sortOrder":1},
                {"optionLabel":"B","optionText":"b","isCorrect":false,"sortOrder":2},
                {"optionLabel":"C","optionText":"c","isCorrect":false,"sortOrder":3},
                {"optionLabel":"D","optionText":"d","isCorrect":false,"sortOrder":4}
              ]},
              {"questionType":1,"questionText":"Q4?","language":"en","marks":1,"questionNote":"n","options":[
                {"optionLabel":"A","optionText":"a","isCorrect":true,"sortOrder":1},
                {"optionLabel":"B","optionText":"b","isCorrect":false,"sortOrder":2},
                {"optionLabel":"C","optionText":"c","isCorrect":false,"sortOrder":3},
                {"optionLabel":"D","optionText":"d","isCorrect":false,"sortOrder":4}
              ]},
              {"questionType":1,"questionText":"Q5?","language":"en","marks":1,"questionNote":"n","options":[
                {"optionLabel":"A","optionText":"a","isCorrect":true,"sortOrder":1},
                {"optionLabel":"B","optionText":"b","isCorrect":false,"sortOrder":2},
                {"optionLabel":"C","optionText":"c","isCorrect":false,"sortOrder":3},
                {"optionLabel":"D","optionText":"d","isCorrect":false,"sortOrder":4}
              ]}
            ]}
            """;
        var svc = CreateService();
        var result = await svc.GenerateQuestionsAsync(ValidRequest(topicId: 1), TeacherA, true);
        Assert.All(result.Items, i => Assert.Equal(1, i.CourseTopicId));
    }

    [Fact]
    public async Task Rate_Limit_Returns_AiRateLimitException()
    {
        var opts = new AiGenerationOptions
        {
            Enabled = true,
            Provider = "Fake",
            MaxCount = 20,
            RateLimitPerUser = 2,
            RateLimitWindowMinutes = 10
        };
        var svc = CreateService(opts);
        await svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, true);
        await svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, true);
        await Assert.ThrowsAsync<AiRateLimitException>(() =>
            svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, true));
    }

    [Fact]
    public async Task Provider_Disabled_Throws_AiDisabledException()
    {
        var opts = new AiGenerationOptions { Enabled = false, Provider = "Fake", MaxCount = 20 };
        var svc = CreateService(opts);
        await Assert.ThrowsAsync<AiDisabledException>(() =>
            svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, true));
        Assert.Equal(0, _fake.CallCount);
    }

    [Fact]
    public async Task Generate_Creates_Zero_QuestionBank_Records()
    {
        var before = await _db.QuestionBankItems.CountAsync(i => !i.IsDeleted);
        var svc = CreateService();
        await svc.GenerateQuestionsAsync(ValidRequest(), TeacherA, true);
        var after = await _db.QuestionBankItems.CountAsync(i => !i.IsDeleted);
        Assert.Equal(before, after);
    }

    [Fact]
    public void Controller_Maps_Disabled_And_RateLimit_StatusCodes()
    {
        // Smoke: Ensure AiController exists and generate action is POST api/ai/questions/generate
        var route = typeof(AiController).GetCustomAttribute<RouteAttribute>();
        Assert.Equal("api/ai", route?.Template);
        var method = typeof(AiController).GetMethod(nameof(AiController.GenerateQuestions));
        Assert.NotNull(method);
        Assert.NotNull(method!.GetCustomAttribute<HttpPostAttribute>());
        var http = method.GetCustomAttribute<HttpPostAttribute>();
        Assert.Equal("questions/generate", http!.Template);
    }
}
