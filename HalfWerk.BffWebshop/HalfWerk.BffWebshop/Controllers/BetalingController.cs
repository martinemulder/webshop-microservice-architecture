using HalfWerk.BffWebshop.Helpers;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.PcsBetalingService;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Commands;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetalingController : ControllerBase
    {
        private readonly ILogger<BetalingController> _logger;
        private readonly ICommandPublisher _commandPublisher;

        public BetalingController(ICommandPublisher commandPublisher, ILoggerFactory loggerFactory)
        {
            _commandPublisher = commandPublisher;
            _logger = loggerFactory.CreateLogger<BetalingController>();
        }

        [HttpPost]
        [JwtInRole("Sales")]
        [SwaggerOperation(
            Summary = "Verwerk betaling",
            OperationId = "BetalingVerwerken"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Betaling is verwerkt")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Factuurnummer kon niet worden gevonden")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout, "Aanvraag kon niet worden verwerkt")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Something unexpected happend")]
        public async Task<IActionResult> BetalingVerwerken([FromBody] BetalingCM betaling)
        {
            var betalingVerwerkenCommand = new VerwerkBetalingCommand(betaling, NameConstants.BetaalServiceBetalingVerwerkenCommandQueue);

            try
            {
                await _commandPublisher.Publish<long>(betalingVerwerkenCommand);
            }
            catch (InvalidFactuurnummerException)
            {
                _logger.LogError("InvalidFactuurnummerException occured, the factuurnummer is: {}", betaling.Factuurnummer);
                return BadRequest($"Factuurnummer {betaling.Factuurnummer} kon niet worden gevonden");
            }
            catch (TimeoutException)
            {
                _logger.LogError("BetalingVerwerkenCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kon niet worden verwerkt");
            }
            catch (Exception ex)
            {
                _logger.LogError("BetalingVerwerkenCommand resulted in an internal server error.");
                _logger.LogDebug(
                    "Exception occured during execution of UpdateBestelStatusCommand {}, it threw exception: {}. Inner exception: {}",
                    betaling, ex.Message, ex.InnerException?.Message
                );
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int) HttpStatusCode.OK);
        }
    }
}