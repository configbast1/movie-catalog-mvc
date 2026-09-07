using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WebApplication23333.Data;

namespace WebApplication23333.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        return app;
    }

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
