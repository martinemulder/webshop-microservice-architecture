using System;

namespace HalfWerk.CommonModels.DsBestelService.Models
{
    public class Artikel
    {
        public long Artikelnummer { get; set; }
        public string Leveranciercode { get; set; }
        public string Naam { get; set; }
        public decimal Prijs { get; set; }
        public DateTime? LeverbaarTot { get; set; }
    }
}
