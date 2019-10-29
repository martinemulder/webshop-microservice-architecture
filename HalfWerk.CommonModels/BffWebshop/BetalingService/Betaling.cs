using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.BetalingService
{
    public class Betaling
    {
        [Required(ErrorMessage = "Factuurnummer is verplicht")]
        [Range(0, long.MaxValue, ErrorMessage = "Factuurnummer moet groter dan 0 zijn")]
        public long Factuurnummer { get; set; }

        [Required(ErrorMessage = "Bedrag is verplicht")]
        [Range(0, double.MaxValue, ErrorMessage = "Bedrag moet groter dan 0 zijn")]
        public decimal Bedrag { get; set; }
    }
}
