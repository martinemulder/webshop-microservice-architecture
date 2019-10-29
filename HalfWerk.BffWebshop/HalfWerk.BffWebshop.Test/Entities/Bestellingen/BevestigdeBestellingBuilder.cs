using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.DsBestelService.Models;
using System;
using System.Collections.Generic;
using BestelStatus = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelStatus;

namespace HalfWerk.BffWebshop.Test.Entities.Bestellingen
{
    public class BevestigdeBestellingBuilder
    {
        private long Id { get; set; }
        private long Factuurnummer { get; set; }
        private long Klantnummer { get; set; }
        private decimal FactuurTotaalExclBtw { get; set; }
        private decimal FactuurTotaalInclBtw { get; set; }
        private DateTime Besteldatum { get; set; }
        private BestelStatus BestelStatus { get; set; }
        private List<BevestigdeBestelRegel> BestelRegels { get; set; }

        public BevestigdeBestellingBuilder SetId(long id) { Id = id; return this; }
        public BevestigdeBestellingBuilder SetFactuurnummer(long factuurnummer) { Factuurnummer = factuurnummer; return this; }
        public BevestigdeBestellingBuilder SetKlantnummer(long klantnummer) { Klantnummer = klantnummer; return this; }
        public BevestigdeBestellingBuilder SetBesteldatum(DateTime dateTime) { Besteldatum = dateTime; return this; }
        public BevestigdeBestellingBuilder SetFactuurTotaalExclBtw(decimal factuurTotaalExclBtw)
        {
            FactuurTotaalExclBtw = factuurTotaalExclBtw;
            return this;
        }
        public BevestigdeBestellingBuilder SetFactuurTotaalInclBtw(decimal factuurTotaalInclBtw)
        {
            FactuurTotaalInclBtw = factuurTotaalInclBtw;
            return this;
        }
        public BevestigdeBestellingBuilder SetBestelStatus(BestelStatus bestelStatus) { BestelStatus = bestelStatus; return this; }
        public BevestigdeBestellingBuilder SetBestelRegels(List<BevestigdeBestelRegel> bestelRegelEntities) { BestelRegels = bestelRegelEntities; return this; }

        public BevestigdeBestellingBuilder SetDummy()
        {
            Id = DateTime.Now.GetHashCode();
            Factuurnummer = DateTime.Now.GetHashCode();
            Klantnummer = 3;
            FactuurTotaalExclBtw = 786.95M;
            FactuurTotaalInclBtw = 786.95M;
            Besteldatum = DateTime.Now;
            BestelStatus = BestelStatus.Geplaatst;
            BestelRegels = new List<BevestigdeBestelRegel>()
            {
                new BevestigdeBestelRegel()
                {
                    Id = DateTime.Now.GetHashCode(),
                    Artikelnummer = DateTime.Now.GetHashCode(),
                    Aantal = 1,
                    Naam = "Fiets",
                    PrijsExclBtw = 399.95M,
                    PrijsInclBtw = 399.95M,
                    RegelTotaalExclBtw = 399.95M,
                    RegelTotaalInclBtw = 399.95M,
                },
                new BevestigdeBestelRegel()
                {
                    Id = DateTime.Now.GetHashCode(),
                    Artikelnummer = DateTime.Now.GetHashCode(),
                    Aantal = 3,
                    Naam = "Helm",
                    PrijsExclBtw = 129.00M,
                    PrijsInclBtw = 129.00M,
                    RegelTotaalExclBtw = 387.00M,
                    RegelTotaalInclBtw = 387.00M,
                }
            };
            return this;
        }

        public BevestigdeBestelling Create()
        {
            return new BevestigdeBestelling()
            {
                Id = Id,
                Factuurnummer = Factuurnummer,
                Klantnummer = Klantnummer,
                FactuurTotaalInclBtw = FactuurTotaalInclBtw,
                FactuurTotaalExclBtw = FactuurTotaalExclBtw,
                Besteldatum = Besteldatum,
                BestelRegels = BestelRegels,
                BestelStatus = BestelStatus
            };
        }
    }

}
