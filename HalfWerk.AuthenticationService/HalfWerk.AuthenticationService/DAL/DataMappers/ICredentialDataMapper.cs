using HalfWerk.AuthenticationService.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.AuthenticationService.DAL.DataMappers
{
    public interface ICredentialDataMapper : IDataMapper<CredentialEntity, long>
    {
    }
}
