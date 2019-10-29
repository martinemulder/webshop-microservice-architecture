using System;
using System.Linq;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.DAL;
using HalfWerk.DsBestelService.DAL.DataMappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalfWerk.DsBestelService.Test.DataMapper
{
    [TestClass]
    public class ArtikelDataMapperTest
    {
        private DbContextOptions<BestelContext> _options;
        private SqliteConnection _connection;
        private BestelContext _bestelContext;

        private ArtikelDataMapper _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BestelContext>()
                .UseSqlite(_connection)
                .Options;

            _bestelContext = new BestelContext(_options);
            _bestelContext.Database.EnsureCreated();

            _target = new ArtikelDataMapper(_bestelContext);
        }

        [TestCleanup]
        public void AfterEach()
        {
            _bestelContext.Dispose();
            _connection.Dispose();
        }

        [TestMethod]
        public void Insert_ShouldInsertNewArtikelIntoTheDatabase()
        {
            var artikel = new Artikel
            {
                Artikelnummer = 1,
                Leveranciercode = "1-abc-2",
                LeverbaarTot = new DateTime(2018, 11, 10),
                Naam = "Fietsband",
                Prijs = 10.99m
            };

            _target.Insert(artikel);

            var result = _bestelContext.Artikelen.SingleOrDefault(b => b.Artikelnummer == 1);
            Assert.AreEqual(artikel.Artikelnummer, result.Artikelnummer);
            Assert.AreEqual(artikel.Leveranciercode, result.Leveranciercode);
            Assert.AreEqual(artikel.LeverbaarTot, result.LeverbaarTot);
            Assert.AreEqual(artikel.Naam, result.Naam);
            Assert.AreEqual(artikel.Prijs, result.Prijs);
        }

        [TestMethod]
        public void GetById_ShouldReturnArtikelOfId()
        {
            var artikel = new Artikel
            {
                Artikelnummer = 1,
                Leveranciercode = "1-abc-2",
                LeverbaarTot = new DateTime(2018, 11, 10),
                Naam = "Fietsband",
                Prijs = 10.99m
            };

            _bestelContext.Artikelen.Add(artikel);
            _bestelContext.SaveChanges();

            var result = _target.GetById(1);

            Assert.AreEqual(artikel.Artikelnummer, result.Artikelnummer);
            Assert.AreEqual(artikel.Leveranciercode, result.Leveranciercode);
            Assert.AreEqual(artikel.LeverbaarTot, result.LeverbaarTot);
            Assert.AreEqual(artikel.Naam, result.Naam);
            Assert.AreEqual(artikel.Prijs, result.Prijs);
        }

        [TestMethod]
        public void GetAll_ShouldThrowNotImplemented()
        {
            // Arrange
            // Act
            Action act = () =>
            {
                _target.GetAll();
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }

        [TestMethod]
        public void Find_ShouldThrowNotImplemented()
        {
            // Arrange
            // Act
            Action act = () =>
            {
                _target.Find(null);
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }

        [TestMethod]
        public void Update_ShouldThrowNotImplemented()
        {
            // Arrange
            // Act
            Action act = () =>
            {
                _target.Update(null);
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }

        [TestMethod]
        public void Delete_ShouldThrowNotImplemented()
        {
            // Arrange
            // Act
            Action act = () =>
            {
                _target.Delete(null);
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }
    }
}