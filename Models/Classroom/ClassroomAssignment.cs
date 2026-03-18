namespace StudyHelperAPI.Models.Classroom;

public class ClassroomAssignment
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CourseId { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxPoints { get; set; }
    public string State { get; set; }
    public bool IsCompleted { get; set; }
    
}
