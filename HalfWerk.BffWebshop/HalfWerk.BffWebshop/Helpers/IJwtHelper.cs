using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace HalfWerk.BffWebshop.Helpers
{
    public interface IJwtHelper
    {
        bool GetJwtToken(HttpContext httpContext, out JwtSecurityToken jwtToken);
        string GetEmail(HttpContext httpContext);
    }
}