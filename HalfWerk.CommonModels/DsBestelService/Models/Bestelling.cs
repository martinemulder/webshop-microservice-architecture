using System;
using System.Collections.Generic;

namespace HalfWerk.CommonModels.DsBestelService.Models
{
    public class Bestelling
    {
        public long Id { get; set; }
        public long Factuurnummer { get; set; }
        public long Klantnummer { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public Afleveradres Afleveradres { get; set; }
        public BestelStatus BestelStatus { get; set; }      
        public DateTime Besteldatum { get; set; }
        public decimal FactuurTotaalExclBtw { get; set; }
        public decimal FactuurTotaalInclBtw { get; set; }

        public List<BestelRegel> BestelRegels { get; set; }
    }
}
