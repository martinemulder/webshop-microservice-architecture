using System.Linq;
using HalfWerk.BffWebshop.Entities;

namespace HalfWerk.BffWebshop.Test.Entities.Artikel
{
    public static class ArtikelEntityExtension
    {
        public static bool IsEqual(this ArtikelEntity entity, ArtikelEntity comparable)
        {
            if (entity.Artikelnummer != comparable.Artikelnummer) return false;
            if (entity.Naam != comparable.Naam) return false;
            if (entity.Beschrijving != comparable.Beschrijving) return false;
            if (entity.Prijs != comparable.Prijs) return false;
            if (entity.AfbeeldingUrl != comparable.AfbeeldingUrl) return false;
            if (entity.LeverbaarVanaf != comparable.LeverbaarVanaf) return false;
            if (entity.LeverbaarTot != comparable.LeverbaarTot) return false;
            if (entity.Leveranciercode != comparable.Leveranciercode) return false;
            if (entity.Leverancier != comparable.Leverancier) return false;
            if (entity.Voorrraad != comparable.Voorrraad) return false;
            if (entity.ArtikelCategorieen.Count != comparable.ArtikelCategorieen.Count) return false;

            var entityCatList = entity.ArtikelCategorieen.OrderBy(x => x.Categorie.Categorie);
            var comparableCatList = comparable.ArtikelCategorieen.OrderBy(x => x.Categorie.Categorie);

            for (int i = 0; i < entityCatList.Count(); ++i)
            {
                if (entityCatList.ElementAt(i)?.Categorie.Categorie !=
                        comparableCatList.ElementAt(i)?.Categorie.Categorie) return false;
            }

            return true;
        }
    }
}
