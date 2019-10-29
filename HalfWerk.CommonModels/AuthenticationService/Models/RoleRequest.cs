using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.CommonModels.AuthenticationService.Models
{
    public class RoleRequest
    {
        public string Email { get; set; }
        public Roles Role { get; set; }
    }
}
