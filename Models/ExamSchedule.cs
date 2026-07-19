namespace SchoolManagement.API.Models;

/// <summary>
/// Schedules a specific ExamPaper for a date, time, and room.
/// One paper can have multiple schedule rows (e.g. morning & evening shifts,
/// or different rooms for different sections of the same class).
/// </summary>
public class ExamSchedule : BaseEntity
{
    public int ExamScheduleId { get; set; }
    public int ExamPaperId    { get; set; }

    // ── When ─────────────────────────────────────────────────────────────────
    public DateOnly  ExamDate  { get; set; }
    public TimeOnly  StartTime { get; set; }
    public TimeOnly? EndTime   { get; set; }

    // ── Where ─────────────────────────────────────────────────────────────────
    /// <summary>Room / Hall number e.g. "Room 5", "Main Hall".</summary>
    public string? RoomOrHall { get; set; }

    // ── Invigilator ───────────────────────────────────────────────────────────
    public int? InvigilatorUserId { get; set; }

    // ── Status ───────────────────────────────────────────────────────────────
    /// <summary>Scheduled | Rescheduled | Cancelled | Completed</summary>
    public string Status { get; set; } = "Scheduled";

    public string? Remarks { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public ExamPaper ExamPaper          { get; set; } = null!;
    public User?     InvigilatorUser    { get; set; }
}
