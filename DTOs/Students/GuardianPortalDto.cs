using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>
/// Body for <c>POST /api/students/{id}/guardians/{guardianId}/portal</c>.
/// Either link an existing parent user (<see cref="UserId"/>) or create one
/// (email optional — a synthetic email is generated when omitted).
/// </summary>
public class GuardianPortalDto
{
    /// <summary>Link an existing user (must be role parent). Mutually exclusive with create fields.</summary>
    public int? UserId { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    /// <summary>Optional; when creating, a random password is generated if omitted.</summary>
    [MinLength(6), StringLength(100)]
    public string? Password { get; set; }
}

public class GuardianPortalResultDto
{
    public int GuardianId { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    /// <summary>Only returned when a new account was created.</summary>
    public string? TemporaryPassword { get; set; }
    public bool Created { get; set; }
    /// <summary>Copy/paste invite text for WhatsApp/SMS/email (no SMTP required).</summary>
    public string? InviteMessage { get; set; }
}
