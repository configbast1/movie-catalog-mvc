namespace Kino.BLL.DTO;

// об'єкт передачі даних між BLL та рівнем представлення.
// сутність Movie з DAL нагору не піднімається взагалі
public class MovieDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? OriginalTitle { get; set; }

    public string Director { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public int ReleaseYear { get; set; }

    public string PosterPath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Rating { get; set; }

    // вміст завантаженого постера.
    // саме Stream, а не IFormFile - BLL не посилається на ASP.NET Core
    public Stream? PosterContent { get; set; }

    public string? PosterFileName { get; set; }
}
