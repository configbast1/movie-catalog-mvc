using Microsoft.AspNetCore.Mvc;

namespace Kino.PL.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Movies");

    [Route("Home/StatusCodeHandler/{statusCode:int}")]
    public IActionResult StatusCodeHandler(int statusCode)
    {
        if (statusCode == 404)
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        Response.StatusCode = statusCode;
        ViewData["StatusCode"] = statusCode;
        return View("Error");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
