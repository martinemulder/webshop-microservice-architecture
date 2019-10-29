using HalfWerk.BffWebshop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class CategorieEntityMapping
    {
        public static void Map(EntityTypeBuilder<CategorieEntity> entity)
        {
            entity.ToTable("Categorieen");

            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Id);
        }
    }
}
