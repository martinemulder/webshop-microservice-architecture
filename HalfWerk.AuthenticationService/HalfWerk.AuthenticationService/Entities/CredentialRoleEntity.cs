using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.AuthenticationService.Entities
{
    public class CredentialRoleEntity
    {
        public long CredentialId { get; set; }
        public CredentialEntity Credential { get; set; }

        public long RoleId { get; set; }
        public RoleEntity Role { get; set; }
    }
}
