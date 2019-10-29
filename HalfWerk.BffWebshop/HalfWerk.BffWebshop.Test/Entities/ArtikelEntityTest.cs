using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HalfWerk.BffWebshop.Test.Entities
{
    [TestClass]
    public class ArtikelEntityTest
    {
        [TestMethod]
        public void ShouldCorrectlySetProperties()
        {
            // Arrange        
            var curTime = DateTime.Now;

            // Act
            var artikelEntity = new ArtikelEntityBuilder()
                .SetArtikelnummer(1)
                .SetNaam("Artikel")
                .SetPrijs(12)
                .SetAfbeeldingUrl("Dummy1")
                .SetLeverbaarVanaf(curTime)
                .SetLeverbaarTot(curTime.AddDays(1))
                .SetLeveranciercode("Dummy2")
                .SetLeverancier("Dummy3")
                .SetDummyCategorie("Cat1")
                .Create();

            // Assert
            Assert.AreEqual(1, artikelEntity.Artikelnummer);
            Assert.AreEqual("Artikel", artikelEntity.Naam);
            Assert.AreEqual(12, artikelEntity.Prijs);
            Assert.AreEqual("Dummy1", artikelEntity.AfbeeldingUrl);
            Assert.AreEqual(curTime, artikelEntity.LeverbaarVanaf);
            Assert.AreEqual(curTime.AddDays(1), artikelEntity.LeverbaarTot);
            Assert.AreEqual("Dummy2", artikelEntity.Leveranciercode);
            Assert.AreEqual("Dummy3", artikelEntity.Leverancier);
            Assert.AreEqual("Cat1", artikelEntity.ArtikelCategorieen.ElementAt(0).Categorie.Categorie);
        }

        [TestMethod]
        public void ToArtikelShouldReturnExpectedArtikel()
        {
            // Arrange        
            var curTime = DateTime.Now;

            // Act
            var artikel = new ArtikelEntityBuilder()
                .SetArtikelnummer(1)
                .SetNaam("Artikel")
                .SetPrijs(12)
                .SetAfbeeldingUrl("Dummy1")
                .SetLeverbaarVanaf(curTime)
                .SetLeverbaarTot(curTime.AddDays(1))
                .SetLeveranciercode("Dummy2")
                .SetLeverancier("Dummy3")
                .SetDummyCategorie("Cat1")
                .SetDummyCategorie("Cat2")
                .Create()
                .ToArtikel();

            // Assert
            Assert.AreEqual(1, artikel.Artikelnummer);
            Assert.AreEqual("Artikel", artikel.Naam);
            Assert.AreEqual(12, artikel.Prijs);
            Assert.AreEqual("Dummy1", artikel.AfbeeldingUrl);
            Assert.AreEqual(curTime, artikel.LeverbaarVanaf);
            Assert.AreEqual(curTime.AddDays(1), artikel.LeverbaarTot);
            Assert.AreEqual("Dummy2", artikel.Leveranciercode);
            Assert.AreEqual("Dummy3", artikel.Leverancier);
            Assert.AreEqual("Cat1", artikel.Categorieen.First());
            Assert.AreEqual("Cat2", artikel.Categorieen.Last());
        }

        [TestMethod]
        public void GetKeyValueShouldReturnCorrectKey()
        {
            // Arrange        
            var artikelEntity = new ArtikelEntityBuilder()
                .SetArtikelnummer(145)
                .Create();

            // Act
            var result = artikelEntity.GetKeyValue();

            // Assert
            Assert.AreEqual(145, result);
        }
    }
}
