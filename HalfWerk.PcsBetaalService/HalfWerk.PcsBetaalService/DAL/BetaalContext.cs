using HalfWerk.CommonModels.PcsBetalingService.Models;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.PcsBetaalService.DAL
{
    public class BetaalContext : DbContext
    {
        public DbSet<Betaling> Betalingen { get; set; }
        public DbSet<Bestelling> Bestellingen { get; set; }
        public BetaalContext(DbContextOptions<BetaalContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Betaling>(betaling =>
            {
                betaling.ToTable("Betalingen");

                betaling.Property(b => b.Bedrag)
                    .IsRequired();

                betaling.Property(b => b.Factuurnummer)
                    .IsRequired();

                 betaling.Property(b => b.BetaalDatum)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Bestelling>(bestelling =>
            {
                bestelling.ToTable("Bestellingen");

                bestelling.HasIndex(b => b.Factuurnummer)
                    .IsUnique()
                    .HasFilter(null);

                bestelling.Property(b => b.Klantnummer).IsRequired();
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
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}