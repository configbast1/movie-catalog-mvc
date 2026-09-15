using Kino.BLL.Interfaces;
using Kino.BLL.Services;
using Kino.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kino.BLL;

public static class DependencyInjection
{
    // BLL сам підключає DAL: рівень представлення про нього навіть не згадує
    public static IServiceCollection AddBusinessLogicLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataAccessLayer(configuration);

        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
