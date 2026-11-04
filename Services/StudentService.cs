using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Common;
using SchoolManagement.API.DTOs.Students;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IStudentService
{
    Task<PagedResultDto<StudentListDto>> GetAllAsync(int? classId, int? academicYearId, string? search, bool? isActive, int page, int pageSize, bool includeDeleted = false);
    Task<StudentDetailDto?>    GetByIdAsync(int id);
    Task<StudentDetailDto>     CreateAdmissionAsync(CreateStudentAdmissionDto dto, int createdBy);
    Task<bool>                 UpdateAsync(int id, UpdateStudentDto dto, int updatedBy);
    Task<List<StudentDocumentDto>> GetDocumentsAsync(int studentId);
    Task<StudentDocumentDto>   TagDocumentAsync(int studentId, TagStudentDocumentDto dto);
    Task<StudentDetailDto>     TransferAsync(int studentId, TransferStudentDto dto, int actedBy);
    Task<(int promoted, int skipped)> PromoteBulkAsync(BulkPromoteDto dto, int actedBy);
    Task<GuardianPortalResultDto> LinkGuardianPortalAsync(int studentId, int guardianId, GuardianPortalDto dto, int actedBy);
    Task<bool>                 UnlinkGuardianPortalAsync(int studentId, int guardianId);
    Task<byte[]>               ExportAsync(int? classId, int? academicYearId, string? search, bool? isActive);
    Task<bool>                 SoftDeleteAsync(int id, int actedBy);
    Task<bool>                 RestoreAsync(int id, int actedBy);
    Task<StudentIdCardPdfModel?> GetIdCardDataAsync(int id);
    /// <summary>Preview of the admission number the next admission will get (settings-driven).</summary>
    Task<string>               GetNextAdmissionNumberAsync();
}

/// <summary>
/// Implements Module 3 (Student Admission). The admission flow creates a
/// Student row, its Guardians, and an initial StudentClassEnrollment in a
/// single DB transaction so a partial admission can never be persisted.
/// </summary>
public class StudentService : IStudentService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ISettingsService _settings;
    private readonly ITenantContext _tenant;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IWebHostEnvironment _env;
    private readonly IGuardianAutoLinkService _autoLink;

    public StudentService(
        AppDbContext db,
        IConfiguration config,
        ISettingsService settings,
        ITenantContext tenant,
        IHttpClientFactory httpFactory,
        IWebHostEnvironment env,
        IGuardianAutoLinkService autoLink)
    {
        _db          = db;
        _config      = config;
        _settings    = settings;
        _tenant      = tenant;
        _httpFactory = httpFactory;
        _env         = env;
        _autoLink    = autoLink;
    }

    // ── List ──────────────────────────────────────────────────────────────
    public async Task<PagedResultDto<StudentListDto>> GetAllAsync(int? classId, int? academicYearId, string? search, bool? isActive, int page, int pageSize, bool includeDeleted = false)
    {
        // pageSize <= 0 means "no paging" — used by callers that need the full
        // roster (dashboard stats, attendance sheet, setup checks), not a page.
        page     = page < 1 ? 1 : page;
        var noPaging = pageSize <= 0;
        if (!noPaging) pageSize = Math.Min(pageSize, 200);

        // Build query, then project. The Student entity already has a global
        // HasQueryFilter(!IsDeleted) from AppDbContext — ignore when listing deleted.
        IQueryable<Student> students;
        if (includeDeleted)
        {
            students = _db.Students.IgnoreQueryFilters().AsNoTracking()
                .Where(st => st.IsDeleted);
            if (!_tenant.IsSuperAdmin)
                students = students.Where(st => st.InstituteId == _tenant.InstituteId);
        }
        else
        {
            students = _db.Students.AsNoTracking().AsQueryable();
        }

        if (!includeDeleted && isActive.HasValue)
            students = students.Where(st => st.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            students = students.Where(st =>
                st.FirstName.Contains(s) ||
                st.LastName.Contains(s)  ||
                st.AdmissionNo.Contains(s) ||
                (st.Phone != null && st.Phone.Contains(s)) ||
                st.Guardians.Any(g => !g.IsDeleted && (
                    (g.Phone != null && g.Phone.Contains(s)) ||
                    g.FullName.Contains(s))));
        }

        // Filter by enrollment in a specific class/year via subquery.
        if (classId.HasValue || academicYearId.HasValue)
        {
            students = students.Where(st => st.Enrollments.Any(e =>
                e.Status == "Active" &&
                !e.IsDeleted &&
                (!classId.HasValue        || e.ClassId == classId.Value) &&
                (!academicYearId.HasValue || e.AcademicYearId == academicYearId.Value)));
        }

        var totalCount = await students.CountAsync();

        IQueryable<Student> pagedQuery = students.OrderBy(st => st.FirstName).ThenBy(st => st.LastName);
        if (!noPaging)
            pagedQuery = pagedQuery.Skip((page - 1) * pageSize).Take(pageSize);

        var items = await pagedQuery
            .Select(st => new StudentListDto
            {
                StudentId   = st.StudentId,
                AdmissionNo = st.AdmissionNo,
                FullName    = (st.FirstName + " " + st.LastName).Trim(),
                DateOfBirth = st.DateOfBirth,
                Gender      = st.Gender,
                Phone       = st.Guardians
                    .Where(g => !g.IsDeleted)
                    .OrderBy(g => g.GuardianId)
                    .Select(g => g.Phone)
                    .FirstOrDefault(),
                PhotoFileId = st.PhotoFileId,
                IsActive    = st.IsActive,
                IsDeleted   = st.IsDeleted,

                // Show the most-recent active enrollment (single tuple) per student.
                ClassId           = st.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => (int?)e.ClassId).FirstOrDefault(),
                ClassName         = st.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => e.Class.ClassName).FirstOrDefault(),
                Section           = st.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => e.Class.Section).FirstOrDefault(),
                AcademicYearId    = st.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => (int?)e.AcademicYearId).FirstOrDefault(),
                AcademicYearLabel = st.Enrollments
                    .Where(e => e.Status == "Active" && !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate)
                    .Select(e => e.AcademicYear.YearLabel).FirstOrDefault()
            })
            .ToListAsync();

        return new PagedResultDto<StudentListDto>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = noPaging ? totalCount : pageSize
        };
    }

    // ── Detail ────────────────────────────────────────────────────────────
    public async Task<StudentDetailDto?> GetByIdAsync(int id)
    {
        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.StudentId == id)
            .Include(s => s.Guardians)
            .Include(s => s.Enrollments).ThenInclude(e => e.Class)
            .Include(s => s.Enrollments).ThenInclude(e => e.AcademicYear)
            .FirstOrDefaultAsync();

        if (student is null) return null;

        var current = student.Enrollments
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.AcademicYearId)
            .ThenByDescending(e => e.EnrollmentDate)
            .FirstOrDefault(e => e.Status == "Active")
            ?? student.Enrollments
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.AcademicYearId)
            .ThenByDescending(e => e.EnrollmentDate)
            .FirstOrDefault();

        return new StudentDetailDto
        {
            StudentId     = student.StudentId,
            AdmissionNo   = student.AdmissionNo,
            FirstName     = student.FirstName,
            LastName      = student.LastName,
            DateOfBirth   = student.DateOfBirth,
            Gender        = student.Gender,
            BloodGroup    = student.BloodGroup,
            Religion      = student.Religion,
            Nationality   = student.Nationality,
            Address       = student.Address,
            Phone         = student.Phone,
            Email         = student.Email,
            PhotoFileId   = student.PhotoFileId,
            AdmissionDate = student.AdmissionDate,
            IsActive      = student.IsActive,
            LeavingDate   = student.LeavingDate,
            LeavingReason = student.LeavingReason,
            SiblingGroupKey = student.SiblingGroupKey,
            SiblingOrder    = student.SiblingOrder,

            CurrentClassId           = current?.ClassId,
            CurrentClassName         = current?.Class?.ClassName,
            CurrentSection           = current?.Class?.Section,
            CurrentAcademicYearId    = current?.AcademicYearId,
            CurrentAcademicYearLabel = current?.AcademicYear?.YearLabel,

            Guardians = student.Guardians
                .Where(g => !g.IsDeleted)
                .Select(g => new GuardianDto
                {
                    GuardianId = g.GuardianId,
                    FullName   = g.FullName,
                    Relation   = g.Relation,
                    Phone      = g.Phone,
                    Occupation = g.Occupation,
                    CNIC       = g.CNIC,
                    UserId     = g.UserId
                }).ToList()
        };
    }

    // ── New admission ─────────────────────────────────────────────────────
    public async Task<StudentDetailDto> CreateAdmissionAsync(CreateStudentAdmissionDto dto, int createdBy)
    {
        // Up-front validation that doesn't need to be inside the transaction.
        if (dto.Guardians is null || dto.Guardians.Count == 0)
            throw new ArgumentException("At least one guardian is required.");

        var yearExists = await _db.AcademicYears.AnyAsync(y => y.AcademicYearId == dto.AcademicYearId && !y.IsDeleted);
        if (!yearExists) throw new ArgumentException($"AcademicYearId {dto.AcademicYearId} does not exist.");

        var classRow = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == dto.ClassId);
        if (classRow is null) throw new ArgumentException($"ClassId {dto.ClassId} does not exist.");

        var admissionDate = dto.AdmissionDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        // If a custom admission number is supplied (existing/old student), validate uniqueness up-front.
        if (!string.IsNullOrWhiteSpace(dto.CustomAdmissionNo))
        {
            var exists = await _db.Students.AnyAsync(s => s.AdmissionNo == dto.CustomAdmissionNo.Trim() && !s.IsDeleted);
            if (exists) throw new InvalidOperationException($"Admission number '{dto.CustomAdmissionNo.Trim()}' is already in use.");
        }

        // Generate admission number with retry — the unique index on AdmissionNo
        // protects against the rare race where two admissions in the same year
        // grab the same sequence number.
        await using var tx = await _db.Database.BeginTransactionAsync();
        Student student;
        for (var attempt = 0; ; attempt++)
        {
            var admissionNo = !string.IsNullOrWhiteSpace(dto.CustomAdmissionNo)
                ? dto.CustomAdmissionNo.Trim()
                : await GenerateAdmissionNumberAsync(admissionDate.Year);
            student = new Student
            {
                AdmissionNo   = admissionNo,
                FirstName     = dto.FirstName.Trim(),
                LastName      = dto.LastName.Trim(),
                DateOfBirth   = dto.DateOfBirth,
                Gender        = dto.Gender,
                BloodGroup    = dto.BloodGroup,
                Religion      = dto.Religion,
                Nationality   = dto.Nationality,
                Address       = dto.Address,
                Phone         = dto.Phone,
                Email         = dto.Email,
                PhotoFileId   = dto.PhotoFileId,
                AdmissionDate = admissionDate,
                IsActive      = true,
                CreatedBy     = createdBy
            };
            _db.Students.Add(student);

            try
            {
                await _db.SaveChangesAsync();
                break; // success
            }
            catch (DbUpdateException) when (attempt < 5)
            {
                // Collided on the unique AdmissionNo index; reset and retry.
                _db.Entry(student).State = EntityState.Detached;
            }
        }

        // Guardians
        foreach (var g in dto.Guardians)
        {
            _db.StudentGuardians.Add(new StudentGuardian
            {
                StudentId  = student.StudentId,
                FullName   = g.FullName,
                Relation   = g.Relation,
                Phone      = g.Phone,
                Occupation = g.Occupation,
                CNIC       = g.CNIC,
                UserId     = g.UserId
            });
        }

        // Initial class enrollment
        _db.StudentClassEnrollments.Add(new StudentClassEnrollment
        {
            StudentId       = student.StudentId,
            ClassId         = dto.ClassId,
            AcademicYearId  = dto.AcademicYearId,
            EnrollmentDate  = admissionDate,
            Status          = "Active",
            CreatedBy       = createdBy
        });

        await _db.SaveChangesAsync();

        // Tag the supplementary documents in FileStore so GET /documents finds them.
        await TagFileInternalAsync(dto.BirthCertificateFileId,    "birth-certificate", student.StudentId);
        await TagFileInternalAsync(dto.PreviousSchoolCertFileId,  "prev-school-cert",  student.StudentId);
        if (dto.PhotoFileId.HasValue)
            await TagFileInternalAsync(dto.PhotoFileId, "student-photo", student.StudentId);

        await AssignSiblingGroupAsync(student.StudentId);

        var newGuardians = await _db.StudentGuardians
            .Where(g => g.StudentId == student.StudentId && !g.IsDeleted && g.UserId == null)
            .ToListAsync();
        foreach (var g in newGuardians)
            await _autoLink.TryLinkGuardianAsync(g);

        // Fee assignments at admission time
        if (dto.Fees != null && dto.Fees.Count > 0)
        {
            foreach (var feeDto in dto.Fees)
            {
                var sf = new StudentFee
                {
                    StudentId      = student.StudentId,
                    FeeStructureId = feeDto.FeeStructureId,
                    AcademicYearId = dto.AcademicYearId,
                    AmountDue      = feeDto.AmountDue,
                    Discount       = feeDto.Discount,
                    DueDate        = feeDto.DueDate,
                    Status         = feeDto.IsPaid ? "Paid" : "Unpaid",
                    AmountPaid     = feeDto.IsPaid ? feeDto.AmountPaid : 0,
                    Remarks        = feeDto.Remarks,
                    CreatedBy      = createdBy
                };
                _db.StudentFees.Add(sf);
            }
            await _db.SaveChangesAsync();
        }

        await tx.CommitAsync();

        return (await GetByIdAsync(student.StudentId))!;
    }

    // ── Update ────────────────────────────────────────────────────────────
    public async Task<bool> UpdateAsync(int id, UpdateStudentDto dto, int updatedBy)
    {
        var student = await _db.Students
            .Include(s => s.Guardians)
            .FirstOrDefaultAsync(s => s.StudentId == id);
        if (student is null) return false;

        student.FirstName    = dto.FirstName.Trim();
        student.LastName     = dto.LastName.Trim();
        student.DateOfBirth  = dto.DateOfBirth;
        student.Gender       = dto.Gender;
        student.BloodGroup   = dto.BloodGroup;
        student.Religion     = dto.Religion;
        student.Nationality  = dto.Nationality;
        student.Address      = dto.Address;
        student.Phone        = dto.Phone;
        student.Email        = dto.Email;
        student.IsActive     = dto.IsActive;
        student.UpdatedBy    = updatedBy;
        student.UpdatedAt    = DateTime.UtcNow;

        if (dto.PhotoFileId.HasValue && dto.PhotoFileId != student.PhotoFileId)
        {
            student.PhotoFileId = dto.PhotoFileId;
            await TagFileInternalAsync(dto.PhotoFileId, "student-photo", id);
        }
        if (dto.BirthCertificateFileId.HasValue)
            await TagFileInternalAsync(dto.BirthCertificateFileId, "birth-certificate", id);
        if (dto.PreviousSchoolCertFileId.HasValue)
            await TagFileInternalAsync(dto.PreviousSchoolCertFileId, "prev-school-cert", id);

        var replaceGuardianList = dto.Guardians is { Count: > 0 };
        var guardianDtos = dto.Guardians;
        if (guardianDtos is null && dto.Guardian is not null)
            guardianDtos = new List<UpdateGuardianDto> { dto.Guardian };

        if (guardianDtos is { Count: > 0 })
        {
            var keptIds = new HashSet<int>();
            foreach (var gDto in guardianDtos)
            {
                StudentGuardian? guardian = null;
                if (gDto.GuardianId.HasValue)
                    guardian = student.Guardians.FirstOrDefault(g => g.GuardianId == gDto.GuardianId.Value && !g.IsDeleted);

                if (gDto.Remove)
                {
                    if (guardian is not null) guardian.IsDeleted = true;
                    continue;
                }

                if (guardian is null)
                {
                    _db.StudentGuardians.Add(new StudentGuardian
                    {
                        StudentId  = id,
                        FullName   = gDto.FullName.Trim(),
                        Relation   = gDto.Relation,
                        Phone      = gDto.Phone.Trim(),
                        CNIC       = gDto.CNIC,
                        Occupation = gDto.Occupation
                    });
                }
                else
                {
                    guardian.FullName   = gDto.FullName.Trim();
                    guardian.Relation   = gDto.Relation;
                    guardian.Phone      = gDto.Phone.Trim();
                    guardian.CNIC       = gDto.CNIC;
                    guardian.Occupation = gDto.Occupation;
                    keptIds.Add(guardian.GuardianId);
                }
            }

            // Full list replace: soft-delete guardians omitted from the payload.
            if (replaceGuardianList)
            {
                foreach (var existing in student.Guardians.Where(g => !g.IsDeleted && g.GuardianId > 0))
                {
                    if (!keptIds.Contains(existing.GuardianId))
                        existing.IsDeleted = true;
                }
            }
        }

        await _db.SaveChangesAsync();
        await AssignSiblingGroupAsync(id);

        // Auto-link any unlinked guardians to matching school users (teacher-as-parent).
        var toLink = await _db.StudentGuardians
            .Where(g => g.StudentId == id && !g.IsDeleted && g.UserId == null)
            .ToListAsync();
        foreach (var g in toLink)
            await _autoLink.TryLinkGuardianAsync(g);

        return true;
    }

    // ── Documents ─────────────────────────────────────────────────────────
    public async Task<List<StudentDocumentDto>> GetDocumentsAsync(int studentId)
    {
        var fileServerBaseUrl = (_config["FileServer:BaseUrl"] ?? string.Empty).TrimEnd('/');

        return await _db.FileStores
            .AsNoTracking()
            .Where(f => !f.IsDeleted
                     && f.EntityId == studentId
                     && (f.EntityType == "student-photo"
                      || f.EntityType == "birth-certificate"
                      || f.EntityType == "prev-school-cert"))
            .OrderByDescending(f => f.UploadedAt)
            .Select(f => new StudentDocumentDto
            {
                FileId       = f.FileId,
                EntityType   = f.EntityType!,
                OriginalName = f.OriginalName,
                FileType     = f.FileType,
                SizeBytes    = f.SizeBytes,
                UploadedAt   = f.UploadedAt,
                FileUrl      = fileServerBaseUrl + "/api/files/" + f.FileId
            })
            .ToListAsync();
    }

    public async Task<StudentDocumentDto> TagDocumentAsync(int studentId, TagStudentDocumentDto dto)
    {
        var exists = await _db.Students.AnyAsync(s => s.StudentId == studentId);
        if (!exists) throw new ArgumentException("Student not found.");

        var entityType = (dto.EntityType ?? string.Empty).Trim().ToLowerInvariant();
        if (entityType is not ("student-photo" or "birth-certificate" or "prev-school-cert"))
            throw new ArgumentException("EntityType must be student-photo, birth-certificate, or prev-school-cert.");

        var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == dto.FileId && !f.IsDeleted)
            ?? throw new ArgumentException("Uploaded file not found.");

        var previous = await _db.FileStores.FirstOrDefaultAsync(f =>
            !f.IsDeleted && f.EntityId == studentId && f.EntityType == entityType && f.FileId != dto.FileId);
        if (previous is not null) previous.IsDeleted = true;

        file.EntityId   = studentId;
        file.EntityType = entityType;

        if (entityType == "student-photo")
        {
            var student = await _db.Students.FirstAsync(s => s.StudentId == studentId);
            student.PhotoFileId = file.FileId;
        }

        await _db.SaveChangesAsync();

        var fileServerBaseUrl = (_config["FileServer:BaseUrl"] ?? string.Empty).TrimEnd('/');
        return new StudentDocumentDto
        {
            FileId       = file.FileId,
            EntityType   = entityType,
            OriginalName = file.OriginalName,
            FileType     = file.FileType,
            SizeBytes    = file.SizeBytes,
            UploadedAt   = file.UploadedAt,
            FileUrl      = fileServerBaseUrl + "/api/files/" + file.FileId
        };
    }

    // ── Transfer / Promote / Readmit ───────────────────────────────────────
    public async Task<StudentDetailDto> TransferAsync(int studentId, TransferStudentDto dto, int actedBy)
    {
        var mode = (dto.Mode ?? "Transfer").Trim();
        if (mode is not ("Transfer" or "Promote" or "Readmit"))
            throw new ArgumentException("Mode must be Transfer, Promote, or Readmit.");

        var student = await _db.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == studentId)
            ?? throw new ArgumentException("Student not found.");

        var yearExists = await _db.AcademicYears.AnyAsync(y => y.AcademicYearId == dto.AcademicYearId && !y.IsDeleted);
        if (!yearExists) throw new ArgumentException($"AcademicYearId {dto.AcademicYearId} does not exist.");

        var classRow = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == dto.ClassId && !c.IsDeleted);
        if (classRow is null) throw new ArgumentException($"ClassId {dto.ClassId} does not exist.");

        if (mode == "Readmit")
        {
            if (student.IsActive)
                throw new InvalidOperationException("Student is already active. Use Transfer or Promote instead.");
            student.IsActive = true;
            student.LeavingDate = null;
            student.LeavingReason = null;
        }
        else if (!student.IsActive)
        {
            throw new InvalidOperationException("Inactive students must use Readmit.");
        }

        var effective = dto.EffectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var activeEnrollments = student.Enrollments.Where(e => !e.IsDeleted && e.Status == "Active").ToList();

        foreach (var enr in activeEnrollments)
        {
            if (enr.ClassId == dto.ClassId && enr.AcademicYearId == dto.AcademicYearId)
                throw new InvalidOperationException("Student is already enrolled in that class for the selected year.");
            // Promote marks prior Active as Promoted; Transfer/Readmit close as Withdrawn.
            enr.Status = mode == "Promote" ? "Promoted" : "Withdrawn";
        }

        _db.StudentClassEnrollments.Add(new StudentClassEnrollment
        {
            StudentId      = studentId,
            ClassId        = dto.ClassId,
            AcademicYearId = dto.AcademicYearId,
            EnrollmentDate = effective,
            Status         = "Active",
            CreatedBy      = actedBy,
            InstituteId    = student.InstituteId,
            CampusId       = student.CampusId
        });

        student.UpdatedBy = actedBy;
        student.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(studentId))!;
    }

    public async Task<(int promoted, int skipped)> PromoteBulkAsync(BulkPromoteDto dto, int actedBy)
    {
        if (dto.SourceClassId == dto.TargetClassId && dto.SourceAcademicYearId == dto.TargetAcademicYearId)
            throw new ArgumentException("Source and target class/year must differ.");

        var yearOk = await _db.AcademicYears.AnyAsync(y => y.AcademicYearId == dto.TargetAcademicYearId && !y.IsDeleted);
        if (!yearOk) throw new ArgumentException("Target academic year does not exist.");
        var classOk = await _db.Classes.AnyAsync(c => c.ClassId == dto.TargetClassId && !c.IsDeleted);
        if (!classOk) throw new ArgumentException("Target class does not exist.");

        var q = _db.Students
            .Include(s => s.Enrollments)
            .Where(s => s.IsActive && s.Enrollments.Any(e =>
                !e.IsDeleted && e.Status == "Active"
                && e.ClassId == dto.SourceClassId
                && e.AcademicYearId == dto.SourceAcademicYearId));

        if (dto.StudentIds is { Count: > 0 })
            q = q.Where(s => dto.StudentIds.Contains(s.StudentId));

        var students = await q.ToListAsync();
        var effective = dto.EffectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var promoted = 0;
        var skipped = 0;

        foreach (var student in students)
        {
            var already = student.Enrollments.Any(e =>
                !e.IsDeleted && e.Status == "Active"
                && e.ClassId == dto.TargetClassId
                && e.AcademicYearId == dto.TargetAcademicYearId);
            if (already) { skipped++; continue; }

            foreach (var enr in student.Enrollments.Where(e => !e.IsDeleted && e.Status == "Active"
                     && e.ClassId == dto.SourceClassId && e.AcademicYearId == dto.SourceAcademicYearId))
                enr.Status = "Promoted";

            _db.StudentClassEnrollments.Add(new StudentClassEnrollment
            {
                StudentId      = student.StudentId,
                ClassId        = dto.TargetClassId,
                AcademicYearId = dto.TargetAcademicYearId,
                EnrollmentDate = effective,
                Status         = "Active",
                CreatedBy      = actedBy,
                InstituteId    = student.InstituteId,
                CampusId       = student.CampusId
            });
            student.UpdatedBy = actedBy;
            student.UpdatedAt = DateTime.UtcNow;
            promoted++;
        }

        await _db.SaveChangesAsync();
        return (promoted, skipped);
    }

    // ── Parent portal ─────────────────────────────────────────────────────
    public async Task<GuardianPortalResultDto> LinkGuardianPortalAsync(int studentId, int guardianId, GuardianPortalDto dto, int actedBy)
    {
        var guardian = await _db.StudentGuardians
            .FirstOrDefaultAsync(g => g.GuardianId == guardianId && g.StudentId == studentId && !g.IsDeleted)
            ?? throw new ArgumentException("Guardian not found for this student.");

        const int parentRoleId = 5;

        if (dto.UserId.HasValue)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId.Value && !u.IsDeleted)
                ?? throw new ArgumentException("User not found.");
            // Allow parent / teacher / staff / principal — same person can teach and be a parent.
            if (user.RoleId is not (3 or 4 or 5 or 6))
                throw new ArgumentException("Linked user must be a school parent, teacher, staff, or principal account.");

            guardian.UserId = user.UserId;
            await _db.SaveChangesAsync();
            return new GuardianPortalResultDto
            {
                GuardianId = guardian.GuardianId,
                UserId     = user.UserId,
                Email      = user.Email,
                FullName   = user.FullName,
                Created    = false
            };
        }

        if (guardian.UserId.HasValue)
            throw new InvalidOperationException("Guardian already has a portal account. Unlink first or use Link with UserId.");

        var email = string.IsNullOrWhiteSpace(dto.Email)
            ? $"parent{guardianId}.s{studentId}@oneschool.local"
            : dto.Email.Trim();

        var existingByEmail = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        if (existingByEmail is not null)
        {
            // Teacher/staff already has this email — auto-link instead of creating a duplicate.
            if (existingByEmail.RoleId is 3 or 4 or 5 or 6)
            {
                guardian.UserId = existingByEmail.UserId;
                await _db.SaveChangesAsync();
                return new GuardianPortalResultDto
                {
                    GuardianId = guardian.GuardianId,
                    UserId     = existingByEmail.UserId,
                    Email      = existingByEmail.Email,
                    FullName   = existingByEmail.FullName,
                    Created    = false
                };
            }
            throw new InvalidOperationException($"A user with email '{email}' already exists. Link that user instead.");
        }

        var tempPassword = string.IsNullOrWhiteSpace(dto.Password)
            ? GenerateTempPassword()
            : dto.Password;

        var student = await _db.Students.AsNoTracking().FirstAsync(s => s.StudentId == studentId);
        var newUser = new User
        {
            FullName     = guardian.FullName,
            Email        = email,
            Password     = tempPassword,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            RoleId       = parentRoleId,
            IsActive     = true,
            CreatedBy    = actedBy,
            Phone        = guardian.Phone,
            CNIC         = guardian.CNIC,
            InstituteId  = student.InstituteId,
            CampusId     = student.CampusId
        };
        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        guardian.UserId = newUser.UserId;
        await _db.SaveChangesAsync();

        var schoolName = await _db.Institutes.AsNoTracking()
            .Where(i => i.InstituteId == student.InstituteId)
            .Select(i => i.Name)
            .FirstOrDefaultAsync() ?? "OneSkool";

        var invite = BuildInviteMessage(schoolName, newUser.FullName, newUser.Email, tempPassword);

        return new GuardianPortalResultDto
        {
            GuardianId         = guardian.GuardianId,
            UserId             = newUser.UserId,
            Email              = newUser.Email,
            FullName           = newUser.FullName,
            TemporaryPassword  = tempPassword,
            Created            = true,
            InviteMessage      = invite
        };
    }

    public async Task<bool> UnlinkGuardianPortalAsync(int studentId, int guardianId)
    {
        var guardian = await _db.StudentGuardians
            .FirstOrDefaultAsync(g => g.GuardianId == guardianId && g.StudentId == studentId && !g.IsDeleted);
        if (guardian is null) return false;
        if (!guardian.UserId.HasValue) return true;
        guardian.UserId = null;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, int actedBy)
    {
        var student = await _db.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id);
        if (student is null) return false;

        student.IsDeleted = true;
        student.IsActive = false;
        student.UpdatedBy = actedBy;
        student.UpdatedAt = DateTime.UtcNow;
        foreach (var enr in student.Enrollments.Where(e => !e.IsDeleted && e.Status == "Active"))
            enr.Status = "Withdrawn";
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(int id, int actedBy)
    {
        var student = await _db.Students.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.StudentId == id && s.IsDeleted);
        if (student is null) return false;

        student.IsDeleted = false;
        student.UpdatedBy = actedBy;
        student.UpdatedAt = DateTime.UtcNow;
        // Stay inactive until Readmit puts them back in a class.
        student.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<StudentIdCardPdfModel?> GetIdCardDataAsync(int id)
    {
        var s = await GetByIdAsync(id);
        if (s is null) return null;

        var instituteId = await _db.Students.AsNoTracking()
            .Where(x => x.StudentId == id)
            .Select(x => x.InstituteId)
            .FirstOrDefaultAsync();
        var institute = await _db.Institutes.AsNoTracking()
            .FirstOrDefaultAsync(i => i.InstituteId == instituteId);

        var g = s.Guardians.FirstOrDefault();
        var classLabel = string.Join(" ", new[] { s.CurrentClassName, s.CurrentSection }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

        byte[]? photoBytes = null;
        if (s.PhotoFileId is int photoId)
            photoBytes = await TryLoadFileBytesAsync(photoId);

        byte[]? logoBytes = await TryLoadLogoBytesAsync(institute?.LogoUrl);

        return new StudentIdCardPdfModel
        {
            SchoolName    = institute?.Name ?? "School",
            SchoolPhone   = institute?.Phone,
            SchoolAddress = institute?.Address,
            SchoolEmail   = institute?.Email,
            LogoUrl       = institute?.LogoUrl,
            LogoBytes     = logoBytes,
            PhotoBytes    = photoBytes,
            FullName      = $"{s.FirstName} {s.LastName}".Trim(),
            AdmissionNo   = s.AdmissionNo,
            ClassLabel    = string.IsNullOrWhiteSpace(classLabel) ? "—" : classLabel,
            Gender        = s.Gender ?? "—",
            BloodGroup    = s.BloodGroup ?? "—",
            Phone         = s.Phone ?? "—",
            Address       = s.Address ?? "—",
            DateOfBirth   = s.DateOfBirth?.ToString("dd MMM yyyy") ?? "—",
            GuardianName  = g?.FullName ?? "—",
            GuardianPhone = g?.Phone ?? "—"
        };
    }

    private async Task<byte[]?> TryLoadFileBytesAsync(int fileId)
    {
        try
        {
            // Prefer local FilePath when present (dev / co-located store).
            var local = await _db.FileStores.AsNoTracking()
                .Where(f => f.FileId == fileId && !f.IsDeleted)
                .Select(f => f.FilePath)
                .FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(local) && File.Exists(local))
                return await File.ReadAllBytesAsync(local);

            var baseUrl = (_config["FileServer:BaseUrl"] ?? string.Empty).TrimEnd('/');
            var apiKey  = _config["FileServer:ApiKey"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
                return null;

            var client = _httpFactory.CreateClient("FileServer");
            client.DefaultRequestHeaders.Remove("X-Api-Key");
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            using var res = await client.GetAsync($"{baseUrl}/api/files/{fileId}");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            return null;
        }
    }

    private async Task<byte[]?> TryLoadLogoBytesAsync(string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl)) return null;
        try
        {
            if (logoUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
             || logoUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var client = _httpFactory.CreateClient();
                using var res = await client.GetAsync(logoUrl);
                if (!res.IsSuccessStatusCode) return null;
                return await res.Content.ReadAsByteArrayAsync();
            }

            var relative = logoUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var full = Path.Combine(webRoot, relative);
            if (File.Exists(full))
                return await File.ReadAllBytesAsync(full);
        }
        catch { /* ignore missing logo */ }
        return null;
    }

    // ── Export ────────────────────────────────────────────────────────────
    public async Task<byte[]> ExportAsync(int? classId, int? academicYearId, string? search, bool? isActive)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var page = await GetAllAsync(classId, academicYearId, search, isActive, 1, 0);
        var ids = page.Items.Select(i => i.StudentId).ToList();

        var details = await _db.Students.AsNoTracking()
            .Where(s => ids.Contains(s.StudentId))
            .Include(s => s.Guardians)
            .Include(s => s.Enrollments).ThenInclude(e => e.Class)
            .Include(s => s.Enrollments).ThenInclude(e => e.AcademicYear)
            .ToListAsync();

        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Students");
        string[] headers =
        [
            "AdmissionNo", "FirstName", "LastName", "DateOfBirth", "Gender", "BloodGroup",
            "Religion", "Nationality", "Address", "Phone", "Email", "AdmissionDate",
            "IsActive", "Class", "Section", "AcademicYear",
            "GuardianName", "GuardianPhone", "GuardianRelation", "GuardianCNIC", "GuardianOccupation"
        ];
        for (var c = 0; c < headers.Length; c++)
        {
            ws.Cells[1, c + 1].Value = headers[c];
            ws.Cells[1, c + 1].Style.Font.Bold = true;
        }

        var row = 2;
        foreach (var st in details.OrderBy(s => s.FirstName).ThenBy(s => s.LastName))
        {
            var enr = st.Enrollments.Where(e => !e.IsDeleted && e.Status == "Active")
                .OrderByDescending(e => e.EnrollmentDate).FirstOrDefault()
                ?? st.Enrollments.Where(e => !e.IsDeleted)
                    .OrderByDescending(e => e.EnrollmentDate).FirstOrDefault();
            var g = st.Guardians.Where(x => !x.IsDeleted).OrderBy(x => x.GuardianId).FirstOrDefault();

            ws.Cells[row, 1].Value  = st.AdmissionNo;
            ws.Cells[row, 2].Value  = st.FirstName;
            ws.Cells[row, 3].Value  = st.LastName;
            ws.Cells[row, 4].Value  = st.DateOfBirth?.ToString("dd/MM/yyyy");
            ws.Cells[row, 5].Value  = st.Gender;
            ws.Cells[row, 6].Value  = st.BloodGroup;
            ws.Cells[row, 7].Value  = st.Religion;
            ws.Cells[row, 8].Value  = st.Nationality;
            ws.Cells[row, 9].Value  = st.Address;
            ws.Cells[row, 10].Value = st.Phone;
            ws.Cells[row, 11].Value = st.Email;
            ws.Cells[row, 12].Value = st.AdmissionDate.ToString("dd/MM/yyyy");
            ws.Cells[row, 13].Value = st.IsActive ? "Active" : "Inactive";
            ws.Cells[row, 14].Value = enr?.Class?.ClassName;
            ws.Cells[row, 15].Value = enr?.Class?.Section;
            ws.Cells[row, 16].Value = enr?.AcademicYear?.YearLabel;
            ws.Cells[row, 17].Value = g?.FullName;
            ws.Cells[row, 18].Value = g?.Phone;
            ws.Cells[row, 19].Value = g?.Relation;
            ws.Cells[row, 20].Value = g?.CNIC;
            ws.Cells[row, 21].Value = g?.Occupation;
            row++;
        }

        ws.Cells[1, 1, Math.Max(1, row - 1), headers.Length].AutoFitColumns();
        return pkg.GetAsByteArray();
    }

    // ──────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────

    public async Task<string> GetNextAdmissionNumberAsync()
        => await GenerateAdmissionNumberAsync(DateTime.UtcNow.Year);

    /// <summary>
    /// Generates the next admission number using the institute/campus settings:
    /// <c>{Prefix}-{YYYY}-{Seq}</c>, or <c>{Prefix}-{Seq}</c> when the year part
    /// is disabled. Prefix, year toggle and sequence padding come from
    /// SchoolSettings (a campus settings row overrides the institute row).
    /// Uses a max-of-existing query — fine for a school's admission volume, and
    /// the unique index plus retry loop in <see cref="CreateAdmissionAsync"/>
    /// guards the rare race. The Students tenant query filter scopes the
    /// sequence per institute.
    /// </summary>
    private async Task<string> GenerateAdmissionNumberAsync(int year)
    {
        var cfg = await _settings.GetAsync();
        var prefix = cfg.AdmissionNoIncludeYear
            ? $"{cfg.AdmissionNoPrefix}-{year}-"
            : $"{cfg.AdmissionNoPrefix}-";

        // Numeric max in memory rather than string OrderByDescending — string
        // ordering breaks once the sequence outgrows its padding (e.g. "10000"
        // sorts below "9999").
        var existing = await _db.Students
            .Where(s => s.AdmissionNo.StartsWith(prefix))
            .Select(s => s.AdmissionNo)
            .ToListAsync();

        var seq = existing
            .Select(n => int.TryParse(n.Substring(prefix.Length), out var v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();

        var padding = Math.Clamp(cfg.AdmissionNoPadding, 2, 8);
        return $"{prefix}{(seq + 1).ToString($"D{padding}")}";
    }

    /// <summary>
    /// Tag a previously-uploaded FileStore row with the student's id and
    /// entity-type so it shows up in <c>GET /api/students/{id}/documents</c>.
    /// </summary>
    private async Task TagFileInternalAsync(int? fileId, string entityType, int studentId)
    {
        if (!fileId.HasValue) return;
        var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == fileId.Value && !f.IsDeleted);
        if (file is null) return;

        file.EntityId   = studentId;
        file.EntityType = entityType;
        await _db.SaveChangesAsync();
    }

    private static string GenerateTempPassword()
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
        var bytes = RandomNumberGenerator.GetBytes(10);
        var chars = new char[10];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = alphabet[bytes[i] % alphabet.Length];
        return new string(chars);
    }

    private static string BuildInviteMessage(string schoolName, string fullName, string email, string password) =>
        $"Assalam o Alaikum {fullName},\n\n" +
        $"{schoolName} parent portal account ready hai.\n\n" +
        $"Login email: {email}\n" +
        $"Temporary password: {password}\n\n" +
        "Please sign in and change your password after first login.\n" +
        "— OneSkool";

    private static string? NormalizeCnic(string? cnic)
    {
        if (string.IsNullOrWhiteSpace(cnic)) return null;
        var digits = new string(cnic.Where(char.IsDigit).ToArray());
        return digits.Length >= 5 ? digits : cnic.Trim();
    }

    /// <summary>
    /// Sets SiblingGroupKey from the earliest guardian CNIC and assigns SiblingOrder
    /// (1..n by admission date / student id within the institute).
    /// </summary>
    private async Task AssignSiblingGroupAsync(int studentId)
    {
        var student = await _db.Students
            .Include(s => s.Guardians)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return;

        var cnic = student.Guardians
            .Where(g => !g.IsDeleted && !string.IsNullOrWhiteSpace(g.CNIC))
            .OrderBy(g => g.GuardianId)
            .Select(g => g.CNIC)
            .FirstOrDefault();

        var key = NormalizeCnic(cnic);
        if (key is null)
        {
            student.SiblingGroupKey = null;
            student.SiblingOrder = null;
            await _db.SaveChangesAsync();
            return;
        }

        student.SiblingGroupKey = key;
        await _db.SaveChangesAsync();

        // Re-order everyone in the same group for this institute (including soft-visible only).
        var siblings = await _db.Students
            .Where(s => s.InstituteId == student.InstituteId
                     && s.SiblingGroupKey == key
                     && !s.IsDeleted)
            .OrderBy(s => s.AdmissionDate)
            .ThenBy(s => s.StudentId)
            .ToListAsync();

        for (var i = 0; i < siblings.Count; i++)
            siblings[i].SiblingOrder = i + 1;

        await _db.SaveChangesAsync();
    }
}
