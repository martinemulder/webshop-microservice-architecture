using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class Bestelling
    {
        [Required(ErrorMessage = "Contact info is verplicht")]
        public ContactInfo ContactInfo { get; set; }

        [Required(ErrorMessage = "Afleveradres is verplicht")]
        public Afleveradres Afleveradres { get; set; }

        [Required(ErrorMessage = "BestelRegels is verplicht")]
        [MinLength(1, ErrorMessage = "Bestelling moet tenminste een BestelRegel hebben")]
        public List<BestelRegel> BestelRegels { get; set; }
    }
}
