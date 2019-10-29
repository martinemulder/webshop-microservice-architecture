using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class AdresMapping
    {
        public static void Map(EntityTypeBuilder<Adres> entity)
        {
            entity.ToTable("Adressen");

            entity.Property(a => a.Postcode)
                .HasMaxLength(18);

            entity.Property(a => a.Huisnummer)
                .HasMaxLength(18);

            entity.Property(a => a.Straatnaam)
                .HasMaxLength(100);

            entity.Property(a => a.Plaats)
                .HasMaxLength(100);

            entity.Property(a => a.Land)
                .HasMaxLength(100);
        }
    }
}
