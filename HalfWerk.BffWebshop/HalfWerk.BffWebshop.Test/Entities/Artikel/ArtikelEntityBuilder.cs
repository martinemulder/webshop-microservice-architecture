using HalfWerk.BffWebshop.Entities;
using System;
using System.Collections.Generic;

namespace HalfWerk.BffWebshop.Test.Entities
{
    public class ArtikelEntityBuilder
    {
        private long Artikelnummer { get; set; }
        private string Naam { get; set; }
        private string Beschrijving { get; set; }
        private decimal Prijs { get; set; }
        private string AfbeeldingUrl { get; set; }
        private DateTime LeverbaarVanaf { get; set; }
        private DateTime? LeverbaarTot { get; set; }
        private string Leveranciercode { get; set; }
        private string Leverancier { get; set; }
        private int Voorraad { get; set; }
        private ICollection<ArtikelCategorieEntity> ArtikelCategorieen { get; set; } = new HashSet<ArtikelCategorieEntity>();
        

        public ArtikelEntityBuilder SetArtikelnummer(long n) { Artikelnummer = n; return this; }
        public ArtikelEntityBuilder SetNaam(string s) { Naam = s; return this; }
        public ArtikelEntityBuilder SetBeschrijving(string s) { Beschrijving = s; return this; }
        public ArtikelEntityBuilder SetPrijs(decimal d) { Prijs = d; return this; }
        public ArtikelEntityBuilder SetAfbeeldingUrl(string s) { AfbeeldingUrl = s; return this; }
        public ArtikelEntityBuilder SetLeverbaarVanaf(DateTime dt) { LeverbaarVanaf = dt; return this; }
        public ArtikelEntityBuilder SetLeverbaarTot(DateTime dt) { LeverbaarTot = dt; return this; }
        public ArtikelEntityBuilder SetLeveranciercode(string s) { Leveranciercode = s; return this; }
        public ArtikelEntityBuilder SetLeverancier(string s) { Leverancier = s; return this; }
        public ArtikelEntityBuilder SetVoorraad(int v) { Voorraad = v; return this; }
        public ArtikelEntityBuilder SetArtikelCategorieen(ICollection<ArtikelCategorieEntity> ac) { ArtikelCategorieen = ac; return this; }
        public ArtikelEntityBuilder SetDummy()
        {
            Artikelnummer = 1;
            Naam = "Dummy";
            Beschrijving = "Dummy object";
            Prijs = 1.50M;
            AfbeeldingUrl = "";
            LeverbaarVanaf = DateTime.Now;
            LeverbaarTot = null;
            Leveranciercode = "Dummy";
            Leverancier = "Dummy";
            Voorraad = 0;

            return this;
        }

        public ArtikelEntityBuilder SetDummyCategorie(string categorie)
        {
            var ac = new ArtikelCategorieEntity()
            {
                Categorie = new CategorieEntity()
                {
                    Categorie = categorie
                }
            };

            ArtikelCategorieen.Add(ac);
            
            return this;
        }

        public ArtikelEntity Create()
        {
            var artikelEntity = new ArtikelEntity()
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
                Voorrraad = Voorraad,
                ArtikelCategorieen = ArtikelCategorieen
            };

            foreach(var ac in artikelEntity.ArtikelCategorieen)
            {
                ac.Artikel = artikelEntity;
            }

            return artikelEntity;
        }
    }
}
