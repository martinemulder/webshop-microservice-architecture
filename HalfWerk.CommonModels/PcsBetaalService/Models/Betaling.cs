using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.PcsBetalingService.Models
{
    public class Betaling
    {
        public long Id { get; set; }
        public long Factuurnummer { get; set; }
        public long Klantnummer { get; set; }
        public decimal Bedrag { get; set; }
        public DateTime BetaalDatum { get; set; }
    }
}
