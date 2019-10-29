using Minor.Nijn.WebScale.Events;

namespace HalfWerk.CommonModels.PcsBetaalService
{
    public class BestellingGeaccrediteerdEvent : DomainEvent
    {
        public long Factuurnummer { get; set; }

        public BestellingGeaccrediteerdEvent(long factuurnummer, string routingKey) : base(routingKey)
        {
            Factuurnummer = factuurnummer;
        }
    }
}