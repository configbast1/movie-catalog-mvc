using Kino.BLL.DTO;
using Kino.BLL.Interfaces;
using Kino.BLL.Mapping;
using Kino.DAL.Entities;
using Kino.DAL.Repositories;
using Microsoft.Extensions.Logging;

namespace Kino.BLL.Services;

// вся бізнес-логіка каталогу: правила, робота з постерами, логування.
// контролер сюди передає DTO і отримує DTO назад
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

    public async Task<IReadOnlyList<MovieDto>> GetAllAsync()
    {
        var movies = await _repository.ListAsync(
            orderBy: query => query
                .OrderByDescending(m => m.Rating)
                .ThenBy(m => m.Title));

        return movies.Select(m => m.ToDto()).ToList();
    }

    public async Task<MovieDto?> GetByIdAsync(int? id)
    {
        if (id is null)
        {
            return null;
        }

        var movie = await _repository.FirstOrDefaultAsync(m => m.Id == id, asNoTracking: true);

        return movie?.ToDto();
    }

    public Task<bool> ExistsAsync(int id) => _repository.AnyAsync(m => m.Id == id);

    public async Task<MovieDto> CreateAsync(MovieDto movie)
    {
        var posterPath = await _posterStorage.SaveAsync(movie.PosterContent, movie.PosterFileName);

        var entity = movie.ToEntity();
        entity.Id = 0;
        entity.PosterPath = posterPath ?? _posterStorage.PlaceholderPath;

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Додано фільм {Title} (Id = {Id})", entity.Title, entity.Id);

        return entity.ToDto();
    }

    public async Task<MovieDto?> UpdateAsync(int id, MovieDto movie)
    {
        var existing = await _repository.FirstOrDefaultAsync(m => m.Id == id);
        if (existing is null)
        {
            return null;
        }

        var newPosterPath = await _posterStorage.SaveAsync(movie.PosterContent, movie.PosterFileName);
        var previousPosterPath = existing.PosterPath;

        movie.CopyEditableFields(existing);

        if (newPosterPath is not null)
        {
            existing.PosterPath = newPosterPath;
        }

        _repository.Update(existing);
        await _repository.SaveChangesAsync();

        // старий файл видаляємо лише після успішного збереження
        if (newPosterPath is not null)
        {
            _posterStorage.Delete(previousPosterPath);
        }

        _logger.LogInformation("Оновлено фільм {Title} (Id = {Id})", existing.Title, existing.Id);

        return existing.ToDto();
    }

    public async Task<MovieDto?> DeleteAsync(int id)
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

        return movie.ToDto();
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
