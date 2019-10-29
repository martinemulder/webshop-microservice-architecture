using HalfWerk.CommonModels.DsBestelService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.DsBestelService.DAL.DataMappers
{
    public class BestellingDataMapper : IBestellingDataMapper
    {
        private readonly BestelContext _context;

        public BestellingDataMapper(BestelContext context)
        {
            _context = context;
        }

        public IEnumerable<Bestelling> GetAll()
        {
            return _context.Bestellingen
                .Include(x => x.ContactInfo)
                .Include(x => x.Afleveradres)
                .Include(x => x.BestelRegels)
                .ToList();
        }

        public Bestelling GetById(long id)
        {
            return _context.Bestellingen
                .Include(x => x.ContactInfo)
                .Include(x => x.Afleveradres)
                .Include(x => x.BestelRegels)
                .FirstOrDefault(x => x.Id == id);
        }

        public Bestelling GetByFactuurnummer(long factuurnummer)
        {
            return _context.Bestellingen
                .Include(x => x.ContactInfo)
                .Include(x => x.Afleveradres)
                .Include(x => x.BestelRegels)
                .FirstOrDefault(x => x.Factuurnummer == factuurnummer);
        }

        public IEnumerable<Bestelling> Find(Expression<Func<Bestelling, bool>> predicate)
        {
            return _context.Bestellingen
                .Include(x => x.ContactInfo)
                .Include(x => x.Afleveradres)
                .Include(x => x.BestelRegels)
                .Where(predicate).ToList();
        }

        public Bestelling Insert(Bestelling entity)
        {
            if (entity.Klantnummer == 0)
            {
                throw new ArgumentNullException();
            }

            _context.Bestellingen.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Bestelling entity)
        {
            if (entity.Factuurnummer == 0 || entity.Klantnummer == 0)
            {
                throw new ArgumentNullException();
            }

            _context.Bestellingen.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Bestelling entity)
        {
            throw new InvalidOperationException("Deleting bestelling is not allowed");
        }
    }
}
