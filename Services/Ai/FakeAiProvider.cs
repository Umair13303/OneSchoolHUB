using System.Text.Json;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services.Ai;

/// <summary>
/// Deterministic provider for local/dev (Ai:Provider=Fake) and automated tests.
/// Does not call any external service.
/// </summary>
public class FakeAiProvider : IAiProvider
{
    /// <summary>When set, returns this raw JSON instead of generating valid samples.</summary>
    public string? OverrideJson { get; set; }

    /// <summary>Incremented each call — tests assert provider was / was not invoked.</summary>
    public int CallCount { get; private set; }

    public Task<string> CompleteJsonAsync(AiCompletionRequest request, CancellationToken cancellationToken = default)
    {
        CallCount++;
        if (OverrideJson != null)
            return Task.FromResult(OverrideJson);

        // Infer count and type from user prompt heuristics (tests also set OverrideJson).
        var count = 5;
        var type = QuestionType.MultipleChoice;
        if (request.UserPrompt.Contains("Count: 10")) count = 10;
        else if (request.UserPrompt.Contains("Count: 20")) count = 20;
        else if (request.UserPrompt.Contains("Count: 5")) count = 5;

        // Match the "Question type:" line only — rules text also mentions other types.
        if (request.UserPrompt.Contains("Question type: True/False")) type = QuestionType.TrueFalse;
        else if (request.UserPrompt.Contains("Question type: Fill in the Blank")) type = QuestionType.FillInBlanks;
        else if (request.UserPrompt.Contains("Question type: Short Answer")) type = QuestionType.ShortQuestion;
        else type = QuestionType.MultipleChoice;

        var questions = new List<object>();
        for (var i = 1; i <= count; i++)
        {
            questions.Add(BuildOne(type, i));
        }

        var payload = JsonSerializer.Serialize(new { questions }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return Task.FromResult(payload);
    }

    private static object BuildOne(QuestionType type, int i) => type switch
    {
        QuestionType.TrueFalse => new
        {
            questionType = (int)type,
            questionText = $"Fake TF question {i}?",
            language = "en",
            marks = 1,
            correctAnswer = (string?)null,
            isTrue = i % 2 == 0,
            questionNote = "Fake explanation",
            options = Array.Empty<object>()
        },
        QuestionType.FillInBlanks or QuestionType.ShortQuestion => new
        {
            questionType = (int)type,
            questionText = $"Fake open question {i}?",
            language = "en",
            marks = 2,
            correctAnswer = $"Answer {i}",
            isTrue = (bool?)null,
            questionNote = "Fake explanation",
            options = Array.Empty<object>()
        },
        _ => new
        {
            questionType = (int)QuestionType.MultipleChoice,
            questionText = $"Fake MCQ question {i}?",
            language = "en",
            marks = 1,
            correctAnswer = (string?)null,
            isTrue = (bool?)null,
            questionNote = "Fake explanation",
            options = new object[]
            {
                new { optionLabel = "A", optionText = "Option A", isCorrect = true, sortOrder = 1 },
                new { optionLabel = "B", optionText = "Option B", isCorrect = false, sortOrder = 2 },
                new { optionLabel = "C", optionText = "Option C", isCorrect = false, sortOrder = 3 },
                new { optionLabel = "D", optionText = "Option D", isCorrect = false, sortOrder = 4 }
            }
        }
    };
}
