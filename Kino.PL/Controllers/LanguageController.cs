using Kino.PL.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Kino.PL.Controllers;

// перемикач мови: запам'ятовує вибір у cookie і повертає користувача на ту саму сторінку
public class LanguageController : Controller
{
    [HttpGet]
    public IActionResult Set(string culture, string? returnUrl)
    {
        if (AppCultures.All.Any(c => c.Code == culture))
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });
        }

        // LocalRedirect захищає від open redirect на сторонні сайти
        return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }
}
