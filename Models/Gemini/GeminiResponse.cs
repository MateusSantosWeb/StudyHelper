namespace StudyHelperAPI.Models.Gemini;

public class GeminiResponse
{
    public List<GeminiCandidate> Candidates { get; set; } = new();
}
public class GeminiCandidate
{
    public GeminiContent Content { get; set; }
}