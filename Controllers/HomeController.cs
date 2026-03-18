using Microsoft.AspNetCore.Mvc;

namespace StudyHelperAPI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}