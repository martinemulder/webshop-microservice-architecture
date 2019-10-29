using HalfWerk.BffWebshop.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HalfWerk.BffWebshop.Test.Entities
{
    [TestClass]
    public class ArtikelCategorieEntityTest
    {
        [TestMethod]
        public void ShouldCorrectlySetProperties()
        {
            // Arrange        
            // Act
            var artikelCategorieEntity = new ArtikelCategorieEntity()
            {
                ArtikelId = 44,
                CategorieId = 2,
            };

            // Assert
            Assert.AreEqual(44, artikelCategorieEntity.ArtikelId);
            Assert.AreEqual(null, artikelCategorieEntity.Artikel);
            Assert.AreEqual(2, artikelCategorieEntity.CategorieId);
            Assert.AreEqual(null, artikelCategorieEntity.Categorie);
        }

        [TestMethod]
        public void GetKeyValueShouldReturnCorrectKey()
        {
            // Arrange        
            var artikelCategorieEntity = new ArtikelCategorieEntity()
            {
                ArtikelId = 44,
                CategorieId = 2,
            };

            var expected = new Tuple<long, long>(44, 2);

            // Act
            var result = artikelCategorieEntity.GetKeyValue();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
