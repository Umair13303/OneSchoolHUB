using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Timetable;

public class CreateSubstitutionDto
{
    [Required] public int    TimetableId         { get; set; }
    [Required] public string Date                { get; set; } = ""; // "yyyy-MM-dd"
    [Required] public int    SubstituteTeacherId { get; set; }
               public string Reason              { get; set; } = "";
}

public class UpdateSubstitutionDto
{
    [Required] public int    SubstituteTeacherId { get; set; }
               public string Reason              { get; set; } = "";
}

public class SubstitutionDto
{
    public int    SubstitutionId        { get; set; }
    public int    TimetableId           { get; set; }
    public string Date                  { get; set; } = "";
    public int    OriginalTeacherId     { get; set; }
    public string OriginalTeacherName   { get; set; } = "";
    public int    SubstituteTeacherId   { get; set; }
    public string SubstituteTeacherName { get; set; } = "";
    public int    ClassId               { get; set; }
    public string ClassName             { get; set; } = "";
    public string Section               { get; set; } = "";
    public int    SubjectId             { get; set; }
    public string SubjectName           { get; set; } = "";
    public int    PeriodId              { get; set; }
    public int    PeriodNo              { get; set; }
    public string PeriodName            { get; set; } = "";
    public string StartTime             { get; set; } = "";
    public string EndTime               { get; set; } = "";
    public int    DayOfWeek             { get; set; }
    public string Reason                { get; set; } = "";
}

/// <summary>One slot in the day's timetable with optional substitution info.</summary>
public class DaySlotDto
{
    public int    TimetableId           { get; set; }
    public int    PeriodId              { get; set; }
    public int    PeriodNo              { get; set; }
    public string PeriodName            { get; set; } = "";
    public string StartTime             { get; set; } = "";
    public string EndTime               { get; set; } = "";
    public int    ClassId               { get; set; }
    public string ClassName             { get; set; } = "";
    public string Section               { get; set; } = "";
    public int    SubjectId             { get; set; }
    public string SubjectName           { get; set; } = "";
    public int    OriginalTeacherId     { get; set; }
    public string OriginalTeacherName   { get; set; } = "";
    // null = no substitution on this date
    public int?   SubstitutionId        { get; set; }
    public int?   SubstituteTeacherId   { get; set; }
    public string SubstituteTeacherName { get; set; } = "";
    public string Reason                { get; set; } = "";
}
