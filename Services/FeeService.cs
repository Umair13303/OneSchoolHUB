using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Fees;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IFeeService
{
    // Fee Types
    Task<List<FeeTypeDto>> GetFeeTypesAsync();
    Task<FeeTypeDto> CreateFeeTypeAsync(CreateFeeTypeDto dto, int userId);
    Task<FeeTypeDto?> UpdateFeeTypeAsync(int id, UpdateFeeTypeDto dto, int userId);
    Task<bool> DeleteFeeTypeAsync(int id);

    // Fee Structures
    Task<List<FeeStructureDto>> GetFeeStructuresAsync(int? academicYearId, int? classId);
    Task<FeeStructureDto> CreateFeeStructureAsync(CreateFeeStructureDto dto, int userId);
    Task<FeeStructureDto?> UpdateFeeStructureAsync(int id, UpdateFeeStructureDto dto, int userId);
    Task<bool> DeleteFeeStructureAsync(int id);

    // Discount Policies
    Task<List<DiscountPolicyDto>> GetDiscountPoliciesAsync();
    Task<DiscountPolicyDto> CreateDiscountPolicyAsync(CreateDiscountPolicyDto dto, int userId);
    Task<DiscountPolicyDto?> UpdateDiscountPolicyAsync(int id, UpdateDiscountPolicyDto dto, int userId);
    Task<bool> DeleteDiscountPolicyAsync(int id);

    // Student Discounts
    Task<List<StudentDiscountDto>> GetStudentDiscountsAsync(int? studentId, int? academicYearId);
    Task<StudentDiscountDto> AssignStudentDiscountAsync(AssignStudentDiscountDto dto, int userId);
    Task<StudentDiscountDto?> UpdateStudentDiscountAsync(int id, UpdateStudentDiscountDto dto, int userId);
    Task<bool> DeleteStudentDiscountAsync(int id);

    // Student Fees
    Task<List<StudentFeeDto>> GetStudentFeesAsync(int? studentId, int? classId, int? academicYearId, string? status, int? instituteId = null, int? campusId = null);
    Task<StudentFeeDto?> GetStudentFeeByIdAsync(int id);
    Task<StudentFeeDto> CreateStudentFeeAsync(CreateStudentFeeDto dto, int userId);
    Task<FeeGenerationPreviewDto> PreviewStudentFeeAsync(int studentId, int feeStructureId, int academicYearId);
    Task<List<FeeGenerationPreviewDto>> PreviewBulkAssignAsync(int feeStructureId, int classId, int academicYearId);
    Task<BulkAssignResultDto> BulkAssignFeeAsync(BulkAssignFeeDto dto, int userId);
    Task<StudentFeeDto?> UpdateStudentFeeAsync(int id, UpdateStudentFeeDto dto, int userId);
    Task<bool> DeleteStudentFeeAsync(int id);

    // Payments
    Task<List<FeePaymentDto>> GetPaymentsAsync(int studentFeeId);
    Task<FeePaymentDto> RecordPaymentAsync(RecordPaymentDto dto, int userId);

    // Reports
    Task<List<FeeReportRowDto>> GetFeeReportAsync(int academicYearId, int? classId);
}

public class FeeService(AppDbContext db) : IFeeService
{
    // ── Fee Types ────────────────────────────────────────────────────────────

    public async Task<List<FeeTypeDto>> GetFeeTypesAsync()
        => await db.FeeTypes.Where(f => !f.IsDeleted).OrderBy(f => f.Name).Select(f => MapFeeType(f)).ToListAsync();

    public async Task<FeeTypeDto> CreateFeeTypeAsync(CreateFeeTypeDto dto, int userId)
    {
        var category = Enum.TryParse<FeeCategory>(dto.FeeCategory, out var cat) ? cat : FeeCategory.Recurring;
        var ft = new FeeType { Name = dto.Name.Trim(), Description = dto.Description?.Trim(), FeeCategory = category, IsActive = true, CreatedBy = userId };
        db.FeeTypes.Add(ft);
        await db.SaveChangesAsync();
        return MapFeeType(ft);
    }

    public async Task<FeeTypeDto?> UpdateFeeTypeAsync(int id, UpdateFeeTypeDto dto, int userId)
    {
        var ft = await db.FeeTypes.FirstOrDefaultAsync(f => f.FeeTypeId == id && !f.IsDeleted);
        if (ft is null) return null;
        ft.Name = dto.Name.Trim(); ft.Description = dto.Description?.Trim(); ft.IsActive = dto.IsActive;
        ft.FeeCategory = Enum.TryParse<FeeCategory>(dto.FeeCategory, out var cat) ? cat : FeeCategory.Recurring;
        ft.UpdatedBy = userId; ft.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapFeeType(ft);
    }

    public async Task<bool> DeleteFeeTypeAsync(int id)
    {
        var ft = await db.FeeTypes.FirstOrDefaultAsync(f => f.FeeTypeId == id && !f.IsDeleted);
        if (ft is null) return false;
        ft.IsDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }

    private static FeeTypeDto MapFeeType(FeeType f) =>
        new() { FeeTypeId = f.FeeTypeId, Name = f.Name, Description = f.Description, FeeCategory = f.FeeCategory.ToString(), IsActive = f.IsActive };

    // ── Fee Structures ───────────────────────────────────────────────────────

    public async Task<List<FeeStructureDto>> GetFeeStructuresAsync(int? academicYearId, int? classId)
    {
        var q = db.FeeStructures.Include(f => f.FeeType).Include(f => f.Class).Include(f => f.AcademicYear).Where(f => !f.IsDeleted);
        if (academicYearId.HasValue) q = q.Where(f => f.AcademicYearId == academicYearId);
        if (classId.HasValue)        q = q.Where(f => f.ClassId == classId);
        var list = await q.OrderBy(f => f.FeeType.Name).Select(f => MapStructure(f)).ToListAsync();

        // Structures have no Campus navigation — resolve campus names in one query
        // so multi-campus schools can tell identically named classes apart.
        var campusIds = await q.Where(f => f.CampusId != null).Select(f => f.CampusId!.Value).Distinct().ToListAsync();
        if (campusIds.Count > 0)
        {
            var campusNames = await db.Campuses.Where(c => campusIds.Contains(c.CampusId))
                .ToDictionaryAsync(c => c.CampusId, c => c.Name);
            var structureCampus = await q.Where(f => f.CampusId != null)
                .ToDictionaryAsync(f => f.FeeStructureId, f => f.CampusId!.Value);
            foreach (var s in list)
                if (structureCampus.TryGetValue(s.FeeStructureId, out var cid) && campusNames.TryGetValue(cid, out var name))
                    s.CampusName = name;
        }
        return list;
    }

    public async Task<FeeStructureDto> CreateFeeStructureAsync(CreateFeeStructureDto dto, int userId)
    {
        var fs = new FeeStructure
        {
            FeeTypeId = dto.FeeTypeId, ClassId = dto.ClassId, AcademicYearId = dto.AcademicYearId,
            Amount = dto.Amount, DueDay = dto.DueDay, IsActive = true, CreatedBy = userId
        };
        db.FeeStructures.Add(fs);
        await db.SaveChangesAsync();
        await db.Entry(fs).Reference(x => x.FeeType).LoadAsync();
        await db.Entry(fs).Reference(x => x.Class).LoadAsync();
        await db.Entry(fs).Reference(x => x.AcademicYear).LoadAsync();
        return MapStructure(fs);
    }

    public async Task<FeeStructureDto?> UpdateFeeStructureAsync(int id, UpdateFeeStructureDto dto, int userId)
    {
        var fs = await db.FeeStructures.Include(f => f.FeeType).Include(f => f.Class).Include(f => f.AcademicYear)
            .FirstOrDefaultAsync(f => f.FeeStructureId == id && !f.IsDeleted);
        if (fs is null) return null;
        fs.Amount = dto.Amount; fs.DueDay = dto.DueDay; fs.IsActive = dto.IsActive;
        fs.UpdatedBy = userId; fs.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapStructure(fs);
    }

    public async Task<bool> DeleteFeeStructureAsync(int id)
    {
        var fs = await db.FeeStructures.FirstOrDefaultAsync(f => f.FeeStructureId == id && !f.IsDeleted);
        if (fs is null) return false;
        fs.IsDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }

    private static FeeStructureDto MapStructure(FeeStructure f) => new()
    {
        FeeStructureId = f.FeeStructureId, FeeTypeId = f.FeeTypeId, FeeTypeName = f.FeeType?.Name ?? "",
        FeeCategory = f.FeeType?.FeeCategory.ToString() ?? "Recurring",
        ClassId = f.ClassId, ClassName = f.Class != null ? $"{f.Class.ClassName} {f.Class.Section}" : "",
        AcademicYearId = f.AcademicYearId, YearLabel = f.AcademicYear?.YearLabel ?? "",
        Amount = f.Amount, DueDay = f.DueDay, IsActive = f.IsActive
    };

    // ── Discount Policies ─────────────────────────────────────────────────────

    public async Task<List<DiscountPolicyDto>> GetDiscountPoliciesAsync()
        => await db.DiscountPolicies.Where(d => !d.IsDeleted)
            .OrderBy(d => d.DiscountType).ThenBy(d => d.Name)
            .Select(d => MapPolicy(d)).ToListAsync();

    public async Task<DiscountPolicyDto> CreateDiscountPolicyAsync(CreateDiscountPolicyDto dto, int userId)
    {
        var p = new DiscountPolicy
        {
            Name = dto.Name.Trim(), DiscountType = dto.DiscountType, Description = dto.Description.Trim(),
            ValueType = dto.ValueType, Value = dto.Value, MaxSiblingOrder = dto.MaxSiblingOrder,
            IsActive = true, CreatedBy = userId
        };
        db.DiscountPolicies.Add(p);
        await db.SaveChangesAsync();
        return MapPolicy(p);
    }

    public async Task<DiscountPolicyDto?> UpdateDiscountPolicyAsync(int id, UpdateDiscountPolicyDto dto, int userId)
    {
        var p = await db.DiscountPolicies.FirstOrDefaultAsync(d => d.DiscountPolicyId == id && !d.IsDeleted);
        if (p is null) return null;
        p.Name = dto.Name.Trim(); p.DiscountType = dto.DiscountType; p.Description = dto.Description.Trim();
        p.ValueType = dto.ValueType; p.Value = dto.Value; p.MaxSiblingOrder = dto.MaxSiblingOrder;
        p.IsActive = dto.IsActive; p.UpdatedBy = userId; p.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapPolicy(p);
    }

    public async Task<bool> DeleteDiscountPolicyAsync(int id)
    {
        var p = await db.DiscountPolicies.FirstOrDefaultAsync(d => d.DiscountPolicyId == id && !d.IsDeleted);
        if (p is null) return false;
        p.IsDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }

    private static DiscountPolicyDto MapPolicy(DiscountPolicy d) => new()
    {
        DiscountPolicyId = d.DiscountPolicyId, Name = d.Name, DiscountType = d.DiscountType,
        Description = d.Description, ValueType = d.ValueType, Value = d.Value,
        MaxSiblingOrder = d.MaxSiblingOrder, IsActive = d.IsActive
    };

    // ── Student Discounts ─────────────────────────────────────────────────────

    public async Task<List<StudentDiscountDto>> GetStudentDiscountsAsync(int? studentId, int? academicYearId)
    {
        var q = db.StudentDiscounts
            .Include(sd => sd.Student).Include(sd => sd.DiscountPolicy).Include(sd => sd.AcademicYear)
            .Where(sd => !sd.IsDeleted);
        if (studentId.HasValue)      q = q.Where(sd => sd.StudentId == studentId);
        if (academicYearId.HasValue) q = q.Where(sd => sd.AcademicYearId == academicYearId);
        return await q.OrderBy(sd => sd.Student.FirstName).Select(sd => MapStudentDiscount(sd)).ToListAsync();
    }

    public async Task<StudentDiscountDto> AssignStudentDiscountAsync(AssignStudentDiscountDto dto, int userId)
    {
        bool duplicate = await db.StudentDiscounts.AnyAsync(x =>
            x.StudentId == dto.StudentId && x.DiscountPolicyId == dto.DiscountPolicyId
            && x.AcademicYearId == dto.AcademicYearId && x.IsActive && !x.IsDeleted);
        if (duplicate)
            throw new InvalidOperationException("This discount policy is already assigned to the student for this academic year.");

        var sd = new StudentDiscount
        {
            StudentId = dto.StudentId, DiscountPolicyId = dto.DiscountPolicyId,
            AcademicYearId = dto.AcademicYearId, OverrideValue = dto.OverrideValue,
            Remarks = dto.Remarks, IsActive = true, CreatedBy = userId
        };
        db.StudentDiscounts.Add(sd);
        await db.SaveChangesAsync();
        await db.Entry(sd).Reference(x => x.Student).LoadAsync();
        await db.Entry(sd).Reference(x => x.DiscountPolicy).LoadAsync();
        await db.Entry(sd).Reference(x => x.AcademicYear).LoadAsync();
        return MapStudentDiscount(sd);
    }

    public async Task<StudentDiscountDto?> UpdateStudentDiscountAsync(int id, UpdateStudentDiscountDto dto, int userId)
    {
        var sd = await db.StudentDiscounts
            .Include(x => x.Student).Include(x => x.DiscountPolicy).Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.StudentDiscountId == id && !x.IsDeleted);
        if (sd is null) return null;
        sd.OverrideValue = dto.OverrideValue; sd.Remarks = dto.Remarks; sd.IsActive = dto.IsActive;
        sd.UpdatedBy = userId; sd.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapStudentDiscount(sd);
    }

    public async Task<bool> DeleteStudentDiscountAsync(int id)
    {
        var sd = await db.StudentDiscounts.FirstOrDefaultAsync(x => x.StudentDiscountId == id && !x.IsDeleted);
        if (sd is null) return false;
        sd.IsDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }

    private static StudentDiscountDto MapStudentDiscount(StudentDiscount sd)
    {
        var effective = sd.OverrideValue ?? sd.DiscountPolicy.Value;
        return new StudentDiscountDto
        {
            StudentDiscountId = sd.StudentDiscountId, StudentId = sd.StudentId,
            StudentName = sd.Student.FirstName + " " + sd.Student.LastName,
            AdmissionNo = sd.Student.AdmissionNo, DiscountPolicyId = sd.DiscountPolicyId,
            PolicyName = sd.DiscountPolicy.Name, DiscountType = sd.DiscountPolicy.DiscountType,
            ValueType = sd.DiscountPolicy.ValueType, PolicyValue = sd.DiscountPolicy.Value,
            OverrideValue = sd.OverrideValue, EffectiveValue = effective,
            AcademicYearId = sd.AcademicYearId, YearLabel = sd.AcademicYear?.YearLabel ?? "",
            Remarks = sd.Remarks, IsActive = sd.IsActive
        };
    }

    // ── Discount Engine ───────────────────────────────────────────────────────

    private async Task<(decimal totalDiscount, List<DiscountLineDto> lines)> ComputeDiscountAsync(
        int studentId, int academicYearId, decimal baseFee, FeeCategory feeCategory = FeeCategory.Recurring)
    {
        var discounts = await db.StudentDiscounts
            .Include(sd => sd.DiscountPolicy)
            .Where(sd => sd.StudentId == studentId && sd.AcademicYearId == academicYearId
                      && sd.IsActive && !sd.IsDeleted && !sd.DiscountPolicy.IsDeleted)
            .ToListAsync();

        var lines = new List<DiscountLineDto>();
        decimal total = 0;

        foreach (var sd in discounts)
        {
            var effectiveValue = sd.OverrideValue ?? sd.DiscountPolicy.Value;

            // A flat (FixedAmount) discount is a per-challan amount, but this
            // engine runs once per fee line — so it is deducted only from the
            // Recurring (tuition) line, otherwise it would multiply by the
            // number of fee types on the challan. Percentage is naturally
            // proportional, so it applies to every line.
            if (sd.DiscountPolicy.ValueType != "Percentage" && feeCategory != FeeCategory.Recurring)
                continue;

            decimal discountAmt = sd.DiscountPolicy.ValueType == "Percentage"
                ? Math.Round(baseFee * effectiveValue / 100, 2)
                : Math.Min(effectiveValue, baseFee);

            lines.Add(new DiscountLineDto
            {
                PolicyName = sd.DiscountPolicy.Name, DiscountType = sd.DiscountPolicy.DiscountType,
                ValueType = sd.DiscountPolicy.ValueType, Value = effectiveValue, DiscountAmount = discountAmt
            });
            total += discountAmt;
        }

        return (Math.Min(total, baseFee), lines);
    }

    // ── Student Fees ─────────────────────────────────────────────────────────

    public async Task<List<StudentFeeDto>> GetStudentFeesAsync(int? studentId, int? classId, int? academicYearId, string? status, int? instituteId = null, int? campusId = null)
    {
        var q = db.StudentFees
            .Include(sf => sf.Student).ThenInclude(s => s.Enrollments).ThenInclude(e => e.Class)
            .Include(sf => sf.FeeStructure).ThenInclude(fs => fs.FeeType)
            .Include(sf => sf.AcademicYear)
            .Where(sf => !sf.IsDeleted);

        if (instituteId.HasValue)          q = q.Where(sf => sf.InstituteId == instituteId);
        if (campusId.HasValue)             q = q.Where(sf => sf.CampusId == campusId);
        if (studentId.HasValue)            q = q.Where(sf => sf.StudentId == studentId);
        if (academicYearId.HasValue)       q = q.Where(sf => sf.AcademicYearId == academicYearId);
        if (!string.IsNullOrEmpty(status)) q = q.Where(sf => sf.Status == status);
        if (classId.HasValue)
            q = q.Where(sf => sf.Student.Enrollments.Any(e => e.ClassId == classId && e.Status == "Active" && !e.IsDeleted));

        return await q.OrderBy(sf => sf.DueDate).Select(sf => MapStudentFee(sf)).ToListAsync();
    }

    public async Task<StudentFeeDto?> GetStudentFeeByIdAsync(int id)
    {
        var sf = await db.StudentFees
            .Include(x => x.Student).ThenInclude(s => s.Enrollments).ThenInclude(e => e.Class)
            .Include(x => x.FeeStructure).ThenInclude(fs => fs.FeeType)
            .Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.StudentFeeId == id && !x.IsDeleted);
        return sf is null ? null : MapStudentFee(sf);
    }

    public async Task<StudentFeeDto> CreateStudentFeeAsync(CreateStudentFeeDto dto, int userId)
    {
        var sf = new StudentFee
        {
            StudentId = dto.StudentId, FeeStructureId = dto.FeeStructureId, AcademicYearId = dto.AcademicYearId,
            AmountDue = dto.AmountDue, Discount = dto.Discount, DueDate = dto.DueDate,
            Status = "Unpaid", Remarks = dto.Remarks, CreatedBy = userId
        };
        db.StudentFees.Add(sf);
        await db.SaveChangesAsync();
        return (await GetStudentFeeByIdAsync(sf.StudentFeeId))!;
    }

    public async Task<FeeGenerationPreviewDto> PreviewStudentFeeAsync(int studentId, int feeStructureId, int academicYearId)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId)
            ?? throw new InvalidOperationException("Student not found.");
        var structure = await db.FeeStructures.Include(fs => fs.FeeType)
                .FirstOrDefaultAsync(fs => fs.FeeStructureId == feeStructureId && !fs.IsDeleted)
            ?? throw new InvalidOperationException("Fee structure not found.");

        bool alreadyAssigned = await db.StudentFees.AnyAsync(sf =>
            sf.StudentId == studentId && sf.FeeStructureId == feeStructureId
            && sf.AcademicYearId == academicYearId && !sf.IsDeleted);

        var (totalDiscount, lines) = await ComputeDiscountAsync(studentId, academicYearId, structure.Amount, structure.FeeType.FeeCategory);

        return new FeeGenerationPreviewDto
        {
            StudentId = studentId,
            StudentName = student.FirstName + " " + student.LastName,
            AdmissionNo = student.AdmissionNo,
            BaseFee = structure.Amount,
            TotalDiscount = totalDiscount,
            NetPayable = structure.Amount - totalDiscount,
            DiscountLines = lines,
            AlreadyAssigned = alreadyAssigned
        };
    }

    public async Task<List<FeeGenerationPreviewDto>> PreviewBulkAssignAsync(int feeStructureId, int classId, int academicYearId)
    {
        var structure = await db.FeeStructures.Include(fs => fs.FeeType)
                .FirstOrDefaultAsync(fs => fs.FeeStructureId == feeStructureId && !fs.IsDeleted)
            ?? throw new InvalidOperationException("Fee structure not found.");

        var enrollments = await db.StudentClassEnrollments
            .Include(e => e.Student)
            .Where(e => e.ClassId == classId && e.AcademicYearId == academicYearId
                     && e.Status == "Active" && !e.IsDeleted && !e.Student.IsDeleted)
            .ToListAsync();

        var result = new List<FeeGenerationPreviewDto>();
        foreach (var enrollment in enrollments)
        {
            bool alreadyAssigned = await db.StudentFees.AnyAsync(sf =>
                sf.StudentId == enrollment.StudentId && sf.FeeStructureId == feeStructureId
                && sf.AcademicYearId == academicYearId && !sf.IsDeleted);

            var (totalDiscount, lines) = await ComputeDiscountAsync(enrollment.StudentId, academicYearId, structure.Amount, structure.FeeType.FeeCategory);

            result.Add(new FeeGenerationPreviewDto
            {
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student.FirstName + " " + enrollment.Student.LastName,
                AdmissionNo = enrollment.Student.AdmissionNo,
                BaseFee = structure.Amount,
                TotalDiscount = totalDiscount,
                NetPayable = structure.Amount - totalDiscount,
                DiscountLines = lines,
                AlreadyAssigned = alreadyAssigned
            });
        }
        return result.OrderBy(r => r.StudentName).ToList();
    }

    public async Task<BulkAssignResultDto> BulkAssignFeeAsync(BulkAssignFeeDto dto, int userId)
    {
        var structure = await db.FeeStructures.Include(fs => fs.FeeType)
                .FirstOrDefaultAsync(fs => fs.FeeStructureId == dto.FeeStructureId && !fs.IsDeleted)
            ?? throw new InvalidOperationException("Fee structure not found.");

        var enrollments = await db.StudentClassEnrollments
            .Include(e => e.Student)
            .Where(e => e.ClassId == dto.ClassId && e.AcademicYearId == dto.AcademicYearId
                     && e.Status == "Active" && !e.IsDeleted && !e.Student.IsDeleted)
            .ToListAsync();

        // OneTime / RefundableDeposit fees are charged once ever per student
        // (matched by fee type across years); Recurring / OnDemand fees dedupe
        // per billing month so each month gets its own challan.
        var onceEver = structure.FeeType.FeeCategory is FeeCategory.OneTime or FeeCategory.RefundableDeposit;

        int createdCount = 0, skippedCount = 0;
        foreach (var enrollment in enrollments)
        {
            bool exists = onceEver
                ? await db.StudentFees.AnyAsync(sf =>
                    sf.StudentId == enrollment.StudentId && !sf.IsDeleted
                    && sf.FeeStructure.FeeTypeId == structure.FeeTypeId)
                : dto.Month is int month
                    ? await db.StudentFees.AnyAsync(sf =>
                        sf.StudentId == enrollment.StudentId && sf.FeeStructureId == dto.FeeStructureId
                        && sf.AcademicYearId == dto.AcademicYearId && sf.FeeMonth == month && !sf.IsDeleted)
                    : await db.StudentFees.AnyAsync(sf =>
                        sf.StudentId == enrollment.StudentId && sf.FeeStructureId == dto.FeeStructureId
                        && sf.AcademicYearId == dto.AcademicYearId && sf.DueDate == dto.DueDate && !sf.IsDeleted);
            if (exists) { skippedCount++; continue; }

            var (totalDiscount, _) = await ComputeDiscountAsync(enrollment.StudentId, dto.AcademicYearId, structure.Amount, structure.FeeType.FeeCategory);

            db.StudentFees.Add(new StudentFee
            {
                StudentId = enrollment.StudentId, FeeStructureId = dto.FeeStructureId,
                AcademicYearId = dto.AcademicYearId, AmountDue = structure.Amount,
                Discount = totalDiscount, DueDate = dto.DueDate, FeeMonth = dto.Month,
                Status = "Unpaid", CreatedBy = userId
            });
            createdCount++;
        }
        await db.SaveChangesAsync();

        // Return the batch for this structure+year and billing period (month when
        // given, else due date) so the preview reflects what this challan contains.
        var batchQ = db.StudentFees
            .Include(sf => sf.Student).ThenInclude(s => s.Enrollments).ThenInclude(e => e.Class)
            .Include(sf => sf.FeeStructure).ThenInclude(fs => fs.FeeType)
            .Include(sf => sf.AcademicYear)
            .Where(sf => sf.FeeStructureId == dto.FeeStructureId && sf.AcademicYearId == dto.AcademicYearId && !sf.IsDeleted);
        batchQ = dto.Month is int m
            ? batchQ.Where(sf => sf.FeeMonth == m)
            : batchQ.Where(sf => sf.DueDate == dto.DueDate);

        var fees = await batchQ.Select(sf => MapStudentFee(sf)).ToListAsync();
        return new BulkAssignResultDto { Created = createdCount, Skipped = skippedCount, Fees = fees };
    }

    public async Task<StudentFeeDto?> UpdateStudentFeeAsync(int id, UpdateStudentFeeDto dto, int userId)
    {
        var sf = await db.StudentFees.FirstOrDefaultAsync(x => x.StudentFeeId == id && !x.IsDeleted);
        if (sf is null) return null;
        sf.Discount = dto.Discount; sf.DueDate = dto.DueDate; sf.Status = dto.Status;
        sf.Remarks = dto.Remarks; sf.UpdatedBy = userId; sf.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return await GetStudentFeeByIdAsync(id);
    }

    public async Task<bool> DeleteStudentFeeAsync(int id)
    {
        var sf = await db.StudentFees.FirstOrDefaultAsync(x => x.StudentFeeId == id && !x.IsDeleted);
        if (sf is null) return false;
        sf.IsDeleted = true;
        await db.SaveChangesAsync();
        return true;
    }

    private static StudentFeeDto MapStudentFee(StudentFee sf)
    {
        var ae = sf.Student.Enrollments?.FirstOrDefault(e => e.Status == "Active" && !e.IsDeleted);
        return new StudentFeeDto
        {
            StudentFeeId = sf.StudentFeeId, StudentId = sf.StudentId,
            StudentName = sf.Student.FirstName + " " + sf.Student.LastName,
            AdmissionNo = sf.Student.AdmissionNo,
            ClassName = ae?.Class != null ? $"{ae.Class.ClassName} {ae.Class.Section}" : "",
            FeeStructureId = sf.FeeStructureId, FeeTypeName = sf.FeeStructure?.FeeType?.Name ?? "",
            AcademicYearId = sf.AcademicYearId, YearLabel = sf.AcademicYear?.YearLabel ?? "",
            AmountDue = sf.AmountDue, AmountPaid = sf.AmountPaid, Discount = sf.Discount,
            DueDate = sf.DueDate, FeeMonth = sf.FeeMonth, Status = sf.Status, Remarks = sf.Remarks,
            InstituteId = sf.InstituteId
        };
    }

    // ── Payments ─────────────────────────────────────────────────────────────

    public async Task<List<FeePaymentDto>> GetPaymentsAsync(int studentFeeId)
        => await db.FeePayments
            .Include(p => p.CollectedByUser)
            .Where(p => p.StudentFeeId == studentFeeId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new FeePaymentDto
            {
                FeePaymentId = p.FeePaymentId, StudentFeeId = p.StudentFeeId,
                AmountPaid = p.AmountPaid, PaymentDate = p.PaymentDate, Method = p.Method,
                ReceiptNo = p.ReceiptNo, Remarks = p.Remarks, CollectedByName = p.CollectedByUser.FullName
            })
            .ToListAsync();

    public async Task<FeePaymentDto> RecordPaymentAsync(RecordPaymentDto dto, int userId)
    {
        var sf = await db.StudentFees.FirstOrDefaultAsync(x => x.StudentFeeId == dto.StudentFeeId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Student fee record not found.");

        var payment = new FeePayment
        {
            StudentFeeId = dto.StudentFeeId, AmountPaid = dto.AmountPaid, PaymentDate = dto.PaymentDate,
            Method = dto.Method, ReceiptNo = dto.ReceiptNo, Remarks = dto.Remarks,
            CollectedBy = userId, CreatedBy = userId
        };
        db.FeePayments.Add(payment);

        sf.AmountPaid += dto.AmountPaid;
        decimal balance = sf.AmountDue - sf.Discount - sf.AmountPaid;
        sf.Status = balance <= 0 ? "Paid" : sf.AmountPaid > 0 ? "Partial" : "Unpaid";
        sf.UpdatedBy = userId; sf.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        var user = await db.Users.FindAsync(userId);
        return new FeePaymentDto
        {
            FeePaymentId = payment.FeePaymentId, StudentFeeId = payment.StudentFeeId,
            AmountPaid = payment.AmountPaid, PaymentDate = payment.PaymentDate, Method = payment.Method,
            ReceiptNo = payment.ReceiptNo, Remarks = payment.Remarks, CollectedByName = user?.FullName ?? ""
        };
    }

    // ── Reports ──────────────────────────────────────────────────────────────

    public async Task<List<FeeReportRowDto>> GetFeeReportAsync(int academicYearId, int? classId)
    {
        var q = db.StudentFees
            .Include(sf => sf.Student).ThenInclude(s => s.Enrollments).ThenInclude(e => e.Class)
            .Where(sf => sf.AcademicYearId == academicYearId && !sf.IsDeleted && !sf.Student.IsDeleted);

        if (classId.HasValue)
            q = q.Where(sf => sf.Student.Enrollments.Any(e => e.ClassId == classId && e.Status == "Active" && !e.IsDeleted));

        var fees = await q.ToListAsync();
        return fees
            .GroupBy(sf => sf.StudentId)
            .Select(g =>
            {
                var first = g.First();
                var ae = first.Student.Enrollments?.FirstOrDefault(e => e.Status == "Active" && !e.IsDeleted);
                return new FeeReportRowDto
                {
                    StudentId = g.Key,
                    StudentName = first.Student.FirstName + " " + first.Student.LastName,
                    AdmissionNo = first.Student.AdmissionNo,
                    ClassName = ae?.Class != null ? $"{ae.Class.ClassName} {ae.Class.Section}" : "",
                    TotalDue = g.Sum(x => x.AmountDue), TotalPaid = g.Sum(x => x.AmountPaid),
                    TotalDiscount = g.Sum(x => x.Discount),
                    UnpaidCount = (g.Sum(x => x.AmountDue) - g.Sum(x => x.Discount) - g.Sum(x => x.AmountPaid)) > 0 && g.Sum(x => x.AmountPaid) == 0 ? 1 : 0,
                    PaidCount   = (g.Sum(x => x.AmountDue) - g.Sum(x => x.Discount) - g.Sum(x => x.AmountPaid)) <= 0 ? 1 : 0,
                    PartialCount = (g.Sum(x => x.AmountDue) - g.Sum(x => x.Discount) - g.Sum(x => x.AmountPaid)) > 0 && g.Sum(x => x.AmountPaid) > 0 ? 1 : 0
                };
            })
            .OrderBy(r => r.StudentName)
            .ToList();
    }
}
