using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WebApplication23333.Data;

namespace WebApplication23333.Extensions;

/// <summary>
/// Extension-методи для WebApplication: налаштування конвеєра обробки запитів.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>Створює базу даних, якщо її ще немає.</summary>
    public static WebApplication EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        return app;
    }

    /// <summary>Інваріантна культура — щоб крапка/кома в оцінці розбиралися однаково.</summary>
    public static WebApplication UseApplicationLocalization(this WebApplication app)
    {
        var invariantCultures = new[] { CultureInfo.InvariantCulture };

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
            SupportedCultures = invariantCultures,
            SupportedUICultures = invariantCultures
        });

        return app;
    }

    /// <summary>Обробка помилок: сторінки статус-кодів та HSTS у продакшені.</summary>
    public static WebApplication UseApplicationErrorHandling(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Home/StatusCodeHandler/{0}");

        return app;
    }
}
