using HalfWerk.CommonModels.DsKlantBeheer.Models;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.DsKlantBeheer.DAL
{
    public class KlantContext : DbContext
    {
        public DbSet<Klant> Klanten { get; set; }

        public KlantContext(DbContextOptions<KlantContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Klant>(klant =>
            {
                klant.ToTable("Klanten");

                klant.HasIndex(k => k.Email)
                    .IsUnique()
                    .HasFilter(null);

                klant.Property(k => k.Email).IsRequired().HasMaxLength(255);

                klant.Property(k => k.Voornaam).HasMaxLength(200);
                klant.Property(k => k.Achternaam).HasMaxLength(200);
                klant.Property(k => k.Telefoonnummer).HasMaxLength(25);
                klant.HasOne(k => k.Adres);
            });

            modelBuilder.Entity<Adres>(adres =>
            {
                adres.ToTable("Adressen");

                adres.HasIndex(a => new { a.Postcode, a.Huisnummer });

                adres.Property(a => a.Postcode).HasMaxLength(18);
                adres.Property(a => a.Huisnummer).HasMaxLength(18);
                adres.Property(a => a.Straatnaam).HasMaxLength(100);
                adres.Property(a => a.Plaats).HasMaxLength(100);
                adres.Property(a => a.Land).HasMaxLength(100);
            });
        }
    }
}
