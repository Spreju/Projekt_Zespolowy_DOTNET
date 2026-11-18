using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olympus.Core.Entities;
using Olympus.Data.Identity;

namespace Olympus.Data.Configurations;

public class BogConfiguration : IEntityTypeConfiguration<Bog>
{
    public void Configure(EntityTypeBuilder<Bog> builder)
    {
        // Powiązanie z kontem użytkownika (1 konto = max 1 bóg)
        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(b => b.UserId).IsUnique();
    }
}