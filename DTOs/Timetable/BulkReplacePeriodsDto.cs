using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Timetable;

public class BulkReplacePeriodsDto
{
    [Required, MinLength(1)]
    public List<UpsertPeriodDto> Periods { get; set; } = new();
}

public class UpsertPeriodDto
{
    public int    PeriodNo   { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public string StartTime  { get; set; } = string.Empty;
    public string EndTime    { get; set; } = string.Empty;
    public bool   IsBreak    { get; set; } = false;
    public bool   IsActive   { get; set; } = true;
    public int?   DurationMinutes { get; set; }
}
