using HalfWerk.CommonModels.DsKlantBeheer.Models;

namespace HalfWerk.DsKlantBeheer.Test.Entities
{
    public static class KlantExtension
    {
        public static bool IsEqual(this Klant entity, Klant comparable)
        {
            if (entity.GetHashCode() != comparable.GetHashCode()) return false;

            if (entity.Id != comparable.Id) return false;
            if (entity.Voornaam != comparable.Voornaam) return false;
            if (entity.Achternaam != comparable.Achternaam) return false;
            if (entity.Telefoonnummer != comparable.Telefoonnummer) return false;
            if (entity.Email != comparable.Email) return false;
            if (!entity.Adres.IsEqual(comparable.Adres)) return false;

            return true;
        }
    }
}
