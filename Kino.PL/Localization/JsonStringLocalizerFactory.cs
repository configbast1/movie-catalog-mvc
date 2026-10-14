using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kino.PL.Localization;

/// <summary>
/// Фабрика повністю замінює стандартну ResourceManagerStringLocalizerFactory (.resx).
/// Після її реєстрації в DI IStringLocalizer, IHtmlLocalizer і локалізація
/// DataAnnotations працюють з нашими JSON-файлами.
/// </summary>
public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly string _resourcesPath;

    public JsonStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions, IHostEnvironment env)
    {
        _resourcesPath = Path.Combine(env.ContentRootPath, localizationOptions.Value.ResourcesPath ?? "Resources");
    }

    public IStringLocalizer Create(Type resourceSource)
        => new JsonStringLocalizer(_resourcesPath, resourceSource.Name, AppCultures.Default);

    public IStringLocalizer Create(string baseName, string location)
        => new JsonStringLocalizer(_resourcesPath, baseName.Split('.').Last(), AppCultures.Default);
}
