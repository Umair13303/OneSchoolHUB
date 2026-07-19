namespace SchoolManagement.API.DTOs.Exam;

public class ExamScheduleDto
{
    public int      ExamScheduleId    { get; set; }
    public int      ExamPaperId       { get; set; }
    public DateOnly ExamDate          { get; set; }
    public TimeOnly StartTime         { get; set; }
    public TimeOnly? EndTime          { get; set; }
    public string?  RoomOrHall        { get; set; }
    public int?     InvigilatorUserId { get; set; }
    public string?  InvigilatorName   { get; set; }
    public string   Status            { get; set; } = "Scheduled";
    public string?  Remarks           { get; set; }
}

public class CreateExamScheduleDto
{
    public int      ExamPaperId       { get; set; }
    public DateOnly ExamDate          { get; set; }
    public TimeOnly StartTime         { get; set; }
    public TimeOnly? EndTime          { get; set; }
    public string?  RoomOrHall        { get; set; }
    public int?     InvigilatorUserId { get; set; }
    public string?  Remarks           { get; set; }
}

public class UpdateExamScheduleDto
{
    public DateOnly? ExamDate          { get; set; }
    public TimeOnly? StartTime         { get; set; }
    public TimeOnly? EndTime           { get; set; }
    public string?   RoomOrHall        { get; set; }
    public int?      InvigilatorUserId { get; set; }
    /// <summary>Scheduled | Rescheduled | Cancelled | Completed</summary>
    public string?   Status            { get; set; }
    public string?   Remarks           { get; set; }
}
