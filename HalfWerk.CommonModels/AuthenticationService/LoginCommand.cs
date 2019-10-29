using HalfWerk.CommonModels.AuthenticationService.Models;
using Minor.Nijn.WebScale.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService
{
    public class LoginCommand : DomainCommand
    {
        public Credential Credential { get; set; }

        public LoginCommand(Credential credential, string routingKey) : base(routingKey)
        {
            Credential = credential;
        }
    }
}
