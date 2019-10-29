using System;
using System.Collections.Generic;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class BevestigdeBestelling
    {
        public long Id { get; set; }
        public long Factuurnummer { get; set; }
        public long Klantnummer { get; set; }
        public BevestigdeBestellingContactInfo ContactInfo { get; set; }
        public BevestigdeBestellingAfleveradres Afleveradres { get; set; }
        public decimal FactuurTotaalExclBtw { get; set; }
        public decimal FactuurTotaalInclBtw { get; set; }
        public DateTime Besteldatum { get; set; }
        public BestelStatus BestelStatus { get; set; }

        public List<BevestigdeBestelRegel> BestelRegels { get; set; }
    }
}
