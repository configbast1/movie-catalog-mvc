using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Kino.PL.Validation;

// спільні хелпери для власних атрибутів валідації
internal static class ValidationAttributeHelpers
{
    // додає data-атрибут лише якщо його ще немає
    public static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes.Add(key, value);
        }
    }

    // переклад для серверної перевірки: локалізатор береться з DI через ValidationContext
    public static string Localize(this IServiceProvider services, string key, params object[] arguments)
    {
        var localizer = services.GetService(typeof(IStringLocalizer<SharedResource>)) as IStringLocalizer;

        return localizer is null
            ? string.Format(key, arguments)
            : localizer[key, arguments].Value;
    }

    // переклад для клієнтської перевірки (текст потрапляє в data-val-* атрибути)
    public static string Localize(this ClientModelValidationContext context, string key, params object[] arguments)
        => context.ActionContext.HttpContext.RequestServices.Localize(key, arguments);
}
