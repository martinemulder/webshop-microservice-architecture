using HalfWerk.CommonModels.DsBestelService.Models;
using Minor.Nijn.WebScale.Events;

namespace HalfWerk.CommonModels.DsBestelService
{
    public class BestelStatusBijgewerktEvent : DomainEvent
    {
        public Bestelling Bestelling { get; set; }

        public BestelStatusBijgewerktEvent(Bestelling bestelling, string routingKey) : base(routingKey)
        {
            Bestelling = bestelling;
        }
    }
}
