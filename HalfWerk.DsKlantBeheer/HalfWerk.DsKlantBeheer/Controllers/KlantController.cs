using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsKlantBeheer;
using HalfWerk.DsKlantBeheer.DAL.DataMappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Attributes;
using Minor.Nijn.WebScale.Events;
using System;
using System.Linq;

namespace HalfWerk.DsKlantBeheer.Controllers
{
    [CommandListener]
    public class KlantController
    {
        private readonly IKlantDataMapper _klantDataMapper;
        private readonly IEventPublisher _eventPublish;
        private readonly ILogger<KlantController> _logger;

        public KlantController(IKlantDataMapper klantDataMapper,
                               IEventPublisher eventPublisher, 
                               ILoggerFactory loggerFactory)
        {
            _klantDataMapper = klantDataMapper;
            _eventPublish = eventPublisher;
            _logger = loggerFactory.CreateLogger<KlantController>();
        }

        [Command(NameConstants.KlantBeheerVoegKlantToeCommand)]
        public long HandleVoegKlantToe(VoegKlantToeCommand request)
        {
            var klant = request.Klant;

            try
            {
                _klantDataMapper.Insert(klant);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Email allready exists with email: {0}", klant.Email);
                _logger.LogDebug(
                    "DB exception occured with klant {}, it threw exception: {}. Inner exception: {}",
                    klant, ex.Message, ex.InnerException?.Message);
                throw new EmailAllreadyExistsException("Something unexpected happened whule inserting into the database");
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with klantnummer: {0}", klant?.Id);
                _logger.LogDebug(
                    "DB exception occured with klant {}, it threw exception: {}. Inner exception: {}",
                    klant, ex.Message, ex.InnerException?.Message
                );
                throw new DatabaseException("Something unexpected happend while inserting into the database");
            }

            _eventPublish.Publish(new KlantToegevoegdEvent(klant, NameConstants.KlantBeheerKlantToegevoegdEvent));
            return klant.Id;
        }
    }
}
