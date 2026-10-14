namespace Kino.PL.Localization;

/// <summary>
/// Мови, які підтримує сайт. Щоб додати нову мову, достатньо дописати її сюди
/// і покласти поруч файл Resources/SharedResource.{код}.json.
/// </summary>
public static class AppCultures
{
    public const string Default = "uk-UA";

    public static readonly IReadOnlyList<AppCulture> All = new[]
    {
        new AppCulture("uk-UA", "UA", "Українська"),
        new AppCulture("en-US", "EN", "English"),
        new AppCulture("fr-FR", "FR", "Français")
    };

    public static string[] Codes => All.Select(c => c.Code).ToArray();
}

public record AppCulture(string Code, string ShortName, string NativeName);
