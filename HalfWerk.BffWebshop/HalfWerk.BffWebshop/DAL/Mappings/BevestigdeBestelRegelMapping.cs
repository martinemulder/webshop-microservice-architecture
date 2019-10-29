using HalfWerk.CommonModels.BffWebshop.BestellingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class BevestigdeBestelRegelMapping
    {
        public static void Map(EntityTypeBuilder<BevestigdeBestelRegel> bestelRegel)
        {
            bestelRegel.ToTable("BevestigdeBestelRegels");

            bestelRegel.Property(br => br.Aantal).IsRequired();
            bestelRegel.Property(br => br.Artikelnummer).IsRequired();
            bestelRegel.Property(br => br.Naam).HasMaxLength(300);
            bestelRegel.Property(br => br.PrijsExclBtw).HasColumnType("decimal(19, 2)");
            bestelRegel.Property(br => br.PrijsInclBtw).HasColumnType("decimal(19, 2)");
            bestelRegel.Property(br => br.RegelTotaalExclBtw).HasColumnType("decimal(19, 2)");
            bestelRegel.Property(br => br.RegelTotaalInclBtw).HasColumnType("decimal(19, 2)");
        }
    }
}
