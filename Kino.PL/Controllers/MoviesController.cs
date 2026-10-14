using Kino.BLL.Interfaces;
using Kino.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Kino.PL.Controllers;

// контролер тонкий: жодного EF Core, жодного DbContext.
// він знає лише про інтерфейс IMovieService з бізнес-рівня
public class MoviesController : Controller
{
    private readonly IMovieService _movieService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public MoviesController(IMovieService movieService, IStringLocalizer<SharedResource> localizer)
    {
        _movieService = movieService;
        _localizer = localizer;
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
        return View(new MovieViewModel { ReleaseYear = DateTime.Now.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieViewModel model)
    {
        ModelState.Remove(nameof(MovieViewModel.PosterPath));

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var created = await _movieService.CreateAsync(model.ToDto());

        TempData["Message"] = _localizer["Messages.Created", created.Title].Value;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie.ToViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MovieViewModel model)
    {
        if (id != model.Id || !await _movieService.ExistsAsync(id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _movieService.UpdateAsync(id, model.ToDto());
        if (updated is null)
        {
            return NotFound();
        }

        TempData["Message"] = _localizer["Messages.Updated", updated.Title].Value;
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

        TempData["Message"] = _localizer["Messages.Deleted", deleted.Title].Value;
        return RedirectToAction(nameof(Index));
    }
}
