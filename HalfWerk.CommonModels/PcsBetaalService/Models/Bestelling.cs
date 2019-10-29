using HalfWerk.CommonModels.PcsBetaalService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.PcsBetalingService.Models
{
    public class Bestelling
    {
        public long Id { get; set; }
        public long Factuurnummer { get; set; }
        public long Klantnummer { get; set; }
        public BestelStatus BestelStatus { get; set; }
        public DateTime Besteldatum { get; set; }
        public decimal FactuurTotaalExclBtw { get; set; }
        public decimal FactuurTotaalInclBtw { get; set; }

        public List<BestelRegel> BestelRegels { get; set; }
    }
}
