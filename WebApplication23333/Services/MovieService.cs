using WebApplication23333.Models;
using WebApplication23333.Repositories;

namespace WebApplication23333.Services;

public class MovieService : IMovieService
{
    private readonly IRepository<Movie> _repository;
    private readonly IPosterStorage _posterStorage;
    private readonly ILogger<MovieService> _logger;

    public MovieService(
        IRepository<Movie> repository,
        IPosterStorage posterStorage,
        ILogger<MovieService> logger)
    {
        _repository = repository;
        _posterStorage = posterStorage;
        _logger = logger;
    }

    public string PlaceholderPosterPath => _posterStorage.PlaceholderPath;

    public async Task<IReadOnlyList<Movie>> GetAllAsync()
    {
        return await _repository.ListAsync(
            orderBy: query => query
                .OrderByDescending(m => m.Rating)
                .ThenBy(m => m.Title));
    }

    public async Task<Movie?> GetByIdAsync(int? id)
    {
        if (id is null)
        {
            return null;
        }

        return await _repository.FirstOrDefaultAsync(m => m.Id == id, asNoTracking: true);
    }

    public Task<bool> ExistsAsync(int id) => _repository.AnyAsync(m => m.Id == id);

    public async Task<Movie> CreateAsync(Movie movie)
    {
        var posterPath = await _posterStorage.SaveAsync(movie.PosterFile);

        movie.Id = 0;
        movie.PosterPath = posterPath ?? _posterStorage.PlaceholderPath;

        await _repository.AddAsync(movie);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Додано фільм {Title} (Id = {Id})", movie.Title, movie.Id);

        return movie;
    }

    public async Task<Movie?> UpdateAsync(int id, Movie movie)
    {
        var existing = await _repository.FirstOrDefaultAsync(m => m.Id == id);
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

        _repository.Update(existing);
        await _repository.SaveChangesAsync();

        if (newPosterPath is not null)
        {
            _posterStorage.Delete(previousPosterPath);
        }

        _logger.LogInformation("Оновлено фільм {Title} (Id = {Id})", existing.Title, existing.Id);

        return existing;
    }

    public async Task<Movie?> DeleteAsync(int id)
    {
        var movie = await _repository.FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        {
            return null;
        }

        _repository.Remove(movie);
        await _repository.SaveChangesAsync();

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

        return !await _repository.AnyAsync(m => m.Id != excludeId && m.Title == normalized);
    }

    public bool IsTitleUnique(string? title, int excludeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return true;
        }

        var normalized = title.Trim();

        return !_repository.Any(m => m.Id != excludeId && m.Title == normalized);
    }
}
