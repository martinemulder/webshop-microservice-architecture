using System.Linq;
using HalfWerk.CommonModels.DsBestelService.Models;

namespace HalfWerk.DsBestelService.Test.Entities
{
    public static class BestellingExtension
    {
        public static bool IsEqual(this Bestelling entity, Bestelling comparable)
        {
            if (entity.Factuurnummer != comparable.Factuurnummer) return false;
            if (entity.Klantnummer != comparable.Klantnummer) return false;
            if (entity.BestelStatus != comparable.BestelStatus) return false;
            if (entity.Besteldatum != comparable.Besteldatum) return false;
            if (entity.BestelRegels.Count != comparable.BestelRegels.Count) return false;

            var entityCatList = entity.BestelRegels.OrderBy(x => x.Artikelnummer);
            var comparableCatList = comparable.BestelRegels.OrderBy(x => x.Artikelnummer);

            for (int i = 0; i < entity.BestelRegels.Count; ++i)
            {
                if (entity.BestelRegels.ElementAt(i)?.Artikelnummer !=
                    comparable.BestelRegels.ElementAt(i)?.Artikelnummer) return false;
                if (entity.BestelRegels.ElementAt(i)?.Id !=
                    comparable.BestelRegels.ElementAt(i)?.Id) return false;
            }

            return true;
        }
    }
}
