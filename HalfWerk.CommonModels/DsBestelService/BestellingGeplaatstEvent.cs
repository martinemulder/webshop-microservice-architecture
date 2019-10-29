using HalfWerk.CommonModels.DsBestelService.Models;
using Minor.Nijn.WebScale.Events;

namespace HalfWerk.CommonModels.DsBestelService
{
    public class BestellingGeplaatstEvent : DomainEvent
    {
        public Bestelling Bestelling { get; set; } 

        public BestellingGeplaatstEvent(Bestelling bestelling, string routingKey) : base(routingKey)
        {
            Bestelling = bestelling;
        }
    }
}