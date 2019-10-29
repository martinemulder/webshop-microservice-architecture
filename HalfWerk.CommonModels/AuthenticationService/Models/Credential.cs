using System.ComponentModel.DataAnnotations;

namespace HalfWerk.CommonModels.AuthenticationService.Models
{
    public class Credential
    {
        [Required(ErrorMessage = "Email is verplicht")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        public string Password { get; set; }
    }
}
