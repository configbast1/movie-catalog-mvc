using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace Kino.PL.Localization;

/// <summary>
/// Власна реалізація IStringLocalizer, яка читає переклади з JSON-файлів
/// замість стандартних .resx. Файли мають ім'я {ResourceName}.{culture}.json
/// і лежать у папці Resources, вкладені об'єкти розгортаються в ключі через крапку
/// (наприклад, "Movies.Index.Title").
/// </summary>
public class JsonStringLocalizer : IStringLocalizer
{
    private readonly string _resourcesPath;
    private readonly string _resourceName;
    private readonly string _defaultCulture;

    // файл читається з диска один раз на культуру за весь час життя процесу
    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> Cache = new();

    public JsonStringLocalizer(string resourcesPath, string resourceName, string defaultCulture)
    {
        _resourcesPath = resourcesPath;
        _resourceName = resourceName;
        _defaultCulture = defaultCulture;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = GetString(name);
            var format = value ?? name;
            return new LocalizedString(name, string.Format(CultureInfo.CurrentCulture, format, arguments), value is null);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var resources = GetResourcesForCulture(CultureInfo.CurrentUICulture.Name);

        foreach (var pair in resources)
        {
            yield return new LocalizedString(pair.Key, pair.Value, resourceNotFound: false);
        }
    }

    private string? GetString(string key)
    {
        var culture = CultureInfo.CurrentUICulture.Name;

        if (GetResourcesForCulture(culture).TryGetValue(key, out var value))
        {
            return value;
        }

        // якщо ключа немає в поточній мові — беремо з мови за замовчуванням
        if (culture != _defaultCulture &&
            GetResourcesForCulture(_defaultCulture).TryGetValue(key, out var fallbackValue))
        {
            return fallbackValue;
        }

        return null;
    }

    private IReadOnlyDictionary<string, string> GetResourcesForCulture(string culture)
        => Cache.GetOrAdd($"{_resourcesPath}|{_resourceName}.{culture}", _ => LoadResources(culture));

    private IReadOnlyDictionary<string, string> LoadResources(string culture)
    {
        var result = new Dictionary<string, string>();
        var filePath = Path.Combine(_resourcesPath, $"{_resourceName}.{culture}.json");

        if (!File.Exists(filePath))
        {
            return result;
        }

        using var stream = File.OpenRead(filePath);
        using var document = JsonDocument.Parse(stream);

        Flatten(document.RootElement, string.Empty, result);
        return result;
    }

    private static void Flatten(JsonElement element, string prefix, Dictionary<string, string> result)
    {
        foreach (var property in element.EnumerateObject())
        {
            var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                Flatten(property.Value, key, result);
            }
            else
            {
                result[key] = property.Value.GetString() ?? string.Empty;
            }
        }
    }
}
