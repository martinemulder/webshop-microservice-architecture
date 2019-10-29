using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HalfWerk.AuthenticationService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.AuthenticationService.DAL.DataMappers
{
    public class CredentialDataMapper : ICredentialDataMapper
    {
        private readonly AuthenticationContext _context;
        public CredentialDataMapper(AuthenticationContext context)
        {
            _context = context;
        }

        public IEnumerable<CredentialEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public CredentialEntity GetById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CredentialEntity> Find(Expression<Func<CredentialEntity, bool>> predicate)
        {
            return _context.CredentialEntities.Include(x => x.CredentialRoles).ThenInclude(x => x.Role).Where(predicate).ToList();
        }

        private void InsertRoleIfNonExistant(CredentialEntity entity)
        {
            foreach (var credentialRole in entity.CredentialRoles)
            {
                var role = _context.RoleEntities.FirstOrDefault(x => x.Name == credentialRole.Role.Name);
                if (role == null)
                {
                    _context.RoleEntities.Add(credentialRole.Role);
                }
                else
                {
                    credentialRole.Role = role;
                }
            }
        }

        public CredentialEntity Insert(CredentialEntity entity)
        {
            InsertRoleIfNonExistant(entity);

            CredentialEntity credentialEntity = _context.CredentialEntities.Add(entity).Entity;

            _context.SaveChanges();
            return credentialEntity;
        }

        public void Update(CredentialEntity entity)
        {
            InsertRoleIfNonExistant(entity);

            var removedRelations = _context.CredentialRoleEntities.Where(x => x.CredentialId == entity.Id
                    && entity.CredentialRoles.FirstOrDefault(y => y.Role.Name == x.Role.Name) == null);

            _context.CredentialEntities.Update(entity);
            _context.CredentialRoleEntities.RemoveRange(removedRelations);

            _context.SaveChanges();
        }

        public void Delete(CredentialEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
