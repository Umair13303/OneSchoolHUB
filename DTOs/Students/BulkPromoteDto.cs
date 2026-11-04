using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>Body for <c>POST /api/students/promote-bulk</c>.</summary>
public class BulkPromoteDto
{
    [Required] public int SourceClassId { get; set; }
    [Required] public int SourceAcademicYearId { get; set; }
    [Required] public int TargetClassId { get; set; }
    [Required] public int TargetAcademicYearId { get; set; }

    /// <summary>If empty, all Active students in the source class/year are promoted.</summary>
    public List<int>? StudentIds { get; set; }

    public DateOnly? EffectiveDate { get; set; }
}
