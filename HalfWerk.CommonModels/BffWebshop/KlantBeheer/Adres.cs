using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.KlantBeheer
{
    public class Adres
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Straatnaam is verplicht")]
        public string Straatnaam { get; set; }

        [Required(ErrorMessage = "Postcode is verplicht")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Huisnummer is verplicht")]
        public string Huisnummer { get; set; }

        [Required(ErrorMessage = "Plaatsnaam is verplicht")]
        public string Plaats { get; set; }

        [Required(ErrorMessage = "Land is verplicht")]
        public string Land { get; set; }

    }
}
