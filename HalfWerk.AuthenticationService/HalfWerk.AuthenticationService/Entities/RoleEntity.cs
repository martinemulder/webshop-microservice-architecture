using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.AuthenticationService.Entities
{
    public class RoleEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CredentialRoleEntity> CredentialRoles { get; set; }
    }
}
