using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olympus.Core.Entities;
using Olympus.Data.Identity;

namespace Olympus.Data.Configurations;

public class SmiertelnikConfiguration : IEntityTypeConfiguration<Smiertelnik>
{
    public void Configure(EntityTypeBuilder<Smiertelnik> builder)
    {
        builder.Property(s => s.Imie).IsRequired().HasMaxLength(100);

        // Powiązanie z kontem użytkownika (1 konto = max 1 śmiertelnik)
        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.UserId).IsUnique();
    }
}
