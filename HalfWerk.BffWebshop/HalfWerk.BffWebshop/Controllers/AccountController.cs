using AutoMapper;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Helpers;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.AuthenticationService;
using HalfWerk.CommonModels.AuthenticationService.Models;
using HalfWerk.CommonModels.BffWebshop;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using HalfWerk.CommonModels.DsKlantBeheer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Commands;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Klant = HalfWerk.CommonModels.DsKlantBeheer.Models.Klant;

namespace HalfWerk.BffWebshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ICommandPublisher _commandPublisher;
        private readonly ILogger<AccountController> _logger;
        private readonly IJwtHelper _jwtHelper;
        private readonly IKlantDataMapper _klantDataMapper;

        public AccountController(ICommandPublisher commandPublisher,
                                 ILoggerFactory loggerFactory,
                                 IJwtHelper jwtHelper,
                                 IKlantDataMapper klantDataMapper)
        {
            _commandPublisher = commandPublisher;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _jwtHelper = jwtHelper;
            _klantDataMapper = klantDataMapper;
        }

        [HttpGet]
        [JwtInRole("Sales", "Magazijn", "Klant")]
        [SwaggerOperation(
            Summary = "Verkrijg klant gegevens",
            OperationId = "Index"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK, "Eerstvolgende bestelling gevonden")]
        public ActionResult<CommonModels.BffWebshop.KlantBeheer.Klant> Index()
        {
            var email = _jwtHelper.GetEmail(HttpContext);
            var klant = _klantDataMapper.GetByEmail(email);
            return klant;
        }

        [HttpPost]
        [Route("register")]
        [SwaggerOperation(
            Summary = "Registreer een account",
            OperationId = "Register"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: "Invalid parameters or exception")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, description: "Email already in use")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout)]
        public async Task<ActionResult> Register([FromBody] KlantRegistration registration)
        {
            if (string.IsNullOrWhiteSpace(registration.Email) || string.IsNullOrWhiteSpace(registration.Password))
            {
                return BadRequest("Invalid post parameters");
            }

            //TODO Check if email and password are valid

            try
            {
                var registerCommand = new RegisterCommand(
                    new Credential() { Email = registration.Email, Password = registration.Password }, 
                    NameConstants.AuthenticationServiceRegisterCommand
                );

                RegisterResult registerResult = await _commandPublisher.Publish<RegisterResult>(registerCommand);
                if (registerResult != RegisterResult.Ok)
                {
                    return Conflict("Account already exists");
                }
            }
            catch(TimeoutException)
            {
                _logger.LogError("RegisterCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kan niet worden verwerkt");
            }
            catch(Exception ex)
            {
                _logger.LogError("RegisterCommand resulted in an error.");
                _logger.LogDebug(
                    "Exception occured during execution of RegisterCommand, it threw exception: {}. Inner exception: {}",
                    ex.Message, ex.InnerException?.Message
                );
                return BadRequest("Failed to register due to an internal error");
            }
            try
            {
                var klant = Mapper.Map<Klant>(registration.Klant);
                var klantCommand = new VoegKlantToeCommand(klant, NameConstants.KlantBeheerVoegKlantToeCommand);
                await _commandPublisher.Publish<long>(klantCommand);
                return Ok();
            }
            catch (TimeoutException)
            {
                _logger.LogError("KlantCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kan niet worden verwerkt");
            }
            catch (Exception ex)
            {
                _logger.LogError("KlantCommand resulted in an error.");
                _logger.LogDebug(
                    "Exception occured during execution of KlantCommand, it threw exception: {}. Inner exception: {}",
                    ex.Message, ex.InnerException?.Message
                );
                return BadRequest("Failed to create klant due to an internal error");
            }
        }

        [HttpPost]
        [Route("login")]
        [SwaggerOperation(
            Summary = "Inloggen met een account",
            OperationId = "Login"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description:"Invalid parameters or exception")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description:"Credentials dont match with stored credentials")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout)]
        public async Task<ActionResult<JwtToken>> Login([FromBody] Credential credential)
        {
            if (string.IsNullOrWhiteSpace(credential.Email) || string.IsNullOrWhiteSpace(credential.Password))
            {
                return BadRequest("Invalid post parameters");
            }

            try
            {
                var registerCommand = new LoginCommand(
                    new Credential() { Email = credential.Email, Password = credential.Password }, 
                    NameConstants.AuthenticationServiceLoginCommand
                );

                string jwtToken = await _commandPublisher.Publish<string>(registerCommand);
                if(jwtToken.Length == 0)
                {
                    return Unauthorized();
                }

                return Ok(new JwtToken() { Token = jwtToken });
            }
            catch (TimeoutException)
            {
                _logger.LogError("LoginCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kan niet worden verwerkt");
            }
            catch (Exception)
            {
                return BadRequest("Failed to login due to an internal error");
            }
        }

        [HttpPost]
        [Route("validate")]
        [SwaggerOperation(
             Summary = "Valideren van een JwtToken",
             OperationId = "Validate"
         )]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, description: "Token is invalid")]
        [SwaggerResponse((int)HttpStatusCode.RequestTimeout)]
        public async Task<ActionResult> Validate([FromBody]JwtToken jwtToken)
        {
            try
            {
                if (jwtToken?.Token == null)
                {
                    throw new ArgumentNullException("No jwtToken provided");
                }

                var validateCommand = new ValidateCommand(jwtToken.Token, NameConstants.AuthenticationServiceValidateCommand);
                bool isValid = await _commandPublisher.Publish<bool>(validateCommand);
                if (!isValid)
                {
                    throw new UnauthorizedAccessException("Validation of JwtToken has failed!");
                }
            }
            catch (TimeoutException)
            {
                _logger.LogError("ValidateCommand resulted in a TimeoutException.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, "Aanvraag kan niet worden verwerkt");
            }
            catch (Exception)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [ExcludeFromCodeCoverage]
#if DEBUG
        [HttpPost]
        [Route("addrole")]
        [SwaggerOperation(
             Summary = "Toevoegen van een rol aan een account",
             OperationId = "AddRole"
         )]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: "Assigning role failed")]
#endif
        public ActionResult AddRole([FromBody]RoleRequest request)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(request.Email))
                {
                    throw new ArgumentNullException("Email is empty");
                }

                var addRoleCommand = new AddRoleCommand(request, NameConstants.AuthenticationServiceAddRoleCommand);
                if (!_commandPublisher.Publish<bool>(addRoleCommand).Result)
                {
                    throw new ApplicationException("Assigning role has failed!");
                }
            }
            catch(Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet]
        [Route("roles")]
        [SwaggerOperation(
            Summary = "Lijst van rollen",
            OperationId = "AllRoles"
        )]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<string>> AllRoles()
        {
            var vals = Enum.GetValues(typeof(Roles));
            var roles = new List<string>();
            foreach(var e in vals)
            {
                roles.Add(e.ToString());
            }

            return Ok(roles);
        }
    }
}
