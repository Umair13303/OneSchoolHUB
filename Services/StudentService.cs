using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Common;
using SchoolManagement.API.DTOs.Students;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IStudentService
{
    Task<PagedResultDto<StudentListDto>> GetAllAsync(int? classId, int? academicYearId, string? search, int page, int pageSize);
    Task<StudentDetailDto?>    GetByIdAsync(int id);
    Task<StudentDetailDto>     CreateAdmissionAsync(CreateStudentAdmissionDto dto, int createdBy);
    Task<bool>                 UpdateAsync(int id, UpdateStudentDto dto, int updatedBy);
    Task<List<StudentDocumentDto>> GetDocumentsAsync(int studentId);
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

    public StudentService(AppDbContext db, IConfiguration config, ISettingsService settings)
    {
        _db       = db;
        _config   = config;
        _settings = settings;
    }

    // ── List ──────────────────────────────────────────────────────────────
    public async Task<PagedResultDto<StudentListDto>> GetAllAsync(int? classId, int? academicYearId, string? search, int page, int pageSize)
    {
        // pageSize <= 0 means "no paging" — used by callers that need the full
        // roster (dashboard stats, attendance sheet, setup checks), not a page.
        page     = page < 1 ? 1 : page;
        var noPaging = pageSize <= 0;
        if (!noPaging) pageSize = Math.Min(pageSize, 200);

        // Build query, then project. The Student entity already has a global
        // HasQueryFilter(!IsDeleted) from AppDbContext.
        var students = _db.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            students = students.Where(st =>
                st.FirstName.Contains(s) ||
                st.LastName.Contains(s)  ||
                st.AdmissionNo.Contains(s));
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
                PhotoFileId = st.PhotoFileId,
                IsActive    = st.IsActive,

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
        await TagDocumentAsync(dto.BirthCertificateFileId,    "birth-certificate", student.StudentId);
        await TagDocumentAsync(dto.PreviousSchoolCertFileId,  "prev-school-cert",  student.StudentId);
        if (dto.PhotoFileId.HasValue)
            await TagDocumentAsync(dto.PhotoFileId, "student-photo", student.StudentId);

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
        var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == id);
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
            await TagDocumentAsync(dto.PhotoFileId, "student-photo", id);
        }

        await _db.SaveChangesAsync();
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
    private async Task TagDocumentAsync(int? fileId, string entityType, int studentId)
    {
        if (!fileId.HasValue) return;
        var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == fileId.Value && !f.IsDeleted);
        if (file is null) return;

        file.EntityId   = studentId;
        file.EntityType = entityType;
        // Save is intentionally deferred to the caller's SaveChangesAsync / transaction.
        await _db.SaveChangesAsync();
    }
}
