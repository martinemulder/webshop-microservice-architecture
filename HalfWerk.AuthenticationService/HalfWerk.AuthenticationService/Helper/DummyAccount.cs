using HalfWerk.AuthenticationService.DAL.DataMappers;
using HalfWerk.AuthenticationService.Entities;
using HalfWerk.CommonModels.AuthenticationService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

/* Temporary allow DummyAccount in Release */
//#if DEBUG
namespace HalfWerk.AuthenticationService.Helper
{
#if RELEASE
    #warning DummyAccounts are enabled. Security Risk!
#endif

    [ExcludeFromCodeCoverage]
    public class DummyAccount
    {
        private readonly ICredentialDataMapper _credentialDataMapper;

        public DummyAccount(ICredentialDataMapper dataMapper)
        {
            _credentialDataMapper = dataMapper;
        }

        private string GenerateSha256(string s)
        {
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(s + Program.PasswordSalt)
                )).Replace("-", "");
            }
        }

        private CredentialEntity AddRole(CredentialEntity entity, Roles role)
        {
            var credentialRoleEntity = new CredentialRoleEntity()
            {
                Credential = entity,
                Role = new RoleEntity()
                {
                    Name = role.ToString(),
                    CredentialRoles = new HashSet<CredentialRoleEntity>()
                }
            };

            credentialRoleEntity.Credential.CredentialRoles.Add(credentialRoleEntity);
            credentialRoleEntity.Role.CredentialRoles.Add(credentialRoleEntity);

            _credentialDataMapper.Update(entity);
            return entity;
        }

        private CredentialEntity CreateAccount(Credential credential, Roles role)
        {
            var Credential = _credentialDataMapper.Insert(new CredentialEntity()
            {
                Email = credential.Email,
                Password = GenerateSha256(credential.Password),
                CredentialRoles = new HashSet<CredentialRoleEntity>()
            });

            return AddRole(Credential, role);
        }

        public void CreateKlant()
        {
            CreateAccount(new Credential() { Email = "klant", Password = "klant" }, Roles.Klant);
        }
        public void CreateMagazijn()
        {
            CreateAccount(new Credential() { Email = "magazijn1", Password = "magazijn1" }, Roles.Magazijn);
            CreateAccount(new Credential() { Email = "magazijn2", Password = "magazijn2" }, Roles.Magazijn);
        }
        public void CreateSales()
        {
            CreateAccount(new Credential() { Email = "sales", Password = "sales" }, Roles.Sales);
        }
        public void CreateEigenaar()
        {
            CreateAccount(new Credential() { Email = "eigenaar", Password = "eigenaar" }, Roles.Eigenaar);
        }

        public void CreateHolyGrail()
        {
            var acc = CreateAccount(new Credential() { Email = "holygrail", Password = "holygrail" }, Roles.Eigenaar);
            AddRole(acc, Roles.Sales);
            AddRole(acc, Roles.Magazijn);
            AddRole(acc, Roles.Klant);
        }
    }
}
//#endif