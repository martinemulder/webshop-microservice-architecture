using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.BffWebshop.DataMapper
{
    public class MagazijnSessionDataMapper : IMagazijnSessionDataMapper
    {
        private readonly BffContext _context;

        public MagazijnSessionDataMapper(BffContext context)
        {
            _context = context;
        }

        public IEnumerable<MagazijnSessionEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public MagazijnSessionEntity GetById(long id)
        {
            throw new NotImplementedException();
        }

        public MagazijnSessionEntity GetByFactuurnummer(long factuurnummer)
        {
            return _context.MagazijnSessions.SingleOrDefault(s => s.Factuurnummer == factuurnummer);
        }

        public IEnumerable<MagazijnSessionEntity> Find(Expression<Func<MagazijnSessionEntity, bool>> predicate)
        {
            return _context.MagazijnSessions.Where(predicate).ToList();
        }

        public MagazijnSessionEntity Insert(MagazijnSessionEntity entity)
        {
            _context.MagazijnSessions.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(MagazijnSessionEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(MagazijnSessionEntity entity)
        {
            _context.MagazijnSessions.Remove(entity);
            _context.SaveChanges();
        }
    }
}