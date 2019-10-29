using HalfWerk.BffWebshop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class ArtikelEntityMapping
    {
        public static void Map(EntityTypeBuilder<ArtikelEntity> entity)
        {
            entity.ToTable("Artikelen");

            entity.HasKey(e => e.Artikelnummer);
            entity.HasIndex(e => e.Artikelnummer);

            entity.Property(x => x.Artikelnummer)
                .ValueGeneratedNever();

            entity.Property(e => e.Naam).HasMaxLength(200).IsRequired();
            entity.Property(e => e.AfbeeldingUrl).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.Beschrijving).HasMaxLength(4000).IsRequired();
            entity.Property(e => e.Prijs).HasColumnType("decimal(19, 2)").IsRequired();
            entity.Property(e => e.LeverbaarVanaf).IsRequired();
            entity.Property(e => e.Leverancier).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Leveranciercode).HasMaxLength(20).IsRequired();
        }
    }
}
