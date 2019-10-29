using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HalfWerk.AuthenticationService.DAL.DataMappers
{
    public interface IDataMapper<TEntity, in TKey>
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetById(TKey id);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
