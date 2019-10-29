using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class BestelRegel
    {
        [Required(ErrorMessage = "Artikelnummer is verplicht")]
        [Range(1, long.MaxValue, ErrorMessage = "Artikelnummer moet groter dan 1 zijn")]
        public long Artikelnummer { get; set; }

        [Required(ErrorMessage = "Aantal is verplicht")]
        [Range(1, int.MaxValue, ErrorMessage = "Aantal moet tenminste 1 zijn")]
        public int Aantal { get; set; }
    }
}
