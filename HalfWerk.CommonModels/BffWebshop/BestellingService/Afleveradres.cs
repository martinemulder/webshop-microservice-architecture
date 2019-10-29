using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class Afleveradres
    {
        [Required(ErrorMessage = "Afleveradres is verplicht")]
        public string Adres { get; set; }

        [Required(ErrorMessage = "Postcode is verplicht")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Plaats is verplicht")]
        public string Plaats { get; set; }

        [Required(ErrorMessage = "Land is verplicht")]
        public string Land { get; set; }
    }
}