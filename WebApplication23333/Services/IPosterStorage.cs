namespace WebApplication23333.Services;

/// <summary>
/// Сервіс роботи з файлами постерів (файлова система).
/// Контролер про фізичні шляхи нічого не знає — усе тут.
/// </summary>
public interface IPosterStorage
{
    /// <summary>Шлях до постера-заглушки, якщо користувач не завантажив файл.</summary>
    string PlaceholderPath { get; }

    /// <summary>Зберігає завантажений файл у wwwroot і повертає веб-шлях або null.</summary>
    Task<string?> SaveAsync(IFormFile? file);

    /// <summary>Видаляє раніше завантажений постер (заглушку та сідові постери не чіпає).</summary>
    void Delete(string? posterPath);
}
