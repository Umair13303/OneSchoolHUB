namespace SchoolManagement.API.Services.Ai;

public class AiCompletionRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserPrompt { get; set; } = string.Empty;
    public string? JsonSchemaHint { get; set; }
}

/// <summary>
/// Provider-agnostic JSON generation. Implementations must never be called from Angular.
/// </summary>
public interface IAiProvider
{
    Task<string> CompleteJsonAsync(AiCompletionRequest request, CancellationToken cancellationToken = default);
}
