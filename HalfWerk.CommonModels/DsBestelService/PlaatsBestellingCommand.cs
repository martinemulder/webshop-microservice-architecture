using HalfWerk.CommonModels.DsBestelService.Models;
using Minor.Nijn.WebScale.Commands;

namespace HalfWerk.CommonModels.DsBestelService
{
    public class PlaatsBestellingCommand : DomainCommand
    {
        public BestellingCM Bestelling { get; set; }

        public PlaatsBestellingCommand(BestellingCM bestelling, string routingKey) : base(routingKey)
        {
            Bestelling = bestelling;
        }
    }
}