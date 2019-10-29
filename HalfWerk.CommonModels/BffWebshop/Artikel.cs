using System;
using System.Collections.Generic;

namespace HalfWerk.CommonModels.BffWebshop
{
    public class Artikel
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
        public IList<string> Categorieen { get; set; }
        public int Voorraad { get; set; }

        public Artikel()
        {
            Categorieen = new List<string>();
        }
    }
}
