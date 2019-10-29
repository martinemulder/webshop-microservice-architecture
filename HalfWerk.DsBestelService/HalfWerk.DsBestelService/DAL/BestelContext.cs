using HalfWerk.CommonModels.DsBestelService.Models;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.DsBestelService.DAL
{
    public class BestelContext : DbContext
    {
        public DbSet<Bestelling> Bestellingen { get; set; }
        public DbSet<Artikel> Artikelen { get; set; }

        public BestelContext(DbContextOptions<BestelContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bestelling>(bestelling =>
            {
                bestelling.ToTable("Bestellingen");

                bestelling.HasIndex(b => b.Factuurnummer)
                    .IsUnique()
                    .HasFilter(null);

                bestelling.Property(b => b.Klantnummer).IsRequired();

                bestelling.HasOne(m => m.ContactInfo);
                bestelling.HasOne(m => m.Afleveradres);
                bestelling.HasMany(m => m.BestelRegels);

                bestelling.Ignore(b => b.FactuurTotaalExclBtw);
                bestelling.Ignore(b => b.FactuurTotaalInclBtw);
            });

            modelBuilder.Entity<BestelRegel>(regel =>
            {
                regel.ToTable("BestelRegels");

                regel.Property(r => r.Artikelnummer)
                    .IsRequired();

                regel.Property(r => r.LeverancierCode)
                    .HasMaxLength(100)
                    .IsRequired();

                regel.Property(r => r.Naam)
                    .HasMaxLength(300)
                    .IsRequired();

                regel.Property(r => r.PrijsExclBtw)
                    .HasColumnType("decimal(19, 2)")
                    .IsRequired();

                regel.Ignore(r => r.PrijsInclBtw);
                regel.Ignore(r => r.RegelTotaalExclBtw);
                regel.Ignore(r => r.RegelTotaalInclBtw);
            });

            modelBuilder.Entity<Artikel>(artikel =>
            {
                artikel.ToTable("Artikelen");

                artikel.HasKey(e => e.Artikelnummer);
                artikel.HasIndex(e => e.Artikelnummer);

                artikel.Property(x => x.Artikelnummer)
                    .ValueGeneratedNever();

                artikel.Property(e => e.Naam)
                    .HasMaxLength(200)
                    .IsRequired();

                artikel.Property(e => e.Prijs)
                    .HasColumnType("decimal(19, 2)")
                    .IsRequired();

                artikel.Property(e => e.Leveranciercode)
                    .HasMaxLength(20).IsRequired();
            });

            modelBuilder.Entity<Afleveradres>(adres =>
            {
                adres.ToTable("Afleveradressen");

                adres.Property(a => a.Adres)
                    .HasMaxLength(120)
                    .IsRequired();

                adres.Property(a => a.Postcode)
                    .HasMaxLength(18)
                    .IsRequired();

                adres.Property(a => a.Plaats)
                    .HasMaxLength(100)
                    .IsRequired();

                adres.Property(a => a.Land)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ContactInfo>(contact =>
            {
                contact.ToTable("ContactInfo");

                contact.Property(c => c.Naam)
                    .HasMaxLength(405)
                    .IsRequired();

                contact.Property(c => c.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                contact.Property(c => c.Telefoonnummer)
                    .HasMaxLength(25)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
