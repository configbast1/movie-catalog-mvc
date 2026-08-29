using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication23333.Data;
using WebApplication23333.Models;

namespace WebApplication23333.Controllers;

public class MoviesController : Controller
{
    private const string UploadsFolder = "/images/uploads/";
    private const string PlaceholderPoster = "/images/poster-placeholder.svg";

    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MoviesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies
            .AsNoTracking()
            .OrderByDescending(m => m.Rating)
            .ThenBy(m => m.Title)
            .ToListAsync();

        return View(movies);
    }

    public async Task<IActionResult> Details(int? id)
    {
        var movie = await FindMovieAsync(id, asNoTracking: true);
        if (movie is null)
        {
            return NotFound();
        }

        return View(movie);
    }

    /// <summary>
    /// Серверна частина клієнтської перевірки [Remote] на унікальність назви.
    /// Викликається з браузера через AJAX і має повертати JSON true/false.
    /// </summary>
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckTitle(string? title, int id)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Json(true);
        }

        var normalized = title.Trim();
        var exists = await _context.Movies.AnyAsync(m => m.Id != id && m.Title == normalized);

        return Json(!exists);
    }

    public IActionResult Create()
    {
        return View(new Movie { ReleaseYear = DateTime.Now.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        // шлях до постера формується на сервері, тому з перевірки форми його прибираємо
        ModelState.Remove(nameof(Movie.PosterPath));

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        // файл уже пройшов перевірку атрибутом PosterFile, лишилось зберегти
        var posterPath = await SavePosterAsync(movie.PosterFile);

        movie.Id = 0;
        movie.PosterPath = posterPath ?? PlaceholderPoster;

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Фільм «{movie.Title}» додано.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var movie = await FindMovieAsync(id, asNoTracking: true);
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
        if (id != movie.Id)
        {
            return NotFound();
        }

        var existing = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        var newPosterPath = await SavePosterAsync(movie.PosterFile);
        var previousPosterPath = existing.PosterPath;

        existing.Title = movie.Title;
        existing.OriginalTitle = movie.OriginalTitle;
        existing.Director = movie.Director;
        existing.Genre = movie.Genre;
        existing.ReleaseYear = movie.ReleaseYear;
        existing.Description = movie.Description;
        existing.Rating = movie.Rating;

        if (newPosterPath is not null)
        {
            existing.PosterPath = newPosterPath;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Movies.AnyAsync(m => m.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        if (newPosterPath is not null)
        {
            DeleteUploadedPoster(previousPosterPath);
        }

        TempData["Message"] = $"Фільм «{existing.Title}» оновлено.";
        return RedirectToAction(nameof(Details), new { id = existing.Id });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var movie = await FindMovieAsync(id, asNoTracking: true);
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
        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        DeleteUploadedPoster(movie.PosterPath);

        TempData["Message"] = $"Фільм «{movie.Title}» видалено.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Movie?> FindMovieAsync(int? id, bool asNoTracking)
    {
        if (id is null)
        {
            return null;
        }

        var query = _context.Movies.AsQueryable();
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(m => m.Id == id);
    }

    private async Task<string?> SavePosterAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadsDirectory = Path.Combine(webRoot, "images", "uploads");
        Directory.CreateDirectory(uploadsDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDirectory, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        return $"{UploadsFolder}{fileName}";
    }

    private void DeleteUploadedPoster(string? posterPath)
    {
        if (string.IsNullOrWhiteSpace(posterPath))
        {
            return;
        }

        if (!posterPath.StartsWith(UploadsFolder, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var relative = posterPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(webRoot, relative);

        try
        {
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
