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
    private readonly IPdfReportService _pdf;

    public StudentController(
        IStudentService service,
        AppDbContext db,
        IParentAccessService parentAccess,
        IPdfReportService pdf)
    {
        _service = service;
        _db = db;
        _parentAccess = parentAccess;
        _pdf = pdf;
    }

    /// <summary>
    /// Self-service: the students linked to the logged-in parent's account
    /// via StudentGuardians.UserId. This is what a Parent-role mobile/web
    /// client should call first to discover which studentId(s)/classId(s)
    /// it's allowed to query on the other self-service endpoints.
    /// GET /api/students/my-children
    /// </summary>
    [HttpGet("my-children")]
    [Authorize(Roles = "parent,teacher,staff,admin,principal")]
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
                Phone       = s.Guardians
                    .Where(g => !g.IsDeleted)
                    .OrderBy(g => g.GuardianId)
                    .Select(g => g.Phone)
                    .FirstOrDefault(),
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
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] bool includeDeleted = false)
        => Ok(await _service.GetAllAsync(classId, academicYearId, search, isActive, page, pageSize, includeDeleted));

    /// <summary>Export filtered students as .xlsx (same columns as import, plus class/status).</summary>
    [HttpGet("export")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> Export(
        [FromQuery] int? classId,
        [FromQuery] int? academicYearId,
        [FromQuery] string? search,
        [FromQuery] bool? isActive)
    {
        var bytes = await _service.ExportAsync(classId, academicYearId, search, isActive);
        var name = $"Students_{DateTime.UtcNow:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name);
    }

    /// <summary>Bulk promote students from one class/year into another.</summary>
    [HttpPost("promote-bulk")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> PromoteBulk([FromBody] BulkPromoteDto dto)
    {
        try
        {
            var (promoted, skipped) = await _service.PromoteBulkAsync(dto, CurrentUserId());
            return Ok(new { promoted, skipped });
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent,staff")]
    public async Task<IActionResult> GetById(int id)
    {
        // Parents (and teacher-as-parent) may only open their own linked children.
        if (User.IsInRole("parent") || User.IsInRole("teacher") || User.IsInRole("staff"))
        {
            var isStaffAdminView = User.IsInRole("admin") || User.IsInRole("principal") || User.IsInRole("superadmin");
            if (!isStaffAdminView)
            {
                var isTeacher = User.IsInRole("teacher");
                var isGuardian = await _parentAccess.IsGuardianOfAsync(CurrentUserId(), id);
                // Pure parent / staff-parent: must be guardian. Teacher may view any for class work,
                // OR their own child when linked.
                if (!isTeacher && !isGuardian)
                    return Forbid();
            }
        }

        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:int}/documents")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetDocuments(int id)
        => Ok(await _service.GetDocumentsAsync(id));

    [HttpPost("{id:int}/documents")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> TagDocument(int id, [FromBody] TagStudentDocumentDto dto)
    {
        try { return Ok(await _service.TagDocumentAsync(id, dto)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{id:int}/transfer")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Transfer(int id, [FromBody] TransferStudentDto dto)
    {
        try { return Ok(await _service.TransferAsync(id, dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:int}/guardians/{guardianId:int}/portal")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> LinkGuardianPortal(int id, int guardianId, [FromBody] GuardianPortalDto dto)
    {
        try { return Ok(await _service.LinkGuardianPortalAsync(id, guardianId, dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Clear StudentGuardians.UserId (does not delete the parent user account).</summary>
    [HttpDelete("{id:int}/guardians/{guardianId:int}/portal")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UnlinkGuardianPortal(int id, int guardianId)
    {
        var ok = await _service.UnlinkGuardianPortalAsync(id, guardianId);
        return ok ? NoContent() : NotFound();
    }

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
        try
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Soft-delete a student (hidden from normal lists; enrollments withdrawn).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var ok = await _service.SoftDeleteAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Restore a soft-deleted student (still inactive until re-admit).</summary>
    [HttpPost("{id:int}/restore")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Restore(int id)
    {
        var ok = await _service.RestoreAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Server-rendered student ID card PDF (front + back on one page).</summary>
    [HttpGet("{id:int}/id-card.pdf")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> IdCardPdf(int id)
    {
        var data = await _service.GetIdCardDataAsync(id);
        if (data is null) return NotFound();
        var bytes = _pdf.StudentIdCardPdf(data);
        return File(bytes, "application/pdf", $"student-id-card-{id}.pdf");
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

        var digits = new string(cnic.Where(char.IsDigit).ToArray());
        var normalizedCnic = digits.Length >= 5 ? digits : cnic.Trim();

        // Match raw CNIC, digit-only key, or SiblingGroupKey already persisted.
        var siblings = await _db.Students
            .AsNoTracking()
            .Where(s => !s.IsDeleted && (
                s.SiblingGroupKey == normalizedCnic
                || s.Guardians.Any(g => !g.IsDeleted && g.CNIC != null
                    && (g.CNIC == cnic.Trim()
                        || g.CNIC.Replace("-", "") == normalizedCnic))))
            .Select(s => new { s.StudentId, s.AdmissionNo, Name = s.FirstName + " " + s.LastName, s.SiblingOrder })
            .OrderBy(s => s.SiblingOrder ?? 999)
            .ThenBy(s => s.StudentId)
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
