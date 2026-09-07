using WebApplication23333.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApplicationDatabase(builder.Configuration);
builder.Services.AddApplicationServices();

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
