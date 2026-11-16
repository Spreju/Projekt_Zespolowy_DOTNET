using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olympus.Core.Entities;
using Olympus.Data.Identity;

namespace Olympus.Data.Configurations;

public class HerosConfiguration : IEntityTypeConfiguration<Heros>
{
    public void Configure(EntityTypeBuilder<Heros> builder)
    {
        builder.Property(h => h.Imie).IsRequired().HasMaxLength(100);

        // Powiązanie z kontem użytkownika (1 konto = max 1 heros)
        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(h => h.UserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(h => h.UserId).IsUnique();
    }
}