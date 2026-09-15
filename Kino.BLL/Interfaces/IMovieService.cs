using Kino.BLL.DTO;

namespace Kino.BLL.Interfaces;

public interface IMovieService
{
    Task<IReadOnlyList<MovieDto>> GetAllAsync();

    Task<MovieDto?> GetByIdAsync(int? id);

    Task<bool> ExistsAsync(int id);

    Task<MovieDto> CreateAsync(MovieDto movie);

    Task<MovieDto?> UpdateAsync(int id, MovieDto movie);

    Task<MovieDto?> DeleteAsync(int id);

    Task<bool> IsTitleUniqueAsync(string? title, int excludeId);

    bool IsTitleUnique(string? title, int excludeId);

    string PlaceholderPosterPath { get; }
}
