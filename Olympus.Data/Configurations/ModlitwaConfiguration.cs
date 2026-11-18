using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olympus.Core.Entities;

namespace Olympus.Data.Configurations;

public class ModlitwaConfiguration : IEntityTypeConfiguration<Modlitwa>
{
    public void Configure(EntityTypeBuilder<Modlitwa> builder)
    {
        builder.Property(m => m.Tresc).IsRequired().HasMaxLength(2000);
        builder.Property(m => m.Ofiara).HasMaxLength(200);
        builder.Property(m => m.OdpowiedzBoga).HasMaxLength(1000);

        // Status zapisywany jako tekst (czytelniejszy w bazie niż liczba)
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);

        builder.Property(m => m.ZalacznikNazwaPliku).HasMaxLength(100);
        builder.Property(m => m.ZalacznikNazwaOryginalna).HasMaxLength(200);
        builder.Property(m => m.ZalacznikTyp).HasMaxLength(50);

        // OneToMany: Śmiertelnik -> Modlitwy
        builder.HasOne(m => m.Smiertelnik)
               .WithMany(s => s.Modlitwy)
               .HasForeignKey(m => m.SmiertelnikId)
               .OnDelete(DeleteBehavior.Cascade);

        // OneToMany: Bóg -> Modlitwy (nie kasujemy boga, który ma modlitwy)
        builder.HasOne(m => m.Bog)
               .WithMany(b => b.Modlitwy)
               .HasForeignKey(m => m.BogId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.BogId, m.Status });
    }
}
