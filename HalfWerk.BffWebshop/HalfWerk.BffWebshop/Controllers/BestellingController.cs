using AutoMapper;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.BffWebshop.Helpers;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Commands;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bestelling = HalfWerk.CommonModels.BffWebshop.BestellingService.Bestelling;
using BffWebshopBestelStatus = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelStatus;
using DsBestelServiceBestelStatus = HalfWerk.CommonModels.DsBestelService.Models.BestelStatus;


namespace HalfWerk.BffWebshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestellingController : ControllerBase
    {
        private readonly ILogger<BestellingController> _logger;

        private readonly ICommandPublisher _commandPublisher;
        private readonly IBestellingDataMapper _bestellingDataMapper;
        private readonly IKlantDataMapper _klantDataMapper;
        private readonly IMagazijnSessionDataMapper _magazijnSessionDataMapper;
        private readonly IJwtHelper _jwtHelper;

        public BestellingController(
            ICommandPublisher commandPublisher,
            IBestellingDataMapper bestellingDataMapper,
            IKlantDataMapper klantDataMapper,
            IMagazijnSessionDataMapper magazijnSessionData,
            IJwtHelper jwtHelper,
            ILoggerFactory loggerFactory)
        {
            _commandPublisher = commandPublisher;
            _bestellingDataMapper = bestellingDataMapper;
            _klantDataMapper = klantDataMapper;
            _jwtHelper = jwtHelper;
            _magazijnSessionDataMapper = magazijnSessionData;
            _logger = loggerFactory.CreateLogger<BestellingController>();
        }

        [HttpGet]
        [JwtInRole("Sales")]
        [SwaggerOperation(
            Summary = "Lijst van bestellingen",
            OperationId = "GetBestellingen"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Bestelling gevonden")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Geen bestelling gevonden")]
        public ActionResult<IEnumerable<BevestigdeBestelling>> GetBestellingen([FromQuery] string status = null)
        {
            if (status == null)
            {
                return _bestellingDataMapper.GetAll().ToList();
            }

            Enum.TryParse<BffWebshopBestelStatus>(status, out var bestelStatus);
            return _bestellingDataMapper.Find(b => b.BestelStatus == bestelStatus).ToList();
        }

        [HttpGet("next")]
        [JwtInRole("Magazijn", "Sales")]
        [SwaggerOperation(
            Summary = "Verkrijg de eerstvolgende bestelling",
            OperationId = "GetNextBestelling"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Eerstvolgende bestelling gevonden")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Geen eerstvolgende bestelling gevonden")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout, "Aanvraag kon niet worden verwerkt")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something unexpected happend")]
        public async Task<ActionResult<BevestigdeBestelling>> GetNextBestelling()
        {
            var email = _jwtHelper.GetEmail(HttpContext);
            var session = _magazijnSessionDataMapper.Find(m => m.MedewerkerEmail == email).FirstOrDefault();

            var bestelling = session != null
            ? _bestellingDataMapper.GetByFactuurnummer(session.Factuurnummer)
            : _bestellingDataMapper
                .Find(f => f.BestelStatus == BffWebshopBestelStatus.Goedgekeurd)
                .OrderBy(b => b.Besteldatum)
                .FirstOrDefault();

            if (bestelling == null)
            {
                _logger.LogInformation("Couldn't find any bestelling that has status: Goedgekeurd");
                return NoContent();
            }

            try
            {
                if (session == null)
                {
                    await VerstuurUpdateBestelStatus(bestelling.Factuurnummer, BffWebshopBestelStatus.WordtIngepakt);
                    _magazijnSessionDataMapper.Insert(new MagazijnSessionEntity
                    {
                        Factuurnummer = bestelling.Factuurnummer,
                        MedewerkerEmail = email
                    });
                }

                return bestelling;
            }
            catch (TimeoutException)
            {
                _logger.LogError("UpdateBestelStatusCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kon niet worden verwerkt");
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateBestelStatusCommand after GetNextOrder resulted in an error.");
                _logger.LogDebug(
                    "Exception occured during execution of UpdateBestelStatusCommand when getting next order, it threw exception: {}. Inner exception: {}",
                    ex.Message, ex.InnerException?.Message
                );

                return StatusCode(
                    (int) HttpStatusCode.InternalServerError, 
                    "Er is een fout opgetreden tijdens het updaten van de bestelstatus"
                );
            }
        }

        [HttpPost]
        [JwtInRole("Klant")]
        [SwaggerOperation(
            Summary = "Plaats een nieuwe bestelling",
            OperationId = "CreateBestelling"
        )]
        [SwaggerResponse((int)HttpStatusCode.Created, "Bestelling geplaatst")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bestelling in invalide formaat")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout, "Bestelling kan niet worden geplaatst")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something unexpected happend")]
        public async Task<IActionResult> CreateBestelling([FromBody] Bestelling incomingBestelling)
        {
            var klantEmail = _jwtHelper.GetEmail(HttpContext);
            var klant = _klantDataMapper.GetByEmail(klantEmail);

            var bestelling = Mapper.Map<BestellingCM>(incomingBestelling);
            bestelling.Klantnummer = klant.Id;
            
            try
            {
                var bestellingCommand = new PlaatsBestellingCommand(bestelling, NameConstants.BestelServicePlaatsBestellingCommandQueue);
                var result = await _commandPublisher.Publish<long>(bestellingCommand);
                return Created($"/api/bestelling/{result}", result);
            }
            catch (DatabaseException)
            {
                _logger.LogError("PlaatsBestellingCommand resulted in a DatabaseException.");
                return BadRequest("Bestelling bevat incorrecte data");
            }
            catch (TimeoutException)
            {
                _logger.LogError("PlaatsBestellingCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kon niet worden verwerkt");
            }
            catch (Exception ex)
            {
                _logger.LogError("PlaatsBestellingCommand resulted in an internal server error.");
                _logger.LogDebug(
                    "Exception occured during execution of PlaatsBestellingCommand {}, it threw exception: {}. Inner exception: {}",
                    bestelling, ex.Message, ex.InnerException?.Message
                );

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{factuurnummer}/status")]
        [JwtInRole("Magazijn", "Sales")]
        [SwaggerOperation(
            Summary = "Update status van een bestelling",
            OperationId = "UpdateBestelStatus"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Status van bestelling bijgewerkt")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "UpdateBestellingStatus in invalide formaat")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout, "Status kon niet worden bijgewerkt")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something unexpected happend")]
        public async Task<IActionResult> UpdateBestelStatus(long factuurnummer, [FromBody] UpdateBestelStatus bestelStatus)
        {
            try
            {
                await VerstuurUpdateBestelStatus(factuurnummer, bestelStatus.Status);

                if (bestelStatus.Status == BffWebshopBestelStatus.Verzonden)
                {
                    var session = _magazijnSessionDataMapper.GetByFactuurnummer(factuurnummer);
                    _magazijnSessionDataMapper.Delete(session);
                }

                return Ok();
            }
            catch (DatabaseException)
            {
                _logger.LogError("UpdateBestelStatusCommand resulted in a DatabaseException.");
                return BadRequest("Factuurnummer is incorrect");
            }
            catch (InvalidUpdateException e)
            {
                _logger.LogError("UpdateBestelStatusCommand resulted in a InvalidUpdateException");
                _logger.LogDebug("Exception message: {}", e.Message);
                return BadRequest($"Update van bestelstatus naar: {bestelStatus.Status} is niet toegestaan");
            }
            catch (TimeoutException)
            {
                _logger.LogError("UpdateBestelStatusCommand resulted in a TimeoutException.");
                return StatusCode((int) HttpStatusCode.RequestTimeout, "Aanvraag kon niet worden verwerkt");
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateBestelStatusCommand resulted in an internal server error.");
                _logger.LogDebug(
                    "Exception occured during execution of UpdateBestelStatusCommand {}, it threw exception: {}. Inner exception: {}",
                    bestelStatus.Status, ex.Message, ex.InnerException?.Message
                );
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task VerstuurUpdateBestelStatus(long factuurnummer, BffWebshopBestelStatus newStatus)
        {
            var status = newStatus.CastTo<DsBestelServiceBestelStatus>();

            var updateBestelStatusCommand = new UpdateBestelStatusCommand(
                factuurnummer: factuurnummer, 
                bestelstatus: status, 
                routingKey: NameConstants.BestelServiceUpdateBestelStatusCommandQueue
            );

            await _commandPublisher.Publish<long>(updateBestelStatusCommand);
        }
    }
}