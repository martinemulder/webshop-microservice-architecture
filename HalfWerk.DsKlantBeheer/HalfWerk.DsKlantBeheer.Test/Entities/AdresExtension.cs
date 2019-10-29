using HalfWerk.CommonModels.DsKlantBeheer.Models;

namespace HalfWerk.DsKlantBeheer.Test.Entities
{
    public static class AdresExtension
    {
        public static bool IsEqual(this Adres entity, Adres comparable)
        {
            if (entity.GetHashCode() != comparable.GetHashCode()) return false;

            if (entity.Id != comparable.Id) return false;
            if (entity.Straatnaam != comparable.Straatnaam) return false;
            if (entity.Postcode != comparable.Postcode) return false;
            if (entity.Huisnummer != comparable.Huisnummer) return false;
            if (entity.Plaats != comparable.Plaats) return false;
            if (entity.Land != comparable.Land) return false;

            return true;
        }
    }
}
