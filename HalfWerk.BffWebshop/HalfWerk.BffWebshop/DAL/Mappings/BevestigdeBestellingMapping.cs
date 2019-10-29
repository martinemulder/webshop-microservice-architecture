using HalfWerk.CommonModels.BffWebshop.BestellingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class BevestigdeBestellingMapping
    {
        public static void Map(EntityTypeBuilder<BevestigdeBestelling> bestelling)
        {
            bestelling.ToTable("BevestigdeBestellingen");

            bestelling.HasAlternateKey(b => b.Factuurnummer);

            bestelling.Property(b => b.Besteldatum).IsRequired();
            bestelling.Property(b => b.FactuurTotaalExclBtw).HasColumnType("decimal(19, 2)");
            bestelling.Property(b => b.FactuurTotaalInclBtw).HasColumnType("decimal(19, 2)");
        }
    }
}
