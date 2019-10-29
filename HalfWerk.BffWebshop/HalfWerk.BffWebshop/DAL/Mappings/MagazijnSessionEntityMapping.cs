using HalfWerk.BffWebshop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class MagazijnSessionEntityMapping
    {
        public static void Map(EntityTypeBuilder<MagazijnSessionEntity> entity)
        {
            entity.ToTable("MagazijnSessions");

            entity.HasIndex(e => e.Factuurnummer);

            entity.Property(e => e.Factuurnummer).IsRequired();
            entity.Property(e => e.MedewerkerEmail).HasMaxLength(255).IsRequired();
        }
    }
}
