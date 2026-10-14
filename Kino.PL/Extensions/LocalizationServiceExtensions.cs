using System.Globalization;
using Kino.PL.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kino.PL.Extensions;

public static class LocalizationServiceExtensions
{
    /// <summary>
    /// Підключає локалізацію інтерфейсу: JSON-переклади з папки Resources,
    /// локалізацію Razor-в'юх і повідомлень валідації DataAnnotations.
    /// </summary>
    public static IServiceCollection AddApplicationLocalization(this IServiceCollection services)
    {
        // 1. папка з файлами перекладів
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        // 2. замість .resx використовуємо власну JSON-фабрику
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        // 3. MVC: IViewLocalizer / IHtmlLocalizer у в'юхах + переклад атрибутів валідації.
        //    усі атрибути шукають ключі в одному файлі SharedResource.{culture}.json
        services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource));
            });

        // 4. стандартні повідомлення model binding ("The value '' is invalid." тощо)
        services.AddSingleton<IConfigureOptions<MvcOptions>, LocalizedModelBindingMessages>();

        // 5. підтримувані мови
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var uiCultures = AppCultures.Codes.Select(code => new CultureInfo(code)).ToList();

            // Culture (формат чисел і дат) лишається інваріантною — так поле "Оцінка"
            // однаково приймає 8.5 на будь-якій мові. Змінюється лише UICulture — мова текстів.
            options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture, new CultureInfo(AppCultures.Default));
            options.SupportedCultures = new List<CultureInfo> { CultureInfo.InvariantCulture };
            options.SupportedUICultures = uiCultures;

            // порядок визначення мови: ?culture=en-US → cookie → заголовок Accept-Language браузера
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });

        return services;
    }

    private sealed class LocalizedModelBindingMessages : IConfigureOptions<MvcOptions>
    {
        private readonly IStringLocalizerFactory _factory;

        public LocalizedModelBindingMessages(IStringLocalizerFactory factory) => _factory = factory;

        public void Configure(MvcOptions options)
        {
            var l = _factory.Create(typeof(SharedResource));
            var messages = options.ModelBindingMessageProvider;

            messages.SetValueMustNotBeNullAccessor(field => l["Validation.ValueMustNotBeNull", field]);
            messages.SetValueIsInvalidAccessor(value => l["Validation.ValueIsInvalid", value]);
            messages.SetAttemptedValueIsInvalidAccessor((value, field) => l["Validation.AttemptedValueIsInvalid", value, field]);
            messages.SetNonPropertyAttemptedValueIsInvalidAccessor(value => l["Validation.ValueIsInvalid", value]);
            messages.SetUnknownValueIsInvalidAccessor(field => l["Validation.ValueMustNotBeNull", field]);
            messages.SetValueMustBeANumberAccessor(field => l["Validation.MustBeNumber", field]);
            messages.SetNonPropertyValueMustBeANumberAccessor(() => l["Validation.MustBeNumber", "?"]);
        }
    }
}
