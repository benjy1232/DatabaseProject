using Microsoft.AspNetCore.Mvc;

namespace DatabaseProject.Controllers;

public class AlbumController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}