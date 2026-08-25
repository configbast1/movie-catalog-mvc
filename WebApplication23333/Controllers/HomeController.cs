using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication23333.Data;

namespace WebApplication23333.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var topMovies = await _context.Movies
            .AsNoTracking()
            .OrderByDescending(m => m.Rating)
            .ThenBy(m => m.Title)
            .Take(10)
            .ToListAsync();

        return View(topMovies);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var movie = await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
