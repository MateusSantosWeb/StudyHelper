using Microsoft.AspNetCore.Mvc;
using StudyHelperAPI.Services.Interfaces;

namespace StudyHelperAPI.Controllers;

public class ClassroomController : Controller
{
    private readonly IClassroomService _classroomService;
    private readonly IGeminiService _geminiService;

    public ClassroomController(
        IClassroomService classroomService,
        IGeminiService geminiService)
    {
        _classroomService = classroomService;
        _geminiService = geminiService;
        
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _classroomService.GetCoursesAsync();
        return View(courses);
    }

    public async Task<IActionResult> Materials(string id)
    {
        var materials = await _classroomService.GetMaterialsAsync(id);
        ViewBag.CourseId = id;
        return View(materials);
        
    }

    public async Task<IActionResult> Assignments(string id)
    {
        var assignments = await _classroomService.GetAssignmentsAsync(id);
        ViewBag.CourseId = id;
        return View(assignments);
    }

    public async Task<IActionResult> Pending()
    {
        var pending = await _classroomService.GetPendingAssignmentsAsync();
        return View(pending);
    }

    [HttpPost]
    public async Task<IActionResult> Summarize(string materiaId, string content, string title)
    {
        var result = await _geminiService.SummarizeMaterialAsync(materiaId, content);
        ViewBag.Title = title;
        return View(result);
        
    }

    [HttpPost]
    public async Task<IActionResult> AnswerActivity(string assignmentId, string question, string context)
    {
        var answer = await _geminiService.AnswerActivityAsync(question, context);
        ViewBag.Question = question;
        ViewBag.AssignmentId = assignmentId;
        return View("Answer",(object)answer);
    }
    
    
    
}
