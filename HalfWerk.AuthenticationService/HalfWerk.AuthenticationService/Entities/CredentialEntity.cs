using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.AuthenticationService.Entities
{
    public class CredentialEntity
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<CredentialRoleEntity> CredentialRoles {get;set; }
    }
}
