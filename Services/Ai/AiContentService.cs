using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Ai;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Services.Ai;

public class AiContentService : IAiContentService
{
    private static readonly int[] AllowedCounts = { 5, 10, 20 };
    private static readonly HashSet<string> AllowedDifficulties = new(StringComparer.OrdinalIgnoreCase)
        { "Easy", "Medium", "Hard" };
    private static readonly HashSet<string> AllowedLanguages = new(StringComparer.OrdinalIgnoreCase)
        { "en", "ur", "ur-en" };
    private static readonly HashSet<QuestionType> AllowedTypes = new()
    {
        QuestionType.MultipleChoice,
        QuestionType.TrueFalse,
        QuestionType.FillInBlanks,
        QuestionType.ShortQuestion
    };

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AppDbContext _db;
    private readonly IExamService _examService;
    private readonly IAiProvider _provider;
    private readonly IMemoryCache _cache;
    private readonly AiGenerationOptions _options;
    private readonly ILogger<AiContentService> _logger;

    public AiContentService(
        AppDbContext db,
        IExamService examService,
        IAiProvider provider,
        IMemoryCache cache,
        IOptions<AiGenerationOptions> options,
        ILogger<AiContentService> logger)
    {
        _db = db;
        _examService = examService;
        _provider = provider;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<GenerateQuestionsResponse> GenerateQuestionsAsync(
        GenerateQuestionsRequest request,
        int userId,
        bool isTeacherOnly,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            throw new AiDisabledException("AI question generation is currently disabled.");

        ValidateRequest(request);

        // Authorization + topic integrity BEFORE any provider call.
        var ctx = await LoadAndAuthorizeTopicAsync(request, userId, isTeacherOnly);

        EnforceRateLimit(userId);

        var (system, user) = AiPromptBuilder.Build(request, ctx);
        var raw = await _provider.CompleteJsonAsync(new AiCompletionRequest
        {
            SystemPrompt = system,
            UserPrompt = user
        }, cancellationToken);

        var (items, warnings) = ParseAndNormalize(raw, request);

        _logger.LogInformation(
            "AI generate ok user={UserId} topic={TopicId} count={Count}/{Requested} type={Type}",
            userId, request.CourseTopicId, items.Count, request.Count, request.QuestionType);

        return new GenerateQuestionsResponse
        {
            DraftId = Guid.NewGuid().ToString("N"),
            GeneratedAtUtc = DateTime.UtcNow,
            Model = _options.Model,
            Items = items,
            Warnings = warnings
        };
    }

    private void ValidateRequest(GenerateQuestionsRequest request)
    {
        if (request.ClassId <= 0 || request.SubjectId <= 0 || request.CourseTopicId <= 0)
            throw new ArgumentException("ClassId, SubjectId, and CourseTopicId are required.");

        if (!AllowedCounts.Contains(request.Count) || request.Count > _options.MaxCount)
            throw new ArgumentException("Count must be 5, 10, or 20.");

        if (!AllowedTypes.Contains(request.QuestionType))
            throw new ArgumentException("QuestionType must be MCQ, True/False, Fill in the Blank, or Short Answer.");

        if (string.IsNullOrWhiteSpace(request.Difficulty) || !AllowedDifficulties.Contains(request.Difficulty.Trim()))
            throw new ArgumentException("Difficulty must be Easy, Medium, or Hard.");

        if (string.IsNullOrWhiteSpace(request.Language) || !AllowedLanguages.Contains(request.Language.Trim()))
            throw new ArgumentException("Language must be en, ur, or ur-en.");

        if (request.Marks <= 0)
            throw new ArgumentException("Marks must be greater than zero.");

        if (request.Instructions != null && request.Instructions.Length > _options.MaxInstructionsLength)
            throw new ArgumentException($"Instructions must be at most {_options.MaxInstructionsLength} characters.");
    }

    private async Task<AiTopicContext> LoadAndAuthorizeTopicAsync(
        GenerateQuestionsRequest request,
        int userId,
        bool isTeacherOnly)
    {
        var ctx = await _db.CourseTopics.AsNoTracking()
            .Where(t => t.CourseTopicId == request.CourseTopicId && !t.IsDeleted)
            .Select(t => new AiTopicContext
            {
                CourseTopicId = t.CourseTopicId,
                TopicTitle = t.Title,
                ChapterTitle = t.CourseChapter.Title,
                ClassId = t.CourseChapter.CoursePlan.ClassId,
                SubjectId = t.CourseChapter.CoursePlan.SubjectId,
                ClassName = t.CourseChapter.CoursePlan.Class.ClassName,
                SubjectName = t.CourseChapter.CoursePlan.Subject.SubjectName
            })
            .FirstOrDefaultAsync()
            ?? throw new ArgumentException("Topic not found.");

        // Topic must belong to the requested Class + Subject (IDOR / mismatch protection).
        if (ctx.ClassId != request.ClassId || ctx.SubjectId != request.SubjectId)
            throw new UnauthorizedAccessException(
                "Topic does not belong to the selected class and subject.");

        if (isTeacherOnly)
        {
            await _examService.EnsureTeacherCanAccessClassSubjectAsync(userId, request.ClassId, request.SubjectId);
            await _examService.EnsureTeacherCanAccessTopicAsync(userId, request.CourseTopicId);
        }

        return ctx;
    }

    private void EnforceRateLimit(int userId)
    {
        var window = TimeSpan.FromMinutes(Math.Max(1, _options.RateLimitWindowMinutes));
        var max = Math.Max(1, _options.RateLimitPerUser);
        var key = $"ai:gen:{userId}";

        var stamps = _cache.GetOrCreate(key, e =>
        {
            e.AbsoluteExpirationRelativeToNow = window;
            return new List<DateTime>();
        })!;

        lock (stamps)
        {
            var cutoff = DateTime.UtcNow - window;
            stamps.RemoveAll(t => t < cutoff);
            if (stamps.Count >= max)
                throw new AiRateLimitException(
                    $"Rate limit exceeded. Maximum {max} generation requests per {window.TotalMinutes:0} minutes.");

            stamps.Add(DateTime.UtcNow);
        }
    }

    internal static (List<GeneratedQuestionDraftDto> Items, List<string> Warnings) ParseAndNormalize(
        string rawJson,
        GenerateQuestionsRequest request)
    {
        var warnings = new List<string>();
        rawJson = StripMarkdownFence(rawJson.Trim());

        ProviderPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<ProviderPayload>(rawJson, JsonOpts);
        }
        catch (JsonException)
        {
            throw new ArgumentException("AI returned invalid JSON.");
        }

        if (payload?.Questions == null || payload.Questions.Count == 0)
            throw new ArgumentException("AI returned no questions.");

        var items = new List<GeneratedQuestionDraftDto>();
        var lang = NormalizeLanguage(request.Language);

        foreach (var q in payload.Questions)
        {
            if (items.Count >= request.Count)
            {
                warnings.Add("Extra questions beyond the requested count were ignored.");
                break;
            }

            if (!TryNormalizeOne(q, request, lang, out var draft, out var warn))
            {
                if (!string.IsNullOrEmpty(warn))
                    warnings.Add(warn);
                continue;
            }

            items.Add(draft!);
        }

        if (items.Count == 0)
            throw new ArgumentException("AI returned no valid questions for the requested type.");

        if (items.Count < request.Count)
            warnings.Add($"Only {items.Count} of {request.Count} requested questions were valid.");

        return (items, warnings);
    }

    private static bool TryNormalizeOne(
        ProviderQuestion q,
        GenerateQuestionsRequest request,
        string language,
        out GeneratedQuestionDraftDto? draft,
        out string? warning)
    {
        draft = null;
        warning = null;

        var text = SanitizeText(q.QuestionText);
        if (string.IsNullOrWhiteSpace(text))
        {
            warning = "Skipped a question with empty text.";
            return false;
        }

        // Type must match request — do not trust AI type.
        if (q.QuestionType != 0 && (QuestionType)q.QuestionType != request.QuestionType)
        {
            warning = "Skipped a question with mismatched question type.";
            return false;
        }

        var marks = q.Marks > 0 ? q.Marks : request.Marks;
        if (marks <= 0)
        {
            warning = "Skipped a question with invalid marks.";
            return false;
        }

        var note = SanitizeText(q.QuestionNote);
        var correct = SanitizeText(q.CorrectAnswer);
        var options = (q.Options ?? new List<ProviderOption>())
            .Select((o, i) => new GeneratedQuestionOptionDto
            {
                OptionLabel = SanitizeText(o.OptionLabel) ?? "",
                OptionText = SanitizeText(o.OptionText) ?? "",
                IsCorrect = o.IsCorrect,
                SortOrder = o.SortOrder > 0 ? o.SortOrder : i + 1
            })
            .Where(o => !string.IsNullOrWhiteSpace(o.OptionText))
            .ToList();

        switch (request.QuestionType)
        {
            case QuestionType.MultipleChoice:
                if (options.Count != 4 || options.Count(o => o.IsCorrect) != 1)
                {
                    warning = "Skipped an invalid MCQ (need exactly 4 options and one correct).";
                    return false;
                }
                // Normalize labels A–D
                for (var i = 0; i < 4; i++)
                {
                    options[i].OptionLabel = ((char)('A' + i)).ToString();
                    options[i].SortOrder = i + 1;
                }
                correct = null;
                break;

            case QuestionType.TrueFalse:
                if (q.IsTrue is null)
                {
                    warning = "Skipped a True/False question without IsTrue.";
                    return false;
                }
                options = new List<GeneratedQuestionOptionDto>();
                correct = null;
                break;

            case QuestionType.FillInBlanks:
            case QuestionType.ShortQuestion:
                if (string.IsNullOrWhiteSpace(correct))
                {
                    warning = "Skipped a question without CorrectAnswer.";
                    return false;
                }
                options = new List<GeneratedQuestionOptionDto>();
                break;

            default:
                warning = "Unsupported question type.";
                return false;
        }

        draft = new GeneratedQuestionDraftDto
        {
            ClientKey = Guid.NewGuid().ToString("N"),
            CourseTopicId = request.CourseTopicId, // always from request
            QuestionType = request.QuestionType,
            QuestionText = text!,
            Language = language,
            Marks = marks,
            CorrectAnswer = correct,
            IsTrue = request.QuestionType == QuestionType.TrueFalse ? q.IsTrue : null,
            QuestionNote = note,
            Difficulty = request.Difficulty.Trim(),
            Options = options
        };
        return true;
    }

    private static string NormalizeLanguage(string language)
    {
        var l = language.Trim().ToLowerInvariant();
        return l is "ur" or "ur-en" ? l : "en";
    }

    private static string? SanitizeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var s = input.Trim();
        s = Regex.Replace(s, @"<script\b[^<]*(?:(?!</script>)<[^<]*)*</script>", "", RegexOptions.IgnoreCase);
        s = Regex.Replace(s, @"</?\s*script\b[^>]*>", "", RegexOptions.IgnoreCase);
        s = Regex.Replace(s, @"on\w+\s*=\s*([""']).*?\1", "", RegexOptions.IgnoreCase);
        // Strip remaining tags but keep text
        s = Regex.Replace(s, @"<[^>]+>", "");
        return s.Trim();
    }

    private static string StripMarkdownFence(string raw)
    {
        if (!raw.StartsWith("```")) return raw;
        var lines = raw.Split('\n');
        if (lines.Length < 2) return raw;
        var end = lines.Length - 1;
        if (lines[end].Trim().StartsWith("```")) end--;
        return string.Join('\n', lines.Skip(1).Take(end));
    }

    private sealed class ProviderPayload
    {
        public List<ProviderQuestion> Questions { get; set; } = new();
    }

    private sealed class ProviderQuestion
    {
        public int QuestionType { get; set; }
        public string? QuestionText { get; set; }
        public string? Language { get; set; }
        public int Marks { get; set; }
        public string? CorrectAnswer { get; set; }
        public bool? IsTrue { get; set; }
        public string? QuestionNote { get; set; }
        public List<ProviderOption>? Options { get; set; }
    }

    private sealed class ProviderOption
    {
        public string? OptionLabel { get; set; }
        public string? OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public int SortOrder { get; set; }
    }
}
