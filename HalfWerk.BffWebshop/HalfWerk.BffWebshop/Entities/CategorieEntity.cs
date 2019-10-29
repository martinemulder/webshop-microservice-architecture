using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Entities
{
    public class CategorieEntity : IEntity<int>
    {
        public int Id { get; set; }
        public string Categorie { get; set; }
        public virtual ICollection<ArtikelCategorieEntity> ArtikelCategorieen { get; set; }

        public int GetKeyValue()
        {
            return Id;
        }
    }
}
