namespace SchoolManagement.API.DTOs.Academics;

/// <summary>
/// Assign (or clear) the class teacher for one class.
/// When a teacher is set, all existing ClassSubject rows for that class
/// are also pointed at the same teacher (full-day / early-grade model).
/// </summary>
public class AssignClassTeacherDto
{
    /// <summary>Null or 0 clears the class teacher (does not wipe subject teachers).</summary>
    public int? TeacherId { get; set; }
}
