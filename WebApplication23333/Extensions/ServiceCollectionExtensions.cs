using Microsoft.EntityFrameworkCore;
using WebApplication23333.Data;
using WebApplication23333.Services;

namespace WebApplication23333.Extensions;

/// <summary>
/// Extension-методи для IServiceCollection: усі реєстрації в DI-контейнері зібрані тут,
/// тому Program.cs залишається коротким і читабельним.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Реєструє контекст бази даних із рядком підключення DefaultConnection.</summary>
    public static IServiceCollection AddApplicationDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    /// <summary>Реєструє сервіси прикладного рівня (бізнес-логіка + робота з файлами).</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Scoped — час життя збігається з HTTP-запитом, як і в AppDbContext
        services.AddScoped<IPosterStorage, PosterStorage>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
