using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using HalfWerk.CommonModels.BffWebshop.BestellingService;

namespace HalfWerk.BffWebshop.Test.Entities.Bestellingen
{
    public static class BevestigdeBestellingExtension
    {
        public static bool IsEqual(this BevestigdeBestelling entity, BevestigdeBestelling comparable)
        {
            if (entity.Id != comparable.Id) return false;
            if (entity.Factuurnummer != comparable.Factuurnummer) return false;
            if (entity.Klantnummer != comparable.Klantnummer) return false;
            if (entity.FactuurTotaalExclBtw != comparable.FactuurTotaalExclBtw) return false;
            if (entity.FactuurTotaalInclBtw != comparable.FactuurTotaalInclBtw) return false;
            if (entity.BestelStatus != comparable.BestelStatus) return false;
            if (entity.Besteldatum != comparable.Besteldatum) return false;
            if (entity.BestelRegels.Count != comparable.BestelRegels.Count) return false;
            
            var entityCatList = entity.BestelRegels.OrderBy(x => x.Artikelnummer);
            var comparableCatList = comparable.BestelRegels.OrderBy(x => x.Artikelnummer);

            for (int i = 0; i < entity.BestelRegels.Count; ++i)
            {
                if (entityCatList.ElementAt(i)?.Id !=
                    comparableCatList.ElementAt(i)?.Id) return false;
                if (entityCatList.ElementAt(i)?.Artikelnummer !=
                    comparableCatList.ElementAt(i)?.Artikelnummer) return false;
                if (entityCatList.ElementAt(i)?.Naam !=
                    comparableCatList.ElementAt(i)?.Naam) return false;
                if (entityCatList.ElementAt(i)?.Aantal !=
                    comparableCatList.ElementAt(i)?.Aantal) return false;
                if (entityCatList.ElementAt(i)?.PrijsExclBtw !=
                    comparableCatList.ElementAt(i)?.PrijsExclBtw) return false;
                if (entityCatList.ElementAt(i)?.PrijsInclBtw !=
                    comparableCatList.ElementAt(i)?.PrijsInclBtw) return false;
                if (entityCatList.ElementAt(i)?.RegelTotaalExclBtw !=
                    comparableCatList.ElementAt(i)?.RegelTotaalExclBtw) return false;
                if (entityCatList.ElementAt(i)?.RegelTotaalInclBtw !=
                    comparableCatList.ElementAt(i)?.RegelTotaalInclBtw) return false;
            }

            return true;
        }
    }
}
