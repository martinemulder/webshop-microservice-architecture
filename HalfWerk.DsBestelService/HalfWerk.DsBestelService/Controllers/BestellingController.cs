using AutoMapper;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Helpers;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Attributes;
using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.DsBestelService.Controllers
{
    [CommandListener]
    public class BestellingController
    {
        private readonly ILogger<BestellingController> _logger;

        private readonly IBestellingDataMapper _bestellingDataMapper;
        private readonly IArtikelDataMapper _artikelDataMapper;
        private readonly IEventPublisher _eventPublish;

        public BestellingController(
            IBestellingDataMapper bestellingDataMapper, 
            IArtikelDataMapper artikelDataMapper, 
            IEventPublisher eventPublisher, 
            ILoggerFactory loggerFactory)
        {
            _bestellingDataMapper = bestellingDataMapper;
            _artikelDataMapper = artikelDataMapper;
            _eventPublish = eventPublisher;
            _logger = loggerFactory.CreateLogger<BestellingController>();
        }

        [Command(NameConstants.BestelServicePlaatsBestellingCommandQueue)]
        public long HandlePlaatsBestelling(PlaatsBestellingCommand request)
        {
            var bestelling = MapBestelling(request.Bestelling);

            try
            {
                _bestellingDataMapper.Insert(bestelling);
                bestelling.Factuurnummer = bestelling.Id;
                _bestellingDataMapper.Update(bestelling);
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with klantnummer: {0}", bestelling.Klantnummer);
                _logger.LogDebug(
                    "DB exception occured with klant {}, it threw exception: {}. Inner exception: {}", 
                    bestelling, ex.Message, ex.InnerException?.Message
                );
                throw new DatabaseException("Something unexpected happend while inserting into the database");
            }

            _eventPublish.Publish(new BestellingGeplaatstEvent(bestelling, NameConstants.BestelServiceBestellingGeplaatstEvent));
            return bestelling.Id;
        }

        private Bestelling MapBestelling(BestellingCM bestellingCM)
        {
            var contactInfo = Mapper.Map<ContactInfo>(bestellingCM.ContactInfo);
            var afleveradres = Mapper.Map<Afleveradres>(bestellingCM.Afleveradres);

            var bestelling = new Bestelling
            {
                Klantnummer = bestellingCM.Klantnummer,
                ContactInfo = contactInfo,
                Afleveradres = afleveradres,
                BestelStatus = BestelStatus.Geplaatst,
                Besteldatum = DateTime.Now,
                BestelRegels = new List<BestelRegel>()
            };

            MapBestelRegel(bestellingCM, bestelling);
            CalculateBestellingSum(ref bestelling);
            return bestelling;
        }

        private void MapBestelRegel(BestellingCM bestellingCM, Bestelling bestelling)
        {
            var bestelRegels = bestellingCM?.BestelRegels ?? new List<BestelRegelCM>();
            foreach (var regel in bestelRegels)
            {
                var artikel = _artikelDataMapper.GetById(regel.Artikelnummer);

                var regelResult = Mapper.Map<BestelRegel>(artikel);
                regelResult.Aantal = regel.Aantal;
                regelResult.PrijsExclBtw = artikel.Prijs;
                CalculateRegelSum(ref regelResult);

                bestelling.BestelRegels.Add(regelResult);
            }
        }

        private void CalculateRegelSum(ref BestelRegel regel)
        {
            regel.PrijsInclBtw = regel.PrijsExclBtw.CalculatePrijsInclBtw();
            regel.RegelTotaalExclBtw = regel.PrijsExclBtw.CalculatePrijsExclBtw(regel.Aantal);
            regel.RegelTotaalInclBtw = regel.PrijsExclBtw.CalculatePrijsInclBtw(regel.Aantal);
        }

        private void CalculateBestellingSum(ref Bestelling bestelling)
        {
            bestelling.FactuurTotaalExclBtw = bestelling.BestelRegels.Sum(r => r.RegelTotaalExclBtw).RoundPrice();
            bestelling.FactuurTotaalInclBtw = bestelling.BestelRegels.Sum(r => r.RegelTotaalInclBtw).RoundPrice();
        }
    }
}