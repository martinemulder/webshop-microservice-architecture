using HalfWerk.BffWebshop.DAL;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.BffWebshop.DataMapper
{
    public class KlantDataMapper : IKlantDataMapper
    {
        private readonly BffContext _context;

        public KlantDataMapper(BffContext context)
        {
            _context = context;
        }

        public void Delete(Klant entity)
        {
            throw new NotImplementedException();
        }

        public Klant GetByEmail(string email)
        {
            return _context.KlantEntities.Include(x => x.Adres).SingleOrDefault(k => k.Email == email);
        }

        public IEnumerable<Klant> Find(Expression<Func<Klant, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Klant> GetAll()
        {
             return _context.KlantEntities.Include(x => x.Adres).ToList();
        }

        public Klant GetById(long id)
        {
            return _context.KlantEntities.Find(id);
        }

        public Klant Insert(Klant entity)
        {
            _context.KlantEntities.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Klant entity)
        {
            throw new NotImplementedException();
        }
    }
}
