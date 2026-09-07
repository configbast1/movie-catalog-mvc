using Microsoft.EntityFrameworkCore;
using WebApplication23333.Data;
using WebApplication23333.Services;

namespace WebApplication23333.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPosterStorage, PosterStorage>();
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
