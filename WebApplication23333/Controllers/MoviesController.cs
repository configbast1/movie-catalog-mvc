using Microsoft.AspNetCore.Mvc;
using WebApplication23333.Models;
using WebApplication23333.Services;

namespace WebApplication23333.Controllers;

public class MoviesController : Controller
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _movieService.GetAllAsync();
        return View(movies);
    }

    public async Task<IActionResult> Details(int? id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckTitle(string? title, int id)
    {
        var isUnique = await _movieService.IsTitleUniqueAsync(title, id);
        return Json(isUnique);
    }

    public IActionResult Create()
    {
        return View(new Movie { ReleaseYear = DateTime.Now.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        ModelState.Remove(nameof(Movie.PosterPath));

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        var created = await _movieService.CreateAsync(movie);

        TempData["Message"] = $"Фільм «{created.Title}» додано.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie)
    {
        if (id != movie.Id || !await _movieService.ExistsAsync(id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        var updated = await _movieService.UpdateAsync(id, movie);
        if (updated is null)
        {
            return NotFound();
        }

        TempData["Message"] = $"Фільм «{updated.Title}» оновлено.";
        return RedirectToAction(nameof(Details), new { id = updated.Id });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);
        if (deleted is null)
        {
            return NotFound();
        }

        TempData["Message"] = $"Фільм «{deleted.Title}» видалено.";
        return RedirectToAction(nameof(Index));
    }
}
