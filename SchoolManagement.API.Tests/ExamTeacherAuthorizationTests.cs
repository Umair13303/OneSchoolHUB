using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Tests;

/// <summary>
/// Focused ClassSubject authorization coverage for formal Exam APIs.
/// </summary>
public class ExamTeacherAuthorizationTests : IDisposable
{
    private const int TeacherA = 101;
    private const int TeacherB = 102;
    private const int AdminUser = 201;

    private const int Class1 = 1;
    private const int Class2 = 2;
    private const int SubjectMath = 10;
    private const int SubjectEng = 11;

    private readonly AppDbContext _db;
    private readonly ExamService _svc;

    public ExamTeacherAuthorizationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ExamAuth_" + Guid.NewGuid().ToString("N"))
            .Options;
        _db = new AppDbContext(options);
        Seed();
        _svc = new ExamService(_db);
    }

    public void Dispose() => _db.Dispose();

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

        // Teacher A: Class1+Math only
        // Teacher B: Class1+Eng and Class2+Math
        _db.ClassSubjects.AddRange(
            new ClassSubject { Id = 1, ClassId = Class1, SubjectId = SubjectMath, TeacherId = TeacherA, IsActive = true },
            new ClassSubject { Id = 2, ClassId = Class1, SubjectId = SubjectEng, TeacherId = TeacherB, IsActive = true },
            new ClassSubject { Id = 3, ClassId = Class2, SubjectId = SubjectMath, TeacherId = TeacherB, IsActive = true });

        _db.ExamPapers.AddRange(
            new ExamPaper
            {
                ExamPaperId = 1,
                AcademicYearId = 1,
                ClassId = Class1,
                SubjectId = SubjectMath,
                CreatedByUserId = TeacherA,
                ExamType = ExamType.MonthlyTest,
                ClassGroup = ClassGroup.Matric,
                Title = "A Math Paper",
                TotalMarks = 100,
                PassMarks = 33,
                DurationMinutes = 60
            },
            new ExamPaper
            {
                ExamPaperId = 2,
                AcademicYearId = 1,
                ClassId = Class1,
                SubjectId = SubjectEng,
                CreatedByUserId = TeacherB,
                ExamType = ExamType.MonthlyTest,
                ClassGroup = ClassGroup.Matric,
                Title = "B Eng Paper",
                TotalMarks = 100,
                PassMarks = 33,
                DurationMinutes = 60
            },
            new ExamPaper
            {
                ExamPaperId = 3,
                AcademicYearId = 1,
                ClassId = Class2,
                SubjectId = SubjectMath,
                CreatedByUserId = TeacherB,
                ExamType = ExamType.MonthlyTest,
                ClassGroup = ClassGroup.Matric,
                Title = "B Math Other Class",
                TotalMarks = 100,
                PassMarks = 33,
                DurationMinutes = 60
            });

        _db.Students.Add(new Student
        {
            StudentId = 50,
            FirstName = "Ali",
            LastName = "Khan",
            AdmissionNo = "A-50"
        });

        _db.ExamResults.AddRange(
            new ExamResult
            {
                ExamResultId = 1,
                ExamPaperId = 1,
                StudentId = 50,
                ObtainedMarks = 80,
                Percentage = 80,
                Grade = "A",
                IsPass = true,
                EnteredByUserId = TeacherA,
                EnteredAt = DateTime.UtcNow
            },
            new ExamResult
            {
                ExamResultId = 2,
                ExamPaperId = 2,
                StudentId = 50,
                ObtainedMarks = 70,
                Percentage = 70,
                Grade = "B",
                IsPass = true,
                EnteredByUserId = TeacherB,
                EnteredAt = DateTime.UtcNow
            });

        // Curriculum graph for Question Bank: Plan Class1+Math → Chapter → Topic → QB item
        _db.CoursePlans.Add(new CoursePlan
        {
            CoursePlanId = 1,
            AcademicYearId = 1,
            ClassId = Class1,
            SubjectId = SubjectMath,
            Title = "Math Plan",
            Status = "Published"
        });
        _db.CoursePlans.Add(new CoursePlan
        {
            CoursePlanId = 2,
            AcademicYearId = 1,
            ClassId = Class1,
            SubjectId = SubjectEng,
            Title = "Eng Plan",
            Status = "Published"
        });
        _db.CourseChapters.AddRange(
            new CourseChapter { CourseChapterId = 1, CoursePlanId = 1, Title = "Ch1", SortOrder = 1 },
            new CourseChapter { CourseChapterId = 2, CoursePlanId = 2, Title = "Ch1", SortOrder = 1 });
        _db.CourseTopics.AddRange(
            new CourseTopic { CourseTopicId = 1, CourseChapterId = 1, Title = "Algebra", SortOrder = 1 },
            new CourseTopic { CourseTopicId = 2, CourseChapterId = 2, Title = "Grammar", SortOrder = 1 });
        _db.QuestionBankItems.AddRange(
            new QuestionBankItem
            {
                QuestionBankItemId = 1,
                CourseTopicId = 1,
                QuestionType = QuestionType.ShortQuestion,
                QuestionText = "Math Q",
                Marks = 2,
                IsActive = true
            },
            new QuestionBankItem
            {
                QuestionBankItemId = 2,
                CourseTopicId = 2,
                QuestionType = QuestionType.ShortQuestion,
                QuestionText = "Eng Q",
                Marks = 2,
                IsActive = true
            });

        _db.SaveChanges();
    }

    [Fact]
    public async Task Teacher_AssignedClassSubject_Allow()
    {
        await _svc.EnsureTeacherCanAccessClassSubjectAsync(TeacherA, Class1, SubjectMath);
        await _svc.EnsureTeacherCanAccessPaperAsync(TeacherA, 1);
    }

    [Fact]
    public async Task Teacher_UnassignedClassSubject_Deny()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _svc.EnsureTeacherCanAccessClassSubjectAsync(TeacherA, Class2, SubjectEng));
    }

    [Fact]
    public async Task Teacher_SameClass_DifferentSubject_Deny()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _svc.EnsureTeacherCanAccessClassSubjectAsync(TeacherA, Class1, SubjectEng));
    }

    [Fact]
    public async Task Teacher_SameSubject_DifferentClass_Deny()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _svc.EnsureTeacherCanAccessClassSubjectAsync(TeacherA, Class2, SubjectMath));
    }

    [Fact]
    public async Task Teacher_AnotherTeachersPaperId_Deny()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _svc.EnsureTeacherCanAccessPaperAsync(TeacherA, 2));
    }

    [Fact]
    public async Task Teacher_AnotherTeachersResultPaper_Deny_And_FilterStudentResults()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _svc.EnsureTeacherCanAccessPaperAsync(TeacherA, 2));

        var results = await _svc.GetStudentResultsAsync(50, null, TeacherA);
        Assert.Single(results);
        Assert.Equal(1, results[0].ExamPaperId);
    }

    [Fact]
    public async Task Teacher_AnotherTeachersQuestionBank_Deny()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _svc.EnsureTeacherCanAccessQuestionBankItemAsync(TeacherA, 2));

        var bank = await _svc.ListQuestionBankAsync(null, null, null, TeacherA);
        Assert.Single(bank);
        Assert.Equal(1, bank[0].QuestionBankItemId);
    }

    [Fact]
    public async Task Admin_ListPapers_Unscoped_Preserved()
    {
        // Admin/Principal path: no teacherId filter → all papers
        var all = await _svc.GetPapersAsync(null, null, null, null, teacherId: null);
        Assert.Equal(3, all.Count);
    }

    [Fact]
    public async Task Principal_ListPapers_Unscoped_Preserved()
    {
        var all = await _svc.GetPapersAsync(null, null, null, null, teacherId: null);
        Assert.Equal(3, all.Count);

        var teacherScoped = await _svc.GetPapersAsync(null, null, null, null, TeacherA);
        Assert.Single(teacherScoped);
        Assert.Equal(1, teacherScoped[0].ExamPaperId);
    }
}
