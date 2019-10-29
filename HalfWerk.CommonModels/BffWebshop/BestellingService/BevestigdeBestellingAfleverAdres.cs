namespace HalfWerk.CommonModels.BffWebshop.BestellingService
{
    public class BevestigdeBestellingAfleveradres
    {
        public long Id { get; set; }
        public string Adres { get; set; }
        public string Postcode { get; set; }
        public string Plaats { get; set; }
        public string Land { get; set; }
    }
}