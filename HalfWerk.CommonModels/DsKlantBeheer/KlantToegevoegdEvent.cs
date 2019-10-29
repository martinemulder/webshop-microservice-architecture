using HalfWerk.CommonModels.DsKlantBeheer.Models;
using Minor.Nijn.WebScale.Events;

namespace HalfWerk.CommonModels.DsKlantBeheer
{
    public class KlantToegevoegdEvent : DomainEvent
    {
        public Klant Klant { get; set; }

        public KlantToegevoegdEvent(Klant klant, string routingKey) : base(routingKey)
        {
            Klant = klant;
        }
    }
}