using SchoolManagement.API.DTOs.Ai;

namespace SchoolManagement.API.Services.Ai;

public interface IAiContentService
{
    /// <summary>
    /// Authorize + generate ephemeral draft. Never persists QuestionBankItem.
    /// </summary>
    /// <param name="isTeacherOnly">True when caller has teacher role and not admin/principal/superadmin.</param>
    Task<GenerateQuestionsResponse> GenerateQuestionsAsync(
        GenerateQuestionsRequest request,
        int userId,
        bool isTeacherOnly,
        CancellationToken cancellationToken = default);
}

/// <summary>AI feature is disabled via configuration.</summary>
public class AiDisabledException : Exception
{
    public AiDisabledException(string message) : base(message) { }
}

/// <summary>Per-user generation rate limit exceeded.</summary>
public class AiRateLimitException : Exception
{
    public AiRateLimitException(string message) : base(message) { }
}
