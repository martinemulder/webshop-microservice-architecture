using HalfWerk.CommonModels;
using HalfWerk.CommonModels.AuthenticationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Minor.Nijn.WebScale.Commands;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Helpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JwtInRole : Attribute, IAuthorizationFilter
    {
        private readonly IEnumerable<string> _roles;

        public JwtInRole(string role, params string[] roles)
        {
            _roles = new List<string>(roles) { role };
        }

        private bool IsAccessAllowed(JwtSecurityToken jwtToken)
        {
            var roles = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role);
            foreach (var role in roles)
            {
                if (_roles.Count(x => x == role.Value) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ILoggerFactory loggerFactory = (ILoggerFactory)context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory));
            var logger = loggerFactory?.CreateLogger<JwtInRole>();

            try
            {
                var commandPublisher = (ICommandPublisher)context.HttpContext.RequestServices.GetService(typeof(ICommandPublisher));
                if(commandPublisher == null)
                {
                    throw new NullReferenceException("Dependency injection of commandPublisher has failed");
                }

                if(!new JwtHelper().GetJwtToken(context.HttpContext, out JwtSecurityToken jwtToken))
                {
                    throw new UnauthorizedAccessException("Incorrect or no JwtToken provided");
                }

                // Do not even attempt to validate the jwtToken if user doesn't have the required role
                if (!IsAccessAllowed(jwtToken))
                {
                    throw new UnauthorizedAccessException("User doesn't have one of the required roles");
                }

                var validateCommand = new ValidateCommand(jwtToken.RawData, NameConstants.AuthenticationServiceValidateCommand);
                if (!commandPublisher.Publish<bool>(validateCommand).Result)
                {
                    throw new UnauthorizedAccessException("Validation of JwtToken has failed!");
                }
            }
            catch (TimeoutException)
            {
                logger?.LogError("OnAuthorization resulted in a TimeoutException.");
                context.Result = new StatusCodeResult((int)HttpStatusCode.RequestTimeout);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger?.LogDebug(
                    "UnauthorizedAccessException occured during execution of OnAuthorization, it threw exception: {}. Inner exception: {}",
                    ex.Message, ex.InnerException?.Message
                );

                context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {

                logger?.LogError("OnAuthorization resulted in an internal server error.");
                logger?.LogDebug(
                    "Exception occured during execution of OnAuthorization, it threw exception: {}. Inner exception: {}",
                    ex.Message, ex.InnerException?.Message
                );

                context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
        }
    }
}
