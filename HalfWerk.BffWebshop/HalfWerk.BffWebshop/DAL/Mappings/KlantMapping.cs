using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class KlantMapping
    {
        public static void Map(EntityTypeBuilder<Klant> entity)
        {
            entity.ToTable("Klanten");

            entity.Property(k => k.Email)
                .HasMaxLength(255);

            entity.Property(k => k.Voornaam)
                .HasMaxLength(200);

            entity.Property(k => k.Achternaam)
                .HasMaxLength(200);

            entity.Property(k => k.Telefoonnummer)
                .HasMaxLength(25);

            entity.HasOne(k => k.Adres);
        }
    }
}
