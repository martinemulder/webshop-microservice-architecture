using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HalfWerk.CommonModels.DsKlantBeheer.Models;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.DsKlantBeheer.DAL.DataMappers
{
    public class KlantDataMapper : IKlantDataMapper
    {
        private readonly KlantContext _context;

        public KlantDataMapper(KlantContext context)
        {
            _context = context;
        }

        public IEnumerable<Klant> GetAll()
        {
            return _context.Klanten.Include(x => x.Adres).ToList();
        }

        public Klant GetById(long id)
        {
           return _context.Klanten.Include(x => x.Adres).FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Klant> Find(Expression<Func<Klant, bool>> predicate)
        {
            return _context.Klanten.Include(x => x.Adres).Where(predicate).ToList();
        }

        public Klant Insert(Klant entity)
        {
            _context.Klanten.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Klant entity)
        {
            _context.Klanten.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Klant entity)
        {
            throw new InvalidOperationException("Deleting klanten is not allowed");
        }
    }
}
