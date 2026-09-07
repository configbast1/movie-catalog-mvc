using WebApplication23333.Models;

namespace WebApplication23333.Services;

/// <summary>
/// Бізнес-логіка каталогу фільмів: робота з базою даних та постерами.
/// Контролер працює лише через цей інтерфейс і не знає ні про AppDbContext, ні про файлову систему.
/// </summary>
public interface IMovieService
{
    Task<IReadOnlyList<Movie>> GetAllAsync();

    Task<Movie?> GetByIdAsync(int? id);

    Task<bool> ExistsAsync(int id);

    Task<Movie> CreateAsync(Movie movie);

    /// <summary>Оновлює фільм. Повертає null, якщо фільму з таким Id немає.</summary>
    Task<Movie?> UpdateAsync(int id, Movie movie);

    /// <summary>Видаляє фільм разом із постером. Повертає видалений фільм або null.</summary>
    Task<Movie?> DeleteAsync(int id);

    /// <summary>Перевірка унікальності назви (для [Remote] та кастомного атрибута).</summary>
    Task<bool> IsTitleUniqueAsync(string? title, int excludeId);

    /// <summary>Синхронний варіант — потрібен усередині ValidationAttribute.</summary>
    bool IsTitleUnique(string? title, int excludeId);

    /// <summary>Шлях до постера-заглушки.</summary>
    string PlaceholderPosterPath { get; }
}
