using HalfWerk.CommonModels.BffWebshop.BestellingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HalfWerk.BffWebshop.DAL.Mappings
{
    internal static class BevestigdeBestellingContactInfoMapping
    {
        public static void Map(EntityTypeBuilder<BevestigdeBestellingContactInfo> contact)
        {
            contact.ToTable("BevestigdeBestellingContactInfo");

            contact.Property(c => c.Naam)
                .HasMaxLength(405);

            contact.Property(c => c.Email)
                .HasMaxLength(255);

            contact.Property(c => c.Telefoonnummer)
                .HasMaxLength(25);
        }
    }
}