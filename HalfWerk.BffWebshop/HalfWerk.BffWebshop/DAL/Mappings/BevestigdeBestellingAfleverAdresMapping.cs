using HalfWerk.CommonModels.BffWebshop.BestellingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class BevestigdeBestellingAfleveradresMapping
    {
        public static void Map(EntityTypeBuilder<BevestigdeBestellingAfleveradres> adres)
        {
            adres.ToTable("BevestigdeBestellingAfleveradressen");

            adres.Property(a => a.Adres)
                .HasMaxLength(120);

            adres.Property(a => a.Postcode)
                .HasMaxLength(18);

            adres.Property(a => a.Plaats)
                .HasMaxLength(100);

            adres.Property(a => a.Land);
        }
    }
}