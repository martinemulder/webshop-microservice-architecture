using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using Microsoft.EntityFrameworkCore;

namespace HalfWerk.PcsBetaalService.DAL.DataMappers
{
    public class BestellingDataMapper : IBestellingDataMapper
    {
        private readonly BetaalContext _context;

        public BestellingDataMapper(BetaalContext context)
        {
            _context = context;
        }

        public void Delete(Bestelling entity)
        {
            throw new NotImplementedException();
        }

        public Bestelling GetByFactuurnummer(long factuurnummer)
        {
            return _context.Bestellingen.Include(x => x.BestelRegels).FirstOrDefault(x => x.Factuurnummer == factuurnummer);
        }

        public IEnumerable<Bestelling> Find(Expression<Func<Bestelling, bool>> predicate)
        {
            return _context.Bestellingen.Include(x => x.BestelRegels).Where(predicate).ToList();
        }

        public IEnumerable<Bestelling> GetAll()
        {
            return _context.Bestellingen.Include(b => b.BestelRegels).ToList();
        }

        public Bestelling GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Bestelling Insert(Bestelling entity)
        {
            _context.Bestellingen.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Bestelling entity)
        {
            throw new NotImplementedException();
        }
    }
}
