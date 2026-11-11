
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Olympus.Data.Data;
using Olympus.Data.Identity;

var builder = WebApplication.CreateBuilder(args);

// Database configuration - PostgreSQL (Supabase)
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ASP.NET Core Identity configuration
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Custom password and account settings
builder.Services.AddIdentitySettings();

// MVC configuration
builder.Services.AddControllersWithViews();

var app = builder.Build();

// HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

// Authentication must run before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
