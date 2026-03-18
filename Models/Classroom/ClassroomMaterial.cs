namespace StudyHelperAPI.Models.Classroom;

public class ClassroomMaterial
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> AttachmentUrls { get; set; }
}