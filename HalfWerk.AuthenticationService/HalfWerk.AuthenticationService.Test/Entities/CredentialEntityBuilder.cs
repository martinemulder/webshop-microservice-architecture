using HalfWerk.AuthenticationService.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.AuthenticationService.Test.Entities
{
    public class CredentialEntityBuilder
    {
        private long Id { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }
        private ICollection<CredentialRoleEntity> CredentialRoles { get; set; } = new HashSet<CredentialRoleEntity>();

        public CredentialEntityBuilder SetId(long n) { Id = n; return this; }
        public CredentialEntityBuilder SetEmail(string s) { Email = s; return this; }
        public CredentialEntityBuilder SetPassword(string s) { Password = s; return this; }
        public CredentialEntityBuilder SetCredentialRoles(ICollection<CredentialRoleEntity> c) { CredentialRoles = c; return this; }

        public CredentialEntityBuilder SetDummy()
        {
            Id = DateTime.Now.Ticks.GetHashCode();
            Email = "hans@worstmail.com";
            Password = "Geheim101";

            return this;
        }

        public CredentialEntityBuilder SetDummyRole(string s)
        {
            CredentialRoles.Add(new CredentialRoleEntity()
            {
                Role = new RoleEntity()
                {
                    Id = DateTime.Now.Ticks.GetHashCode(),
                    Name = s
                },
            });

            return this;
        }

        public CredentialEntity Create()
        {
            var credEntity = new CredentialEntity()
            {
                Id = Id,
                Email = Email,
                Password = Password,
                CredentialRoles = CredentialRoles
            };


            foreach(var cr in credEntity.CredentialRoles)
            {
                cr.Credential = credEntity;
                cr.CredentialId = credEntity.Id;
                cr.RoleId = cr.RoleId;
            }

            return credEntity;
        }
    }
}
