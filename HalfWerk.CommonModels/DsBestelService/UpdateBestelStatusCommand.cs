using HalfWerk.CommonModels.DsBestelService.Models;
using Minor.Nijn.WebScale.Commands;

namespace HalfWerk.CommonModels.DsBestelService
{
    public class UpdateBestelStatusCommand : DomainCommand
    {
        public long Factuurnummer { get; set; }
        public BestelStatus BestelStatus { get; set; }

        public UpdateBestelStatusCommand(long factuurnummer, BestelStatus bestelstatus, string routingKey) : base(routingKey)
        {
            Factuurnummer = factuurnummer;
            BestelStatus = bestelstatus;
        }
    }
}
