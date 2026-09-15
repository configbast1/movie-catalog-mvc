using Kino.BLL.DTO;

namespace Kino.PL.ViewModels;

// перетворення між моделлю форми та DTO бізнес-рівня
public static class MovieViewModelMapper
{
    public static MovieViewModel ToViewModel(this MovieDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        OriginalTitle = dto.OriginalTitle,
        Director = dto.Director,
        Genre = dto.Genre,
        ReleaseYear = dto.ReleaseYear,
        PosterPath = dto.PosterPath,
        Description = dto.Description,
        Rating = dto.Rating
    };

    public static MovieDto ToDto(this MovieViewModel model) => new()
    {
        Id = model.Id,
        Title = model.Title,
        OriginalTitle = model.OriginalTitle,
        Director = model.Director,
        Genre = model.Genre,
        ReleaseYear = model.ReleaseYear,
        PosterPath = model.PosterPath,
        Description = model.Description,
        Rating = model.Rating,

        // IFormFile далі рівня представлення не йде - передаємо лише потік і ім'я файлу
        PosterContent = model.PosterFile is { Length: > 0 } file ? file.OpenReadStream() : null,
        PosterFileName = model.PosterFile?.FileName
    };
}
