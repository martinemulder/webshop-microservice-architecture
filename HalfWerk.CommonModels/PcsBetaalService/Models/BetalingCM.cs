using Minor.Nijn.WebScale.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.PcsBetalingService.Models
{
    public class BetalingCM
    { 
        public long Factuurnummer { get; set; }
        public decimal Bedrag { get; set; }

        public BetalingCM(long factuurnummer, decimal bedrag)
        {
            Factuurnummer = factuurnummer;
            Bedrag = bedrag;
        }
    }
}
