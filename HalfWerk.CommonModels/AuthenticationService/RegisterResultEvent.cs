using HalfWerk.CommonModels.AuthenticationService.Models;
using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService
{
    public class RegisterResultEvent : DomainEvent
    {
        public RegisterResult Result {get;set;}
        public RegisterResultEvent(RegisterResult result, string routingKey) : base(routingKey)
        {
            Result = result;
        }
    }
}
