using HalfWerk.CommonModels.PcsBetalingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.PcsBetaalService.DAL.DataMappers
{
    public class BetalingDataMapper : IBetalingDataMapper
    {
        private readonly BetaalContext _context;

        public BetalingDataMapper(BetaalContext context)
        {
            _context = context;
        }

        public void Delete(Betaling entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Betaling> Find(Expression<Func<Betaling, bool>> predicate)
        {
            return _context.Betalingen.Where(predicate).ToList();
        }

        public IEnumerable<Betaling> GetAll()
        {
            return _context.Betalingen.ToList();
        }

        public Betaling GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Betaling Insert(Betaling entity)
        {
            _context.Betalingen.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Betaling entity)
        {
            throw new NotImplementedException();
        }
    }
}
