using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.DAL.DataMappers;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Attributes;
using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HalfWerk.CommonModels.MagazijnService;
using HalfWerk.DsBestelService.Helpers;
using Minor.Nijn;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Bestelling = HalfWerk.CommonModels.DsBestelService.Models.Bestelling;

namespace HalfWerk.DsBestelService.Controllers
{
    [CommandListener]
    public class BestelStatusController
    {
        private readonly ILogger<BestelStatusController> _logger;

        private readonly IBestellingDataMapper _bestellingDataMapper;
        private readonly IEventPublisher _eventPublish;
        private readonly ICommandSender _commandSender;

        public BestelStatusController(
            IBusContext<IConnection> context, 
            IBestellingDataMapper bestellingDataMapper, 
            IEventPublisher eventPublisher, 
            ILoggerFactory loggerFactory)
        {
            _bestellingDataMapper = bestellingDataMapper;
            _eventPublish = eventPublisher;
            _commandSender = context.CreateCommandSender();
            _logger = loggerFactory.CreateLogger<BestelStatusController>();
        }

        [Command(NameConstants.BestelServiceUpdateBestelStatusCommandQueue)]
        public async Task<long> HandleUpdateBestelStatus(UpdateBestelStatusCommand request)
        {
            var bestelling = GetBestellingByFactuurNummer(request.Factuurnummer);

            if (!bestelling.BestelStatus.UpdateIsAllowed(request.BestelStatus))
            {
                _logger.LogError("Invalid BestelStatus update, cannot update status {from} to {to}", bestelling.BestelStatus, request.BestelStatus);
                throw new InvalidUpdateException("BestelStatus update not allowed");
            }

            bestelling.BestelStatus = request.BestelStatus;
            UpdateBestelling(bestelling);

            if (bestelling.BestelStatus == BestelStatus.Verzonden)
            {
                await VerlaagVoorraad(bestelling);
            }

            _eventPublish.Publish(new BestelStatusBijgewerktEvent(bestelling, NameConstants.BestelServiceBestelStatusUpgedateEvent));
            return request.Factuurnummer;
        }

        private void UpdateBestelling(Bestelling bestelling)
        {
            try
            {
                _bestellingDataMapper.Update(bestelling);
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with factuurnummer: {0}", bestelling.Factuurnummer);
                _logger.LogDebug(
                    "DB exception occured while updating with factuurnummer {}, it threw exception: {}. Inner exception: {}",
                    bestelling.Factuurnummer, ex.Message, ex.InnerException?.Message
                );

                throw new DatabaseException("Something unexpected happend while updating the database");
            }
        }

        private Bestelling GetBestellingByFactuurNummer(long factuurnummer)
        {
            try
            {
                return _bestellingDataMapper.GetByFactuurnummer(factuurnummer);
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with factuurnummer: {0}", factuurnummer);
                _logger.LogDebug(
                    "DB exception occured while finding factuurnummer {}, it threw exception: {}. Inner exception: {}",
                    factuurnummer, ex.Message, ex.InnerException?.Message
                );

                throw new DatabaseException("Something unexpected happend while find bestelling in the database");
            }
        }

        private Task VerlaagVoorraad(Bestelling bestelling)
        {
            var tasks = new List<Task>();

            foreach (var regel in bestelling.BestelRegels)
            {
                var body = new HaalVoorraadUitMagazijnCommand
                {
                    Artikelnummer = (int)regel.Artikelnummer,
                    Aantal = regel.Aantal
                };

                var request = new RequestCommandMessage(
                    message: JsonConvert.SerializeObject(body),
                    type: NameConstants.MagazijnServiceHaalVoorraadUitMagazijnCommand,
                    correlationId: "",
                    routingKey: NameConstants.MagazijnServiceCommandQueue
                );

                tasks.Add(_commandSender.SendCommandAsync(request));
            }

            return Task.WhenAll(tasks);
        }
    }
}
