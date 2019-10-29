using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfWerk.BffWebshop.Entities;

namespace HalfWerk.BffWebshop.Test.Entities
{
    [TestClass]
    public class CategorieEntityTest
    {
        [TestMethod]
        public void ShouldCorrectlySetProperties()
        {
            // Arrange        
            // Act
            var categorieEntity = new CategorieEntity()
            {
                Id = 1,
                Categorie = "Cat1",
                ArtikelCategorieen = null
            };

            // Assert
            Assert.AreEqual(1, categorieEntity.Id);
            Assert.AreEqual("Cat1", categorieEntity.Categorie);
            Assert.AreEqual(null, categorieEntity.ArtikelCategorieen);
        }

        [TestMethod]
        public void GetKeyValueShouldReturnCorrectKey()
        {
            // Arrange        
            var categorieEntity = new CategorieEntity()
            {
                Id = 1,
                Categorie = "Cat1",
                ArtikelCategorieen = null
            };

            // Act
            var result = categorieEntity.GetKeyValue();

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
