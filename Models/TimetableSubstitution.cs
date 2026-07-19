namespace SchoolManagement.API.Models;

/// <summary>
/// Overrides a single timetable slot for a specific calendar date.
/// When a substitute teacher covers an absent teacher on a specific day,
/// a row is inserted here instead of touching the base timetable.
/// </summary>
public class TimetableSubstitution
{
    public int      SubstitutionId      { get; set; }
    public int      TimetableId         { get; set; }   // FK → SchoolTimetable (the original slot)
    public DateOnly Date                { get; set; }   // e.g. 2026-06-10
    public int      SubstituteTeacherId { get; set; }   // FK → User
    public string   Reason              { get; set; } = "";
    public bool     IsDeleted           { get; set; } = false;
    public int?     CreatedBy           { get; set; }
    public DateTime CreatedAt           { get; set; } = DateTime.UtcNow;
    public int?     UpdatedBy           { get; set; }
    public DateTime? UpdatedAt          { get; set; }
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }

    public SchoolTimetable Timetable          { get; set; } = null!;
    public User            SubstituteTeacher  { get; set; } = null!;
}
