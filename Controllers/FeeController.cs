using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Fees;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/fees")]
[Authorize]
public class FeeController(IFeeService feeSvc, IParentAccessService parentAccess) : ControllerBase
{
    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    // ── Fee Types ────────────────────────────────────────────────────────────

    [HttpGet("types")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetFeeTypes() => Ok(await feeSvc.GetFeeTypesAsync());

    [HttpPost("types")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateFeeType([FromBody] CreateFeeTypeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { error = "Fee type name is required." });
        return CreatedAtAction(nameof(GetFeeTypes), await feeSvc.CreateFeeTypeAsync(dto, CurrentUserId));
    }

    [HttpPut("types/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateFeeType(int id, [FromBody] UpdateFeeTypeDto dto)
    {
        var result = await feeSvc.UpdateFeeTypeAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("types/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFeeType(int id)
        => await feeSvc.DeleteFeeTypeAsync(id) ? NoContent() : NotFound();

    // ── Fee Structures ───────────────────────────────────────────────────────

    [HttpGet("structures")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetFeeStructures([FromQuery] int? academicYearId, [FromQuery] int? classId)
        => Ok(await feeSvc.GetFeeStructuresAsync(academicYearId, classId));

    [HttpPost("structures")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateFeeStructure([FromBody] CreateFeeStructureDto dto)
    {
        if (dto.Amount <= 0) return BadRequest(new { error = "Amount must be greater than zero." });
        return CreatedAtAction(nameof(GetFeeStructures), await feeSvc.CreateFeeStructureAsync(dto, CurrentUserId));
    }

    [HttpPut("structures/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateFeeStructure(int id, [FromBody] UpdateFeeStructureDto dto)
    {
        var result = await feeSvc.UpdateFeeStructureAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("structures/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFeeStructure(int id)
        => await feeSvc.DeleteFeeStructureAsync(id) ? NoContent() : NotFound();

    // ── Discount Policies ────────────────────────────────────────────────────

    [HttpGet("discount-policies")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetDiscountPolicies() => Ok(await feeSvc.GetDiscountPoliciesAsync());

    [HttpPost("discount-policies")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateDiscountPolicy([FromBody] CreateDiscountPolicyDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { error = "Policy name is required." });
        if (dto.Value < 0) return BadRequest(new { error = "Discount value cannot be negative." });
        if (dto.ValueType == "Percentage" && dto.Value > 100) return BadRequest(new { error = "Percentage cannot exceed 100." });
        return CreatedAtAction(nameof(GetDiscountPolicies), await feeSvc.CreateDiscountPolicyAsync(dto, CurrentUserId));
    }

    [HttpPut("discount-policies/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateDiscountPolicy(int id, [FromBody] UpdateDiscountPolicyDto dto)
    {
        var result = await feeSvc.UpdateDiscountPolicyAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("discount-policies/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteDiscountPolicy(int id)
        => await feeSvc.DeleteDiscountPolicyAsync(id) ? NoContent() : NotFound();

    // ── Student Discounts ─────────────────────────────────────────────────────

    [HttpGet("student-discounts")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetStudentDiscounts([FromQuery] int? studentId, [FromQuery] int? academicYearId)
        => Ok(await feeSvc.GetStudentDiscountsAsync(studentId, academicYearId));

    [HttpPost("student-discounts")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignStudentDiscount([FromBody] AssignStudentDiscountDto dto)
    {
        try
        {
            return Ok(await feeSvc.AssignStudentDiscountAsync(dto, CurrentUserId));
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("student-discounts/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateStudentDiscount(int id, [FromBody] UpdateStudentDiscountDto dto)
    {
        var result = await feeSvc.UpdateStudentDiscountAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("student-discounts/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteStudentDiscount(int id)
        => await feeSvc.DeleteStudentDiscountAsync(id) ? NoContent() : NotFound();

    // ── Student Fees ─────────────────────────────────────────────────────────

    /// <summary>
    /// Parents may call this too, but ONLY with their own child's studentId —
    /// no studentId means "list everything", which is an admin-only view.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin,principal,parent")]
    public async Task<IActionResult> GetStudentFees(
        [FromQuery] int? studentId, [FromQuery] int? classId,
        [FromQuery] int? academicYearId, [FromQuery] string? status,
        [FromQuery] int? instituteId, [FromQuery] int? campusId)
    {
        if (User.IsInRole("parent"))
        {
            if (!studentId.HasValue)
                return BadRequest(new { error = "studentId is required." });
            if (!await parentAccess.IsGuardianOfAsync(CurrentUserId, studentId.Value))
                return Forbid();
        }

        return Ok(await feeSvc.GetStudentFeesAsync(studentId, classId, academicYearId, status, instituteId, campusId));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "superadmin,admin,principal,parent")]
    public async Task<IActionResult> GetStudentFee(int id)
    {
        var result = await feeSvc.GetStudentFeeByIdAsync(id);
        if (result is null) return NotFound();

        if (User.IsInRole("parent") && !await parentAccess.IsGuardianOfAsync(CurrentUserId, result.StudentId))
            return Forbid();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateStudentFee([FromBody] CreateStudentFeeDto dto)
    {
        if (dto.AmountDue <= 0) return BadRequest(new { error = "Amount due must be greater than zero." });
        var result = await feeSvc.CreateStudentFeeAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetStudentFee), new { id = result.StudentFeeId }, result);
    }

    [HttpGet("preview/student")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> PreviewStudentFee(
        [FromQuery] int studentId, [FromQuery] int feeStructureId, [FromQuery] int academicYearId)
    {
        try { return Ok(await feeSvc.PreviewStudentFeeAsync(studentId, feeStructureId, academicYearId)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("preview/bulk")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> PreviewBulkAssign(
        [FromQuery] int feeStructureId, [FromQuery] int classId, [FromQuery] int academicYearId)
    {
        try { return Ok(await feeSvc.PreviewBulkAssignAsync(feeStructureId, classId, academicYearId)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("bulk-assign")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> BulkAssign([FromBody] BulkAssignFeeDto dto)
    {
        try
        {
            var result = await feeSvc.BulkAssignFeeAsync(dto, CurrentUserId);
            return Ok(new { created = result.Created, skipped = result.Skipped,
                            assigned = result.Fees.Count, fees = result.Fees });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateStudentFee(int id, [FromBody] UpdateStudentFeeDto dto)
    {
        var result = await feeSvc.UpdateStudentFeeAsync(id, dto, CurrentUserId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteStudentFee(int id)
        => await feeSvc.DeleteStudentFeeAsync(id) ? NoContent() : NotFound();

    // ── Payments ─────────────────────────────────────────────────────────────

    [HttpGet("{id}/payments")]
    [Authorize(Roles = "superadmin,admin,principal,parent")]
    public async Task<IActionResult> GetPayments(int id)
    {
        if (User.IsInRole("parent"))
        {
            var fee = await feeSvc.GetStudentFeeByIdAsync(id);
            if (fee is null) return NotFound();
            if (!await parentAccess.IsGuardianOfAsync(CurrentUserId, fee.StudentId))
                return Forbid();
        }

        return Ok(await feeSvc.GetPaymentsAsync(id));
    }

    [HttpPost("payments")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto dto)
    {
        if (dto.AmountPaid <= 0) return BadRequest(new { error = "Payment amount must be greater than zero." });
        try { return Ok(await feeSvc.RecordPaymentAsync(dto, CurrentUserId)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Reports ──────────────────────────────────────────────────────────────

    [HttpGet("report")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetFeeReport([FromQuery] int academicYearId, [FromQuery] int? classId)
    {
        if (academicYearId == 0) return BadRequest(new { error = "academicYearId is required." });
        return Ok(await feeSvc.GetFeeReportAsync(academicYearId, classId));
    }
}
