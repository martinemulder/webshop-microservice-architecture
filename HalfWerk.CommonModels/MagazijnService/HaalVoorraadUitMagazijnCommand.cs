using Minor.Nijn.WebScale.Commands;

namespace HalfWerk.CommonModels.MagazijnService
{
    public class HaalVoorraadUitMagazijnCommand
    {
        public int Artikelnummer { get; set; }
        public int Aantal { get; set; }
    }
}