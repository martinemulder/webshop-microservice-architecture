using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HalfWerk.AuthenticationService.Models
{
    public class JSONWebToken
    {
        private const string _issuer = "issuer.org";
        private const string _audience = "issuer.org";

        public string Token { get; }
        public JSONWebToken(long id, string email, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            Token = new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JSONWebToken(string token)
        {
            Token = token;
        }

        public bool IsValid()
        {
            var handler = new JwtSecurityTokenHandler();
            var validationsParameters = new TokenValidationParameters()
            {
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.Secret))
            };

            try
            {
                handler.ValidateToken(Token, validationsParameters, out SecurityToken token);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
