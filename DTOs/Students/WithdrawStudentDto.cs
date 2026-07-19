using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

public class WithdrawStudentDto
{
    [Required] public DateOnly LeavingDate   { get; set; }
    [Required, StringLength(500)] public string LeavingReason { get; set; } = string.Empty;
    /// <summary>If true, withdraw even if outstanding dues exist.</summary>
    public bool ForceWithdraw { get; set; } = false;
}
