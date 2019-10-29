using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.BffWebshop.DataMapper
{
    public class ArtikelDataMapper : IArtikelDataMapper
    {
        private readonly BffContext _context;

        public ArtikelDataMapper(BffContext context)
        {
            _context = context;
        }

        public IEnumerable<ArtikelEntity> GetAll()
        {
            return _context.ArtikelEntities.Include(x => x.ArtikelCategorieen).ThenInclude(x => x.Categorie).ToList();
        }

        public ArtikelEntity GetById(int id)
        {
             return _context.ArtikelEntities.Include(x => x.ArtikelCategorieen).ThenInclude(x => x.Categorie).FirstOrDefault(x => x.GetKeyValue() == id);
        }

        public IEnumerable<ArtikelEntity> Find(Expression<Func<ArtikelEntity, bool>> predicate)
        {
            return _context.ArtikelEntities.Include(x => x.ArtikelCategorieen).ThenInclude(x => x.Categorie).Where(predicate).ToList();
        }

        public ArtikelEntity Insert(ArtikelEntity entity)
        {
            InsertCategoryIfNonExistent(entity);
            _context.ArtikelEntities.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(ArtikelEntity entity)
        {
            InsertCategoryIfNonExistent(entity);

            var removedRelations = _context.ArtikelCategorieEntities.Where(x => x.ArtikelId == entity.Artikelnummer
                    && entity.ArtikelCategorieen.FirstOrDefault(y => y.Categorie.Categorie == x.Categorie.Categorie) == null);

            _context.ArtikelEntities.Update(entity);
            _context.ArtikelCategorieEntities.RemoveRange(removedRelations);

            _context.SaveChanges();
        }

        public void Delete(ArtikelEntity entity)
        {
            var removedRelations = _context.ArtikelCategorieEntities.Where(x => x.ArtikelId == entity.Artikelnummer);
            _context.ArtikelCategorieEntities.RemoveRange(removedRelations);
            _context.ArtikelEntities.Remove(entity);
            _context.SaveChanges();
        }

        private void InsertCategoryIfNonExistent(ArtikelEntity entity)
        {
            foreach (var artikelcategorie in entity.ArtikelCategorieen)
            {
                var category = _context.CategorieEntities.FirstOrDefault(x => x.Categorie == artikelcategorie.Categorie.Categorie);
                if (category == null)
                {
                    _context.CategorieEntities.Add(artikelcategorie.Categorie);
                }
                else
                {
                    artikelcategorie.Categorie = category;
                }
            }
        }
    }
}
