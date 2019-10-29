using HalfWerk.CommonModels.DsKlantBeheer.Models;
using Minor.Nijn.WebScale.Commands;

namespace HalfWerk.CommonModels.DsKlantBeheer
{
    public class VoegKlantToeCommand : DomainCommand
    {
        public Klant Klant { get; set; }

        public VoegKlantToeCommand(Klant klant, string routingKey) : base(routingKey)
        {
            Klant = klant;
        }
    }
}