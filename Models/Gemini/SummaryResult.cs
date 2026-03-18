namespace StudyHelperAPI.Models.Gemini;

public class SummaryResult
{
    public string MateriaId { get; set; }
    public string MateriaTitle { get; set; }
    public string Summary { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}