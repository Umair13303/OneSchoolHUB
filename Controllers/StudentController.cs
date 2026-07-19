using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Students;
using SchoolManagement.API.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 3 — Student Admission. Routes mirror the task doc:
///   POST   /api/students                  → new admission
///   GET    /api/students                  → list with filters (class, year)
///   GET    /api/students/{id}             → detail
///   PUT    /api/students/{id}             → update
///   GET    /api/students/{id}/documents   → documents list
/// </summary>
[ApiController]
[Route("api/students")]
[Authorize]
[Produces("application/json")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly AppDbContext _db;
    private readonly IParentAccessService _parentAccess;

    public StudentController(IStudentService service, AppDbContext db, IParentAccessService parentAccess)
    {
        _service = service;
        _db = db;
        _parentAccess = parentAccess;
    }

    /// <summary>
    /// Self-service: the students linked to the logged-in parent's account
    /// via StudentGuardians.UserId. This is what a Parent-role mobile/web
    /// client should call first to discover which studentId(s)/classId(s)
    /// it's allowed to query on the other self-service endpoints.
    /// GET /api/students/my-children
    /// </summary>
    [HttpGet("my-children")]
    [Authorize(Roles = "parent")]
    public async Task<IActionResult> GetMyChildren()
    {
        var studentIds = await _parentAccess.GetChildStudentIdsAsync(CurrentUserId());
        if (studentIds.Count == 0)
            return Ok(new List<DTOs.Students.StudentListDto>());

        var children = await _db.Students.AsNoTracking()
            .Where(s => studentIds.Contains(s.StudentId))
            .Select(s => new DTOs.Students.StudentListDto
            {
                StudentId   = s.StudentId,
                AdmissionNo = s.AdmissionNo,
                FullName    = (s.FirstName + " " + s.LastName).Trim(),
                DateOfBirth = s.DateOfBirth,
                Gender      = s.Gender,
                PhotoFileId = s.PhotoFileId,
                IsActive    = s.IsActive,
                ClassId = s.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => (int?)e.ClassId).FirstOrDefault(),
                ClassName = s.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => e.Class.ClassName).FirstOrDefault(),
                Section = s.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => e.Class.Section).FirstOrDefault(),
                AcademicYearId = s.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => (int?)e.AcademicYearId).FirstOrDefault(),
                AcademicYearLabel = s.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => e.AcademicYear.YearLabel).FirstOrDefault()
            })
            .ToListAsync();

        return Ok(children);
    }

    /// <summary>List students. Teachers can list to mark attendance; admins can filter freely.</summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? classId,
        [FromQuery] int? academicYearId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
        => Ok(await _service.GetAllAsync(classId, academicYearId, search, page, pageSize));

    [HttpGet("{id:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:int}/documents")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetDocuments(int id)
        => Ok(await _service.GetDocumentsAsync(id));

    /// <summary>
    /// Preview of the admission number the next admission will receive, based
    /// on the institute/campus admission-number settings. The number is only
    /// reserved at create time (with a uniqueness retry), so this is a
    /// suggestion for the admission form, not a reservation.
    /// GET /api/students/next-admission-no
    /// </summary>
    [HttpGet("next-admission-no")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> GetNextAdmissionNo()
        => Ok(new { admissionNo = await _service.GetNextAdmissionNumberAsync() });

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateStudentAdmissionDto dto)
    {
        try
        {
            var created = await _service.CreateAdmissionAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.StudentId }, created);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
        catch (Exception ex)               { return StatusCode(500, new { error = ex.Message, detail = ex.InnerException?.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
    {
        var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    /// Check outstanding dues for a student before withdrawal.
    /// GET /api/students/{id}/dues
    /// </summary>
    [HttpGet("{id:int}/dues")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetDues(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student is null) return NotFound();

        var dues = await _db.StudentFees
            .Include(f => f.FeeStructure).ThenInclude(fs => fs.FeeType)
            .Where(f => f.StudentId == id && (f.Status == "Unpaid" || f.Status == "Partial"))
            .Select(f => new {
                f.StudentFeeId,
                FeeTypeName = f.FeeStructure.FeeType.Name,
                f.AmountDue,
                f.AmountPaid,
                f.Discount,
                Balance = f.AmountDue - f.AmountPaid - f.Discount,
                f.DueDate,
                f.Status
            })
            .ToListAsync();

        return Ok(new {
            studentId   = id,
            fullName    = student.FirstName + " " + student.LastName,
            admissionNo = student.AdmissionNo,
            totalDues   = dues.Sum(d => d.Balance),
            hasDues     = dues.Any(),
            dues
        });
    }

    /// <summary>
    /// Withdraw a student from school.
    /// POST /api/students/{id}/withdraw
    /// Body: { leavingDate, leavingReason, forceWithdraw }
    /// Returns 409 if there are unpaid dues and forceWithdraw is false.
    /// </summary>
    [HttpPost("{id:int}/withdraw")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Withdraw(int id, [FromBody] WithdrawStudentDto dto)
    {
        var student = await _db.Students
            .Include(s => s.Guardians)
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id && !s.IsDeleted);

        if (student is null) return NotFound();
        if (!student.IsActive)
            return BadRequest(new { error = "Student is already inactive/withdrawn." });

        // Check unpaid dues
        var totalDues = await _db.StudentFees
            .Where(f => f.StudentId == id && (f.Status == "Unpaid" || f.Status == "Partial"))
            .SumAsync(f => f.AmountDue - f.AmountPaid - f.Discount);

        if (totalDues > 0 && !dto.ForceWithdraw)
            return Conflict(new { error = $"Student has outstanding dues of {totalDues:0.00}. Clear dues or use force withdraw.", totalDues });

        // Mark student inactive
        student.IsActive      = false;
        student.LeavingDate   = dto.LeavingDate;
        student.LeavingReason = dto.LeavingReason;

        // Mark active enrollments as Withdrawn
        foreach (var enr in student.Enrollments.Where(e => e.Status == "Active"))
            enr.Status = "Withdrawn";

        await _db.SaveChangesAsync();

        // Return data needed for leaving certificate
        var guardian = student.Guardians.FirstOrDefault();
        var enrollment = student.Enrollments
            .OrderByDescending(e => e.AcademicYearId)
            .FirstOrDefault();

        string? className = null;
        string? yearLabel = null;
        if (enrollment is not null)
        {
            var cls = await _db.Classes.FindAsync(enrollment.ClassId);
            var yr  = await _db.AcademicYears.FindAsync(enrollment.AcademicYearId);
            className = cls is null ? null : cls.ClassName + (cls.Section is not null ? " " + cls.Section : "");
            yearLabel = yr?.YearLabel;
        }

        return Ok(new {
            studentId       = student.StudentId,
            admissionNo     = student.AdmissionNo,
            fullName        = student.FirstName + " " + student.LastName,
            dateOfBirth     = student.DateOfBirth?.ToString("yyyy-MM-dd"),
            gender          = student.Gender,
            admissionDate   = student.AdmissionDate.ToString("yyyy-MM-dd"),
            leavingDate     = student.LeavingDate?.ToString("yyyy-MM-dd"),
            leavingReason   = student.LeavingReason,
            className,
            yearLabel,
            guardianName    = guardian?.FullName,
            guardianPhone   = guardian?.Phone,
            totalDuesCleared = totalDues
        });
    }

    /// <summary>
    /// Detect siblings: find existing students sharing a guardian with the given CNIC.
    /// Returns count and sibling order (so new student is order = count + 1).
    /// </summary>
    [HttpGet("sibling-check")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> SiblingCheck([FromQuery] string cnic)
    {
        if (string.IsNullOrWhiteSpace(cnic))
            return Ok(new { siblingCount = 0, siblingOrder = 1, siblings = new List<object>() });

        var normalizedCnic = cnic.Trim();

        var siblings = await _db.StudentGuardians
            .Include(g => g.Student)
            .Where(g => g.CNIC == normalizedCnic && !g.Student.IsDeleted)
            .Select(g => new { g.Student.StudentId, g.Student.AdmissionNo, Name = g.Student.FirstName + " " + g.Student.LastName })
            .Distinct()
            .ToListAsync();

        return Ok(new
        {
            siblingCount = siblings.Count,
            siblingOrder = siblings.Count + 1,
            siblings
        });
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
