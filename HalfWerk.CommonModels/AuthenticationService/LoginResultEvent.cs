using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService
{
    public class LoginResultEvent : DomainEvent
    {
        public string JwtToken { get; set; }

        public LoginResultEvent(string token, string routingKey) : base(routingKey)
        {
            JwtToken = token;
        }
    }
}
