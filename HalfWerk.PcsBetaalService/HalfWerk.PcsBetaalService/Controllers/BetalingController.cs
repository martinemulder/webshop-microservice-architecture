using AutoMapper;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.CommonModels.PcsBetalingService;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Services;
using Minor.Nijn.WebScale.Attributes;
using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HalfWerk.PcsBetaalService.Controllers
{
    [CommandListener]
    public class BetalingController
    {
        public IBetalingVerwerkenService _betalingVerwerkenService { get; set; }
        public BetalingController(IBetalingVerwerkenService betalingVerwerkenService)
        {
            _betalingVerwerkenService = betalingVerwerkenService;
        }

        [Command(NameConstants.BetaalServiceBetalingVerwerkenCommandQueue)]
        public long HandleVerWerkBetalingCommand(VerwerkBetalingCommand verwerkBetalingCommand)
        {
            var betaling = Mapper.Map<Betaling>(verwerkBetalingCommand.BetalingCM);
            betaling.BetaalDatum = DateTime.Now;
            _betalingVerwerkenService.HandleBetalingVerwerken(betaling);

            return verwerkBetalingCommand.BetalingCM.Factuurnummer;
        }
    }
}
