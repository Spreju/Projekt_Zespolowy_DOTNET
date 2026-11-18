using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Olympus.Core.Entities;
using Olympus.Data.Identity;

namespace Olympus.Data.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Bog> Bogowie => Set<Bog>();
    public DbSet<Heros> Herosi => Set<Heros>();
    public DbSet<Misja> Misje => Set<Misja>();
    public DbSet<MisjaHeros> MisjeHerosow => Set<MisjaHeros>();
    public DbSet<Smiertelnik> Smiertelnicy => Set<Smiertelnik>();
    public DbSet<Modlitwa> Modlitwy => Set<Modlitwa>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // konieczne dla tabel Identity

        // Automatycznie wczytuje wszystkie klasy *Configuration z tego projektu
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}