using HalfWerk.BffWebshop.Controllers;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.BffWebshop.Test.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Artikel = HalfWerk.CommonModels.BffWebshop.Artikel;

namespace HalfWerk.BffWebshop.Test.Controllers
{
    [TestClass]
    public class ArtikelControllerTest
    {
        [TestMethod]
        public void IndexShouldReturnMultipleArtikels()
        {
            // Arrange
            var artikelEntityList = new List<ArtikelEntity>
            {
                new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat1").Create(),
                new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat2").Create()
            };

            var artikelList = new List<Artikel>
            {
                artikelEntityList[0].ToArtikel(),
                artikelEntityList[1].ToArtikel()
            };

            var mock = new Mock<IArtikelDataMapper>();
            mock.Setup(repo => repo.GetAll()).Returns(artikelEntityList);

            var controller = new ArtikelController(mock.Object);

            // Act
            var result = controller.GetArtikelen();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<Artikel>>));

            List<Artikel> artikelResult = result.Value.ToList();
            Assert.AreEqual("Cat1", artikelList[0].Categorieen.ElementAt(0));
            Assert.AreEqual(artikelList[0].Categorieen.ElementAt(0), artikelResult.ElementAt(0).Categorieen.ElementAt(0));

            Assert.AreEqual("Cat2", artikelList[1].Categorieen.ElementAt(0));
            Assert.AreEqual(artikelList[1].Categorieen.ElementAt(0), artikelResult.ElementAt(1).Categorieen.ElementAt(0));
        }

        [TestMethod]
        public void IndexWithIdParameterShouldReturnSingleArticle()
        {
            // Arrange
            ArtikelEntity artikelEntity = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat1").Create();

            var mock = new Mock<IArtikelDataMapper>();
            mock.Setup(repo => repo.GetById(1)).Returns(artikelEntity);

            var controller = new ArtikelController(mock.Object);

            // Act
            var result1 = controller.GetArtikelById(1);
            var result2 = controller.GetArtikelById(2);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(ActionResult<Artikel>));

            Artikel artikelResult = result1.Value;
            Assert.AreEqual("Cat1", artikelEntity.ToArtikel().Categorieen.ElementAt(0));
            Assert.AreEqual(artikelEntity.ToArtikel().Categorieen.ElementAt(0), artikelResult.Categorieen.ElementAt(0));

            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result2, typeof(ActionResult<Artikel>));
            Assert.IsNull(result2.Value);
        }
    }
}
