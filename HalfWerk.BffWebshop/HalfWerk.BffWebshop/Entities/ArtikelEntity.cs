using HalfWerk.CommonModels.BffWebshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Entities
{
    public class ArtikelEntity : IEntity<long>
    {
        public long Artikelnummer { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public decimal Prijs { get; set; }
        public string AfbeeldingUrl { get; set; }
        public DateTime LeverbaarVanaf { get; set; }
        public DateTime? LeverbaarTot { get; set; }
        public string Leveranciercode { get; set; }
        public string Leverancier { get; set; }
        public int Voorrraad { get; set; }

        public virtual ICollection<ArtikelCategorieEntity> ArtikelCategorieen { get; set; }

        public ArtikelEntity()
        {
            ArtikelCategorieen = new HashSet<ArtikelCategorieEntity>();
        }

        public Artikel ToArtikel()
        {
            var artikel = new Artikel()
            {
                Artikelnummer = Artikelnummer,
                Naam = Naam,
                Beschrijving = Beschrijving,
                Prijs = Prijs,
                AfbeeldingUrl = AfbeeldingUrl,
                LeverbaarVanaf = LeverbaarVanaf,
                LeverbaarTot = LeverbaarTot,
                Leveranciercode = Leveranciercode,
                Leverancier = Leverancier,
                Voorraad = Voorrraad,
            };

            foreach(var ac in ArtikelCategorieen)
            {
                artikel.Categorieen.Add(ac.Categorie.Categorie);
            }

            return artikel;
        }

        public long GetKeyValue()
        {
            return Artikelnummer;
        }
    }
}
