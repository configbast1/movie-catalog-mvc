using Kino.BLL.DTO;
using Kino.DAL.Entities;

namespace Kino.BLL.Mapping;

// ручний маппінг сутність <-> DTO, без сторонніх бібліотек
public static class MovieMapper
{
    public static MovieDto ToDto(this Movie entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        OriginalTitle = entity.OriginalTitle,
        Director = entity.Director,
        Genre = entity.Genre,
        ReleaseYear = entity.ReleaseYear,
        PosterPath = entity.PosterPath,
        Description = entity.Description,
        Rating = entity.Rating
    };

    public static Movie ToEntity(this MovieDto dto) => new()
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

    // переносимо у вже існуючу сутність лише ті поля, які можна редагувати
    public static void CopyEditableFields(this MovieDto dto, Movie entity)
    {
        entity.Title = dto.Title;
        entity.OriginalTitle = dto.OriginalTitle;
        entity.Director = dto.Director;
        entity.Genre = dto.Genre;
        entity.ReleaseYear = dto.ReleaseYear;
        entity.Description = dto.Description;
        entity.Rating = dto.Rating;
    }
}
