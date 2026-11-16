using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olympus.Core.Entities;

namespace Olympus.Data.Configurations;

public class MisjaConfiguration : IEntityTypeConfiguration<Misja>
{
    public void Configure(EntityTypeBuilder<Misja> builder)
    {
        builder.Property(m => m.Tytul).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Opis).IsRequired().HasMaxLength(2000);

        // Status zapisywany jako tekst (czytelniejszy w bazie niż liczba)
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);

        // OneToMany: Bóg -> Misje (nie kasujemy boga, który ma misje)
        builder.HasOne(m => m.Bog)
               .WithMany(b => b.Misje)
               .HasForeignKey(m => m.BogId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}