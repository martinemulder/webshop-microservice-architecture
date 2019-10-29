using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace HalfWerk.BffWebshop.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        public bool GetJwtToken(HttpContext httpContext, out JwtSecurityToken jwtToken)
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues value);
                if (value.Count != 1)
                {
                    throw new Exception();
                }

                string jwtString = value[0].Replace("Bearer ", "");
                jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtString);
            }
            catch
            {
                jwtToken = null;
                return false;
            }

            return true;
        }

        public string GetEmail(HttpContext httpContext)
        {
            if(httpContext == null)
            {
                return null;
            }

            if(!GetJwtToken(httpContext, out JwtSecurityToken jwtToken))
            {
                throw new SecurityTokenException("Failed to retrieve JwtToken from HttpContext");
            }

            return jwtToken.Subject;
        }
    }
}
