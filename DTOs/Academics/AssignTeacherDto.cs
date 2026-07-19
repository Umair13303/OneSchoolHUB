using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

/// <summary>
/// Body for <c>POST /api/classes/{classId}/assign-teacher</c>.
/// Assigns a specific teacher to teach a specific subject in this class.
/// </summary>
public class AssignTeacherDto
{
    [Required] public int  SubjectId { get; set; }
    public int? TeacherId { get; set; }
}
