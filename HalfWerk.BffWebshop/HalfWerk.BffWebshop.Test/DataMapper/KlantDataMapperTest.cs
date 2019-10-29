using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Test.Entities;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HalfWerk.BffWebshop.Test.DataMapper
{
    [TestClass]
    public class KlantDataMapperTest
    {
        private SqliteConnection _connection;
        private DbContextOptions<BffContext> _options;
        private BffContext _context;

        private KlantDataMapper _target;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BffContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new BffContext(_options);
            _target = new KlantDataMapper(_context);

            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Dispose();
        }
         
        [TestMethod]
        public void InsertKlant_ShouldReturnNewKlant()
        {
            // Arrange
            Klant klant = new KlantBuilder().SetDummy().Create();
            klant.Adres = new AdresBuilder().SetDummy().Create();

            // Act
            _target.Insert(klant);
            var result = _context.KlantEntities.Include(x => x.Adres).ToList();

            // Assert
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Kees", result[0].Voornaam);
            Assert.AreEqual("de Koning", result[0].Achternaam);
        }

        [TestMethod]
        public void GetAllKlanten_ShouldReturnKlanten()
        {
            // Arrange
            Klant klant = new KlantBuilder().SetDummy().Create();
            klant.Adres = new AdresBuilder().SetDummy().Create();

            Klant klant2 = new KlantBuilder().SetDummy().Create();
            klant2.Adres = new AdresBuilder().SetDummy().Create();

            _context.KlantEntities.Add(klant);
            _context.KlantEntities.Add(klant2);
            _context.SaveChanges();

            // Act
            var result = _target.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
        }

        [TestMethod]
        public void GetKlantById_ShouldReturnKlantWithSpecifiedId()
        {
            // Arrange
            Klant klant = new KlantBuilder().SetDummy().Create();
            klant.Adres = new AdresBuilder().SetDummy().Create();

            Klant klant2 = new KlantBuilder().SetDummy().Create();
            klant2.Adres = new AdresBuilder().SetDummy().Create();

            _context.KlantEntities.Add(klant);
            _context.KlantEntities.Add(klant2);
            _context.SaveChanges();

            // Act
            var result = _target.GetById(1);

            // Assert
            Assert.AreEqual("de Koning", klant.Achternaam);
        }

        [TestMethod]
        public void Find_ShouldThrowNotImplementedException()
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
        public void Update_ShouldThrowNotImplementedException()
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
        public void Delete_ShouldThrowNotImplementedException()
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

        [TestMethod]
        public void GetByEmail_ShouldReturnKlantOfProvidedEmail()
        {
            // Arrange
            var email = "test@test.nl";

            var klant = new KlantBuilder().SetDummy().SetId(42).SetEmail(email).Create();
            klant.Adres = new AdresBuilder().SetDummy().Create();

            _context.KlantEntities.Add(klant);
            _context.SaveChanges();

            // Act
            var result = _target.GetByEmail(email);

            // Assert
            Assert.AreEqual(klant.Id, result.Id);
        }
    }
}
