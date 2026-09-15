using Kino.BLL.Interfaces;

namespace Kino.PL.Services;

// реалізація сховища постерів живе тут, бо тільки рівень представлення знає про wwwroot
public class PosterStorage : IPosterStorage
{
    private const string UploadsFolder = "/images/uploads/";
    private const string PlaceholderPoster = "/images/poster-placeholder.svg";

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PosterStorage> _logger;

    public PosterStorage(IWebHostEnvironment environment, ILogger<PosterStorage> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public string PlaceholderPath => PlaceholderPoster;

    public async Task<string?> SaveAsync(Stream? content, string? originalFileName)
    {
        if (content is null)
        {
            return null;
        }

        var extension = Path.GetExtension(originalFileName ?? string.Empty).ToLowerInvariant();
        var uploadsDirectory = Path.Combine(WebRoot, "images", "uploads");
        Directory.CreateDirectory(uploadsDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDirectory, fileName);

        await using (content)
        await using (var stream = File.Create(fullPath))
        {
            await content.CopyToAsync(stream);
        }

        _logger.LogInformation("Збережено постер {FileName}", fileName);

        return $"{UploadsFolder}{fileName}";
    }

    public void Delete(string? posterPath)
    {
        if (string.IsNullOrWhiteSpace(posterPath))
        {
            return;
        }

        // видаляємо тільки те, що завантажив користувач; початкові постери не чіпаємо
        if (!posterPath.StartsWith(UploadsFolder, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var relative = posterPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(WebRoot, relative);

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Видалено постер {PosterPath}", posterPath);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Не вдалося видалити постер {PosterPath}", posterPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Немає прав на видалення постера {PosterPath}", posterPath);
        }
    }

    private string WebRoot =>
        _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
}
