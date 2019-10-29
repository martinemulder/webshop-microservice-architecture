using HalfWerk.AuthenticationService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HalfWerk.AuthenticationService.Test.Entities
{
    public static class CredentialEntityExtension
    {
        public static bool IsEqual(this CredentialEntity entity, CredentialEntity comparable)
        {
            if (entity.GetHashCode() != comparable.GetHashCode()) return false;

            if (entity.Id != comparable.Id) return false;
            if (entity.Email != comparable.Email) return false;
            if (entity.Password != comparable.Password) return false;
            if (entity.CredentialRoles.Count != comparable.CredentialRoles.Count) return false;

            var entityListList = entity.CredentialRoles.OrderBy(x => x.Role.Name);
            var comparableRoleList = comparable.CredentialRoles.OrderBy(x => x.Role.Name);

            for (int i = 0; i < entityListList.Count(); ++i)
            {
                if (entityListList.ElementAt(i)?.Role.Name !=
                        comparableRoleList.ElementAt(i)?.Role.Name) return false;
            }

            return true;
        }
    }
}
