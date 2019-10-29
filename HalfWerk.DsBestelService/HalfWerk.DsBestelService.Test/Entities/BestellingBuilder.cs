using HalfWerk.CommonModels.DsBestelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.DsBestelService.Test.Entities
{
    public class BestellingBuilder
    {
        private long Factuurnummer { get; set; }
        private long Klantnummer { get; set; }
        private BestelStatus BestelStatus { get; set; }
        private DateTime Besteldatum { get; set; }
        private ICollection<BestelRegel> BestelRegels { get; set; } = new HashSet<BestelRegel>();

        public BestellingBuilder SetFactuurnummer(long n) { Factuurnummer = n; return this; }
        public BestellingBuilder SetKlantnummer(long n) { Klantnummer = n; return this; }
        public BestellingBuilder SetBestelStatus(BestelStatus s) { BestelStatus = s; return this; }
        public BestellingBuilder SetBesteldatum(DateTime d) { Besteldatum = d; return this; }
        public BestellingBuilder SetBestelRegels(List<BestelRegel> ba) { BestelRegels = ba; return this; }

        public BestellingBuilder SetDummy()
        {
            Factuurnummer = DateTime.Now.GetHashCode();
            Klantnummer = 347;
            BestelStatus = BestelStatus.Geplaatst;
            Besteldatum = DateTime.Parse("1-1-2019 20:00:00");
            BestelRegels = SetDummyBestelRegels();

            return this;
        }

        private ICollection<BestelRegel> SetDummyBestelRegels()
        {
            ICollection<BestelRegel> artikelen = new List<BestelRegel>()
            {
                new BestelRegel()
                {
                    Artikelnummer = 12,
                    Naam = "Kip",
                    LeverancierCode = "KFC-234",
                    PrijsExclBtw = 3.99M,
                    Aantal = 1
                },
                new BestelRegel()
                {
                    Artikelnummer = 256,
                    Naam = "Hamburger",
                    LeverancierCode = "MCD-001",
                    PrijsExclBtw = 6.99M,
                    Aantal = 3
                }
            };
            return artikelen;
        }

        public Bestelling Create()
        {
            var bestelling = new Bestelling()
            {
                Factuurnummer = Factuurnummer,
                Klantnummer = Klantnummer,
                BestelStatus = BestelStatus,
                Besteldatum = Besteldatum,
                BestelRegels = BestelRegels.ToList()
            };

            return bestelling;
        }
    }
}
