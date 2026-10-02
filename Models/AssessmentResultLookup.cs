namespace SchoolManagement.API.Models;

/// <summary>
/// Configurable result status for classroom assessments / sabaq.
/// Codes are used in storage; labels (EN/UR) are for display.
/// System defaults use InstituteId = null; institutes may add their own.
/// </summary>
public class AssessmentResultLookup : BaseEntity
{
    public int AssessmentResultLookupId { get; set; }
    /// <summary>Stable code e.g. Remembered, PartiallyRemembered.</summary>
    public string Code { get; set; } = string.Empty;
    public string LabelEn { get; set; } = string.Empty;
    public string LabelUr { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
