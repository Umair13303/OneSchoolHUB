namespace SchoolManagement.API.Models;

public class AcademicYear : BaseEntity
{
    public int AcademicYearId { get; set; }
    public string YearLabel { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<StudentClassEnrollment> Enrollments { get; set; } = new List<StudentClassEnrollment>();
}
