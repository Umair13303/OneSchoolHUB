using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Ai;
using SchoolManagement.API.Services.Ai;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Phase 1 AI helpers. Generate returns ephemeral drafts only — never persists QuestionBankItem.
/// Save path remains existing POST /api/exam/question-bank after explicit user approval.
/// </summary>
[ApiController]
[Route("api/ai")]
[Authorize]
[Produces("application/json")]
public class AiController : ControllerBase
{
    private readonly IAiContentService _ai;

    public AiController(IAiContentService ai) => _ai = ai;

    private int CurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in token."));

    /// <summary>
    /// True only when the caller has teacher and none of admin/principal/superadmin.
    /// Matches ExamController / CurriculumController.
    /// </summary>
    private bool IsTeacherOnly()
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return roles.Contains("teacher")
            && !roles.Contains("admin")
            && !roles.Contains("principal")
            && !roles.Contains("superadmin");
    }

    /// <summary>
    /// Generate draft questions for Question Bank review. Does not write to the database.
    /// Roles: teacher, admin, principal (same as Question Bank). SuperAdmin is not granted access.
    /// </summary>
    [HttpPost("questions/generate")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> GenerateQuestions(
        [FromBody] GenerateQuestionsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ai.GenerateQuestionsAsync(
                request,
                CurrentUserId(),
                IsTeacherOnly(),
                cancellationToken);
            return Ok(result);
        }
        catch (AiDisabledException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
        }
        catch (AiRateLimitException ex)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (TimeoutException ex)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout, new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = ex.Message });
        }
    }
}
