using HalfWerk.BffWebshop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class ArtikelCategorieEntityMapping
    {
        public static void Map(EntityTypeBuilder<ArtikelCategorieEntity> entity)
        {
            entity.ToTable("ArtikelCategorie");

            entity.HasKey(x => new { x.ArtikelId, x.CategorieId });
            
            entity.HasOne(x => x.Artikel)
                .WithMany(x => x.ArtikelCategorieen)
                .HasForeignKey(x => x.ArtikelId);

            entity.HasOne(x => x.Categorie)
                .WithMany(x => x.ArtikelCategorieen)
                .HasForeignKey(x => x.CategorieId);
        }
    }
}
