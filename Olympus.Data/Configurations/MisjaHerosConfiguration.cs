using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Olympus.Core.Entities;

namespace Olympus.Data.Configurations;

public class MisjaHerosConfiguration : IEntityTypeConfiguration<MisjaHeros>
{
    public void Configure(EntityTypeBuilder<MisjaHeros> builder)
    {
        // Klucz złożony: jeden heros może przyjąć daną misję tylko raz
        builder.HasKey(mh => new { mh.MisjaId, mh.HerosId });

        builder.Property(mh => mh.Raport).HasMaxLength(2000);
        builder.Property(mh => mh.Status).HasConversion<string>().HasMaxLength(20);

        // ManyToMany Misja <-> Heros zrealizowane przez dwie relacje OneToMany
        builder.HasOne(mh => mh.Misja)
               .WithMany(m => m.Herosi)
               .HasForeignKey(mh => mh.MisjaId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mh => mh.Heros)
               .WithMany(h => h.Misje)
               .HasForeignKey(mh => mh.HerosId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}