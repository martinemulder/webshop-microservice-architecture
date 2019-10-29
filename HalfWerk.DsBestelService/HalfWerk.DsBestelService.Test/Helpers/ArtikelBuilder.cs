using HalfWerk.CommonModels.DsBestelService.Models;
using System;

namespace HalfWerk.DsBestelService.Test.Helpers
{
    public class ArtikelBuilder
    {
        public long Artikelnummer { get; private set; }
        public string Leveranciercode { get; private set; }
        public string Naam { get; private set; }
        public decimal Prijs { get; private set; }
        public DateTime? LeverbaarTot { get; private set; }

        public ArtikelBuilder SetArtikelNummer(long artikelNummer)
        {
            Artikelnummer = artikelNummer;
            return this;
        }

        public ArtikelBuilder SetLeveranciercode(string leveranciercode)
        {
            Leveranciercode = leveranciercode;
            return this;
        }

        public ArtikelBuilder SetNaam(string naam)
        {
            Naam = naam;
            return this;
        }

        public ArtikelBuilder SetPrijs(decimal prijs)
        {
            Prijs = prijs;
            return this;
        }

        public ArtikelBuilder SetLeverbaarTot(DateTime leverbaarTot)
        {
            LeverbaarTot = leverbaarTot;
            return this;
        }

        public Artikel Create()
        {
            return new Artikel
            {
                Artikelnummer =  Artikelnummer,
                Leveranciercode =  Leveranciercode,
                Naam = Naam,
                Prijs =  Prijs,
                LeverbaarTot = LeverbaarTot
            };
        }
    }
}
