namespace Kino.BLL.Interfaces;

// абстракція сховища постерів.
// реалізація живе на рівні представлення, бо саме там є wwwroot
public interface IPosterStorage
{
    string PlaceholderPath { get; }

    Task<string?> SaveAsync(Stream? content, string? originalFileName);

    void Delete(string? posterPath);
}
