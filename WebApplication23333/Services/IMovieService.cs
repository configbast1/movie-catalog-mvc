using WebApplication23333.Models;

namespace WebApplication23333.Services;

public interface IMovieService
{
    Task<IReadOnlyList<Movie>> GetAllAsync();

    Task<Movie?> GetByIdAsync(int? id);

    Task<bool> ExistsAsync(int id);

    Task<Movie> CreateAsync(Movie movie);

    Task<Movie?> UpdateAsync(int id, Movie movie);

    Task<Movie?> DeleteAsync(int id);

    Task<bool> IsTitleUniqueAsync(string? title, int excludeId);

    bool IsTitleUnique(string? title, int excludeId);

    string PlaceholderPosterPath { get; }
}
