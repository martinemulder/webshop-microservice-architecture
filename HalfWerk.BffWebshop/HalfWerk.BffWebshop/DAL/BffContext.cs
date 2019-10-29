using HalfWerk.BffWebshop.DAL.Mappings;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.BffWebshop.DAL
{
    public class BffContext : DbContext
    {
        public DbSet<ArtikelEntity> ArtikelEntities { get; set; }
        public DbSet<CategorieEntity> CategorieEntities { get; set; }
        public DbSet<ArtikelCategorieEntity> ArtikelCategorieEntities { get; set; }
        public DbSet<Klant> KlantEntities { get; set; }
        public DbSet<BevestigdeBestelling> BevestigdeBestellingen { get; set; }
        public DbSet<MagazijnSessionEntity> MagazijnSessions { get; set; } 

        public BffContext(DbContextOptions<BffContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtikelEntity>(ArtikelEntityMapping.Map);
            modelBuilder.Entity<CategorieEntity>(CategorieEntityMapping.Map);
            modelBuilder.Entity<ArtikelCategorieEntity>(ArtikelCategorieEntityMapping.Map);

            modelBuilder.Entity<BevestigdeBestelling>(BevestigdeBestellingMapping.Map);
            modelBuilder.Entity<BevestigdeBestelRegel>(BevestigdeBestelRegelMapping.Map);
            modelBuilder.Entity<BevestigdeBestellingContactInfo>(BevestigdeBestellingContactInfoMapping.Map);
            modelBuilder.Entity<BevestigdeBestellingAfleveradres>(BevestigdeBestellingAfleveradresMapping.Map);

            modelBuilder.Entity<Klant>(KlantMapping.Map);
            modelBuilder.Entity<Adres>(AdresMapping.Map);

            modelBuilder.Entity<MagazijnSessionEntity>(MagazijnSessionEntityMapping.Map);

            base.OnModelCreating(modelBuilder);
        }
    }
}
