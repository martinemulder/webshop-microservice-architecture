namespace HalfWerk.CommonModels.DsKlantBeheer.Models
{
    public class Adres
    {
        public long Id { get; set; }
        public string Straatnaam { get; set; }
        public string Postcode { get; set; }
        public string Huisnummer { get; set; }
        public string Plaats { get; set; }
        public string Land { get; set; }
    }
}