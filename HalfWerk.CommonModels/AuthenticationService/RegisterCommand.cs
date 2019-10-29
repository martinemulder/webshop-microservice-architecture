using HalfWerk.CommonModels.AuthenticationService.Models;
using Minor.Nijn.WebScale.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService
{
    public class RegisterCommand : DomainCommand
    {
        public Credential Credential { get; set; }

        public RegisterCommand(Credential credential, string routingKey) : base(routingKey)
        {
            Credential = credential;
        }
    }
}
