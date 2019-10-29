using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.CommonModels.PcsBetaalService.Models;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Services;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.PcsBetaalService.Services
{
    public class BetalingVerwerkenService : IBetalingVerwerkenService
    {
        private readonly ILogger<BetalingVerwerkenService> _logger;
        private readonly IBetalingDataMapper _betalingDataMapper;
        private readonly IBestellingDataMapper _bestellingDataMapper;
        private readonly IEventPublisher _eventPublisher;

        public BetalingVerwerkenService(IBetalingDataMapper betalingDataMapper, IBestellingDataMapper bestellingDataMapper, IEventPublisher eventPublisher, ILoggerFactory loggerFactory)
        {
            _betalingDataMapper = betalingDataMapper;
            _bestellingDataMapper = bestellingDataMapper;
            _eventPublisher = eventPublisher;

            _logger = loggerFactory.CreateLogger<BetalingVerwerkenService>();
        }

        public void HandleBetalingVerwerken(Betaling betaling)
        {
            var klantnummer = _bestellingDataMapper.GetByFactuurnummer(betaling.Factuurnummer).Klantnummer;
            betaling.Klantnummer = klantnummer;
            try
            {
                _betalingDataMapper.Insert(betaling);
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with factuurnummer: {0}", betaling.Factuurnummer);
                _logger.LogDebug(
                    "DB exception occured with betaling {}, it threw exception: {}. Inner exception: {}",
                    betaling, ex.Message, ex.InnerException?.Message
                );
                throw new InvalidFactuurnummerException("Something unexpected happend while inserting into the database");
            }
            ProcessBestellingVerwerken(klantnummer);
        }

        public void HandleBestellingVerwerken(long factuurnummer)
        {
            var klantnummer = _bestellingDataMapper.GetByFactuurnummer(factuurnummer).Klantnummer;
            ProcessBestellingVerwerken(klantnummer);
        }

        private void ProcessBestellingVerwerken(long klantnummer)
        {
            var bestellingen = _bestellingDataMapper.Find(b => b.Klantnummer == klantnummer &&
                                                          b.BestelStatus < BestelStatus.Betaald);

            decimal total = 0;
            DateTime dateTime = DateTime.Now;
            foreach (var bestelling in bestellingen)
            {
                total += bestelling.FactuurTotaalInclBtw;
                if (dateTime > bestelling.Besteldatum)
                {
                    dateTime = bestelling.Besteldatum;
                }
            }

            var betalingen = _betalingDataMapper.Find(b => b.Klantnummer == klantnummer);
            var betaald = 0.0m;
            foreach (var betaling in betalingen)
            {
                if (betaling.BetaalDatum > dateTime)
                {
                    betaald += betaling.Bedrag;
                }
            }
            if (total - betaald <= 500)
            {
                foreach (var item in bestellingen)
                {
                    _eventPublisher.Publish(new BestellingGeaccrediteerdEvent(item.Factuurnummer, NameConstants.BetaalServiceBetalingGeaccrediteerdEvent));
                }
            }
        }
    }
}
