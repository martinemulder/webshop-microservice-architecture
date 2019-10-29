using HalfWerk.CommonModels.AuthenticationService.Models;
using Minor.Nijn.WebScale.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService
{
    public class AddRoleCommand : DomainCommand
    {
        public RoleRequest RoleRequest { get; set; }
        public AddRoleCommand(RoleRequest roleRequest, string routingKey) : base(routingKey)
        {
            RoleRequest = roleRequest;
        }
    }
}
