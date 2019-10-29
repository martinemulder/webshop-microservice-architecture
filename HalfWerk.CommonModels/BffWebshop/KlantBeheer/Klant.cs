using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.KlantBeheer
{
    public class Klant
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Voornaam is verplicht")]
        public string Voornaam { get; set; }

        [Required(ErrorMessage = "Achternaam is verplicht")]
        public string Achternaam { get; set; }

        [Required(ErrorMessage = "Telefoonnummer is verplicht")]
        public string Telefoonnummer { get; set; }

        [Required(ErrorMessage = "Email is verplicht")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adres is verplicht")]
        public Adres Adres { get; set; }
    }
}
