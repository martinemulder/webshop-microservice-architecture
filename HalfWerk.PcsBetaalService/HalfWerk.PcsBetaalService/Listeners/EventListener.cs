using AutoMapper;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Services;
using Minor.Nijn.WebScale.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.PcsBetaalService.Listeners
{
    [EventListener(NameConstants.BetaalServiceEventQueue)]
    public class EventListener
    {
        private readonly IBestellingDataMapper _bestellingDataMapper;
        private readonly IBetalingVerwerkenService _betalingVerwerkenService;

        public EventListener(IBestellingDataMapper bestellingDataMapper, IBetalingVerwerkenService betalingVerwerkenService)
        {
            _bestellingDataMapper = bestellingDataMapper;
            _betalingVerwerkenService = betalingVerwerkenService;
        }

        [Topic(NameConstants.BestelServiceBestellingGeplaatstEvent)]
        public void ReceiveBestellingGeplaatstEvent(BestellingGeplaatstEvent message)
        {
            var bestelling = Mapper.Map<Bestelling>(message.Bestelling);

            _bestellingDataMapper.Insert(bestelling);

            _betalingVerwerkenService.HandleBestellingVerwerken(bestelling.Factuurnummer);
        }
    }
}
