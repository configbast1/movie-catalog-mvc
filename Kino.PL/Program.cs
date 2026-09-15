using Kino.BLL;
using Kino.BLL.Interfaces;
using Kino.PL.Extensions;
using Kino.PL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// один виклик підключає весь бізнес-рівень, а він усередині підключає рівень даних
builder.Services.AddBusinessLogicLayer(builder.Configuration);

// сховище постерів реалізоване тут, бо залежить від wwwroot
builder.Services.AddScoped<IPosterStorage, PosterStorage>();

var app = builder.Build();

app.EnsureDatabaseCreated();
app.UseApplicationLocalization();
app.UseApplicationErrorHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();
