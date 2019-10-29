using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.KlantBeheer
{
    public class KlantRegistration
    {
        [Required(ErrorMessage = "Email is verplicht")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Klant is verplicht")]
        public Klant Klant { get; set; }
    }
}