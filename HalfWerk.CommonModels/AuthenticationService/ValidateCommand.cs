using Minor.Nijn.WebScale.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService
{
    public class ValidateCommand : DomainCommand
    {
        public string JwtToken { get; set; }

        public ValidateCommand(string token, string routingKey) : base(routingKey)
        {
            JwtToken = token;
        }
    }
}
