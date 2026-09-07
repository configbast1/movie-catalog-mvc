namespace WebApplication23333.Services;

public interface IPosterStorage
{
    string PlaceholderPath { get; }

    Task<string?> SaveAsync(IFormFile? file);

    void Delete(string? posterPath);
}
