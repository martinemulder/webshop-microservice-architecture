using HalfWerk.CommonModels.PcsBetalingService.Models;
using Minor.Nijn.WebScale.Commands;

namespace HalfWerk.CommonModels.PcsBetalingService
{
    public class VerwerkBetalingCommand : DomainCommand
    {
        public BetalingCM BetalingCM{ get; set; }

        public VerwerkBetalingCommand(BetalingCM betalingCM, string routingKey) : base(routingKey)
        {
            BetalingCM = betalingCM;
        }
    }
}
