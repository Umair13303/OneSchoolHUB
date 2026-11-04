using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>Body for <c>POST /api/students/{id}/documents</c>.</summary>
public class TagStudentDocumentDto
{
    [Required] public int FileId { get; set; }

    /// <summary>student-photo | birth-certificate | prev-school-cert</summary>
    [Required, StringLength(50)]
    public string EntityType { get; set; } = string.Empty;
}
