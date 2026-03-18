using StudyHelperAPI.Models.Classroom;

namespace StudyHelperAPI.Services.Interfaces;

public interface IClassroomService
{
    Task<List<ClassroomCourse>> GetCoursesAsync();
    Task<List<ClassroomMaterial>> GetMaterialsAsync(string courseId);
    Task<List<ClassroomAssignment>> GetAssignmentsAsync(string courseId);
    Task<List<ClassroomAssignment>> GetPendingAssignmentsAsync();

}
