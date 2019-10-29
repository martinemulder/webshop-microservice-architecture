using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HalfWerk.CommonModels.DsBestelService.Models;

namespace HalfWerk.DsBestelService.DAL.DataMappers
{
    public class ArtikelDataMapper : IArtikelDataMapper
    {
        private readonly BestelContext _context;

        public ArtikelDataMapper(BestelContext context)
        {
            _context = context;
        }

        public IEnumerable<Artikel> GetAll()
        {
            throw new NotImplementedException();
        }

        public Artikel GetById(long id)
        {
            return _context.Artikelen.SingleOrDefault(a => a.Artikelnummer == id);
        }

        public IEnumerable<Artikel> Find(Expression<Func<Artikel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Artikel Insert(Artikel entity)
        {
            _context.Artikelen.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(Artikel entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Artikel entity)
        {
            throw new NotImplementedException();
        }
    }
}