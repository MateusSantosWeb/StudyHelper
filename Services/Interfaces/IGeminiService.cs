using StudyHelperAPI.Models.Gemini;

namespace StudyHelperAPI.Services.Interfaces;

public interface IGeminiService
{
    Task<SummaryResult> SummarizeMaterialAsync(string materialId, string content);
    Task<string> AnswerActivityAsync(string question, string context);
    Task<string> AskAsync(string prompt);
}