namespace HalfWerk.CommonModels.DsKlantBeheer.Models
{
    public class Klant
    {
        public long Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Telefoonnummer { get; set; }
        public string Email { get; set; }
        public Adres Adres { get; set; }
    }
}