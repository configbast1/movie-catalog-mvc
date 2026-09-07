using Microsoft.EntityFrameworkCore;
using WebApplication23333.Data;
using WebApplication23333.Models;

namespace WebApplication23333.Services;

public class MovieService : IMovieService
{
    private readonly AppDbContext _context;
    private readonly IPosterStorage _posterStorage;
    private readonly ILogger<MovieService> _logger;

    public MovieService(AppDbContext context, IPosterStorage posterStorage, ILogger<MovieService> logger)
    {
        _context = context;
        _posterStorage = posterStorage;
        _logger = logger;
    }

    public string PlaceholderPosterPath => _posterStorage.PlaceholderPath;

    public async Task<IReadOnlyList<Movie>> GetAllAsync()
    {
        return await _context.Movies
            .AsNoTracking()
            .OrderByDescending(m => m.Rating)
            .ThenBy(m => m.Title)
            .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int? id)
    {
        if (id is null)
        {
            return null;
        }

        return await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public Task<bool> ExistsAsync(int id) => _context.Movies.AnyAsync(m => m.Id == id);

    public async Task<Movie> CreateAsync(Movie movie)
    {
        var posterPath = await _posterStorage.SaveAsync(movie.PosterFile);

        movie.Id = 0;
        movie.PosterPath = posterPath ?? _posterStorage.PlaceholderPath;

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Додано фільм {Title} (Id = {Id})", movie.Title, movie.Id);

        return movie;
    }

    public async Task<Movie?> UpdateAsync(int id, Movie movie)
    {
        var existing = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        if (existing is null)
        {
            return null;
        }

        var newPosterPath = await _posterStorage.SaveAsync(movie.PosterFile);
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
            if (!await ExistsAsync(id))
            {
                return null;
            }

            throw;
        }

        if (newPosterPath is not null)
        {
            _posterStorage.Delete(previousPosterPath);
        }

        _logger.LogInformation("Оновлено фільм {Title} (Id = {Id})", existing.Title, existing.Id);

        return existing;
    }

    public async Task<Movie?> DeleteAsync(int id)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        {
            return null;
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        _posterStorage.Delete(movie.PosterPath);

        _logger.LogInformation("Видалено фільм {Title} (Id = {Id})", movie.Title, movie.Id);

        return movie;
    }

    public async Task<bool> IsTitleUniqueAsync(string? title, int excludeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return true;
        }

        var normalized = title.Trim();

        return !await _context.Movies
            .AsNoTracking()
            .AnyAsync(m => m.Id != excludeId && m.Title == normalized);
    }

    public bool IsTitleUnique(string? title, int excludeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return true;
        }

        var normalized = title.Trim();

        return !_context.Movies
            .AsNoTracking()
            .Any(m => m.Id != excludeId && m.Title == normalized);
    }
}
