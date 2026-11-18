using Olympus.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Olympus.Data.Data;
using Olympus.Core.Interfaces;
using Olympus.Data.Identity;
using Olympus.Data.Services;
using Olympus.Web.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddIdentitySettings();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


// Serwisy aplikacji (Dependency Injection)
builder.Services.AddSingleton<IPlikiService, LocalPlikiService>();
builder.Services.AddScoped<IModlitwaService, ModlitwaService>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IPasswordResetEmailSender,SmtpPasswordResetEmailSender>();

var app = builder.Build();

// Role oraz bóstwa startowe (operacje idempotentne)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeeder.SeedRoleAsync(roleManager);

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await OlympusDataSeeder.SeedBogowieAsync(db);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex,
            "Nie udało się zasiać danych startowych. Czy zastosowano migracje (dotnet ef database update)?");
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();

app.Run();
