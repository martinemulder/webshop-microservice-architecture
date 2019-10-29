using HalfWerk.BffWebshop.DAL;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.BffWebshop.DataMapper
{
    public class BestellingDataMapper : IBestellingDataMapper
    {
        private readonly BffContext _context;

        public BestellingDataMapper(BffContext context)
        {
            _context = context;
        }

        public IEnumerable<BevestigdeBestelling> GetAll()
        {
            return _context.BevestigdeBestellingen
                .Include(b => b.ContactInfo)
                .Include(b => b.Afleveradres)
                .Include(b => b.BestelRegels)
                .ToList();
        }

        public BevestigdeBestelling GetById(long id)
        {
            return _context.BevestigdeBestellingen
                .Include(b => b.ContactInfo)
                .Include(b => b.Afleveradres)
                .Include(b => b.BestelRegels)
                .FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<BevestigdeBestelling> Find(Expression<Func<BevestigdeBestelling, bool>> predicate)
        {
            return _context.BevestigdeBestellingen
                .Include(b => b.ContactInfo)
                .Include(b => b.Afleveradres)
                .Include(b => b.BestelRegels)
                .Where(predicate).ToList();
        }

        public BevestigdeBestelling Insert(BevestigdeBestelling entity)
        {
            _context.BevestigdeBestellingen.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(BevestigdeBestelling entity)
        {
            _context.BevestigdeBestellingen.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(BevestigdeBestelling entity)
        {
            throw new InvalidOperationException("Deleting a bestelling is not allowed.");
        }

        public BevestigdeBestelling GetByFactuurnummer(long factuurnummer)
        {
            return _context.BevestigdeBestellingen
                .Include(b => b.ContactInfo)
                .Include(b => b.Afleveradres)
                .Include(b => b.BestelRegels)
                .FirstOrDefault(b => b.Factuurnummer == factuurnummer);
        }
    }
}
