namespace SchoolManagement.API.Services.Ai;

/// <summary>
/// Bound from configuration section "Ai". Secrets via env/user-secrets only (Ai:ApiKey / AI__ApiKey).
/// </summary>
public class AiGenerationOptions
{
    public const string SectionName = "Ai";

    /// <summary>Master switch. When false, generate returns 503.</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Adapter name: OpenAI | Fake (tests/local).</summary>
    public string Provider { get; set; } = "OpenAI";

    public string BaseUrl { get; set; } = "https://api.openai.com/v1";

    public string Model { get; set; } = "gpt-4o-mini";

    /// <summary>Backend-only. Never expose to Angular.</summary>
    public string ApiKey { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>Hard cap on questions per request (whitelist also enforces 5/10/20).</summary>
    public int MaxCount { get; set; } = 20;

    public int MaxInstructionsLength { get; set; } = 1000;

    /// <summary>Max generate calls per user in the rolling window.</summary>
    public int RateLimitPerUser { get; set; } = 10;

    /// <summary>Rolling window in minutes for rate limiting.</summary>
    public int RateLimitWindowMinutes { get; set; } = 10;

    /// <summary>Transient HTTP retries for 429/5xx (0–2).</summary>
    public int MaxRetries { get; set; } = 2;
}
