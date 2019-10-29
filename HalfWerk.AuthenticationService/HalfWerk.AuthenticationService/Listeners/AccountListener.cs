using HalfWerk.AuthenticationService.DAL.DataMappers;
using HalfWerk.AuthenticationService.Models;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.AuthenticationService;
using HalfWerk.CommonModels.CommonExceptions;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Attributes;
using Minor.Nijn.WebScale.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HalfWerk.CommonModels.AuthenticationService.Models;
using HalfWerk.AuthenticationService.Entities;
using System.Security.Cryptography;

namespace HalfWerk.AuthenticationService.Listeners
{
    [CommandListener]
    public class AccountListener
    {
        private readonly ILogger<AccountListener> _logger;
        private readonly ICredentialDataMapper _credentialDataMapper;

        public AccountListener(ICredentialDataMapper credentialDataMapper, ILoggerFactory loggerFactory)
        {
            _credentialDataMapper = credentialDataMapper;
            _logger = loggerFactory.CreateLogger<AccountListener>();
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

        [Command(NameConstants.AuthenticationServiceLoginCommand)]
        public string Login(LoginCommand request)
        {
            string jwtToken = "";
            var credential =  request.Credential;
            credential.Password = GenerateSha256(credential.Password);

            try
            {
                var user = _credentialDataMapper.Find(x => x.Email.ToLower().CompareTo(credential.Email.ToLower()) == 0
                    && x.Password == credential.Password).FirstOrDefault();

                if (user != null)
                {
                    var roleList = new List<string>();
                    foreach(var role in user.CredentialRoles)
                    {
                        roleList.Add(role.Role.Name);
                    }

                    jwtToken = new JSONWebToken(user.Id, user.Email, roleList).Token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with email: {0}", credential?.Email);
                _logger.LogDebug(
                    "DB exception occured with email {}, it threw exception: {}. Inner exception: {}",
                    credential?.Email, ex.Message, ex.InnerException?.Message
                );
                throw new DatabaseException("Something unexpected happened while searching through the database");
            }

            return jwtToken;
        }

        [Command(NameConstants.AuthenticationServiceRegisterCommand)]
        public RegisterResult Register(RegisterCommand request)
        {
            var registerResult = RegisterResult.Unknown;
            var credential = request.Credential;
            credential.Password = GenerateSha256(credential.Password);

            try
            {
                var user = _credentialDataMapper.Find(x => x.Email.ToLower().CompareTo(credential.Email.ToLower()) == 0).FirstOrDefault();
                if (user != null)
                {
                    registerResult = RegisterResult.EmailInUse;
                }
                else
                {
                    var credentialRoleEntity = new CredentialRoleEntity()
                    {
                        Credential = new CredentialEntity()
                        {
                            Email = credential.Email,
                            Password = credential.Password,
                            CredentialRoles = new HashSet<CredentialRoleEntity>()
                        },

                        Role = new RoleEntity()
                        {
                            Name = Roles.Klant.ToString(),
                            CredentialRoles = new HashSet<CredentialRoleEntity>()
                        }
                    };

                    credentialRoleEntity.Credential.CredentialRoles.Add(credentialRoleEntity);
                    credentialRoleEntity.Role.CredentialRoles.Add(credentialRoleEntity);

                    _credentialDataMapper.Insert(credentialRoleEntity.Credential);

                    registerResult = RegisterResult.Ok;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with email: {0}", credential?.Email);
                _logger.LogDebug(
                    "DB exception occured with email {}, it threw exception: {}. Inner exception: {}",
                    credential?.Email, ex.Message, ex.InnerException?.Message
                );

                throw new DatabaseException("Something unexpected happened while inserting in the database");
            }

            return registerResult;
        }

        [Command(NameConstants.AuthenticationServiceValidateCommand)]
        public bool Validate(ValidateCommand request)
        {
            return new JSONWebToken(request?.JwtToken).IsValid();
        }

        [Command(NameConstants.AuthenticationServiceAddRoleCommand)]
        public bool AddRole(AddRoleCommand request)
        {
            var roleRequest = request.RoleRequest;
            try
            {
                var user = _credentialDataMapper.Find(x => x.Email.ToLower().CompareTo(roleRequest.Email.ToLower()) == 0).FirstOrDefault();
                if (user == null)
                {
                    throw new NullReferenceException("Email not found in the database");
                }
                else
                {
                    user.CredentialRoles.Add(new CredentialRoleEntity()
                    {
                        Credential = user,
                        Role = new RoleEntity()
                        {
                            Name = roleRequest.Role.ToString(),
                            CredentialRoles = new HashSet<CredentialRoleEntity>()
                        }
                    });
               
                    _credentialDataMapper.Update(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DB exception occured with email: {0}", roleRequest?.Email);
                _logger.LogDebug(
                    "DB exception occured with email {}, it threw exception: {}. Inner exception: {}",
                    roleRequest?.Email, ex.Message, ex.InnerException?.Message
                );

                throw new DatabaseException("Something unexpected happened while updating in the database");
            }

            return true;
        }
    }
}
