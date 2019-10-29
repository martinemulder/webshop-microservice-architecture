using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class ContactInfo
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        public string Naam { get; set; }

        [Required(ErrorMessage = "Email is verplicht")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefoonnummer is verplicht")]
        public string Telefoonnummer { get; set; }
    }
}