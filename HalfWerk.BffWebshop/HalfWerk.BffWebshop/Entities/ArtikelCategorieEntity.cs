using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Entities
{
    public class ArtikelCategorieEntity : IEntity<Tuple<long, long>>
    {
        public long ArtikelId { get; set; }
        public ArtikelEntity Artikel { get; set; }

        public int CategorieId { get; set; }
        public CategorieEntity Categorie { get; set; }

        public Tuple<long, long> GetKeyValue()
        {
            return new Tuple<long, long>(ArtikelId, CategorieId);
        }
    }
}
