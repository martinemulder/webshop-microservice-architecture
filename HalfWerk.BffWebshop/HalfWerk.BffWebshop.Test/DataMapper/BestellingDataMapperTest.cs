using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Test.Entities.Bestellingen;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using BestelStatus = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelStatus;

namespace HalfWerk.BffWebshop.Test.DataMapper
{
    [TestClass]
    public class BestellingDataMapperTest
    {
        private SqliteConnection _connection;
        private DbContextOptions<BffContext> _options;
        private BffContext _context;

        private BestellingDataMapper _target;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BffContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new BffContext(_options);
            _target = new BestellingDataMapper(_context);

            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllArtikelen()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling1 = new BevestigdeBestellingBuilder().SetDummy().Create();
            BevestigdeBestelling bevestigdeBestelling2 = new BevestigdeBestellingBuilder().SetDummy().Create();

            _context.BevestigdeBestellingen.Add(bevestigdeBestelling1);
            _context.BevestigdeBestellingen.Add(bevestigdeBestelling2);
            _context.SaveChanges();

            // Act
            List<BevestigdeBestelling> result = _target.GetAll().OrderBy(r => r.Besteldatum).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(bevestigdeBestelling1.IsEqual(result[0]));
            Assert.IsTrue(bevestigdeBestelling2.IsEqual(result[1]));
        }

        [TestMethod]
        public void GetById_Should_ReturnBevestigdeBestellingWithCertainId_When_Id_IsGiven()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling = new BevestigdeBestellingBuilder().SetDummy().Create();

            _context.BevestigdeBestellingen.Add(bevestigdeBestelling);
            _context.SaveChanges();

            // Act
            BevestigdeBestelling result = _target.GetById(bevestigdeBestelling.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(bevestigdeBestelling.IsEqual(result));
        }

        [TestMethod]
        public void GetByFactuurnummer_Should_ReturnBevestigdeBestellingWithCertainId_When_Factuurnummer_IsGiven()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling = new BevestigdeBestellingBuilder().SetDummy().Create();

            _context.BevestigdeBestellingen.Add(bevestigdeBestelling);
            _context.SaveChanges();

            // Act
            BevestigdeBestelling result = _target.GetByFactuurnummer(bevestigdeBestelling.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(bevestigdeBestelling.IsEqual(result));
        }

        [TestMethod]
        public void Find_Should_ReturnArtikelWithCertainPredicate_When_Predicate_IsGiven()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling1 = new BevestigdeBestellingBuilder().SetDummy().Create();
            BevestigdeBestelling bevestigdeBestelling2 = new BevestigdeBestellingBuilder().SetDummy()
                .SetBestelStatus(BestelStatus.Goedgekeurd).Create();

            _context.BevestigdeBestellingen.Add(bevestigdeBestelling1);
            _context.BevestigdeBestellingen.Add(bevestigdeBestelling2);
            _context.SaveChanges();

            // Act
            List<BevestigdeBestelling> result = _target.Find(b => b.BestelStatus == BestelStatus.Goedgekeurd)
                .ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(bevestigdeBestelling2.IsEqual(result[0]));
        }

        [TestMethod]
        public void Insert_Should_InsertArtikel_When_NieuwArtikel_IsGiven()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling = new BevestigdeBestellingBuilder().SetDummy().Create();

            // Act
            _target.Insert(bevestigdeBestelling);

            BevestigdeBestelling result = _target.GetByFactuurnummer(bevestigdeBestelling.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(bevestigdeBestelling.IsEqual(result));
        }

        [TestMethod]
        public void Insert_ShouldNotInsertArtikelInDatabase_When_AnExistingFactuurnummer_IsGiven()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling1 = new BevestigdeBestellingBuilder().SetDummy()
                .SetFactuurnummer(1)
                .Create();
            BevestigdeBestelling bevestigdeBestelling2 = new BevestigdeBestellingBuilder().SetDummy()
                .SetFactuurnummer(1)
                .Create();

            // Act
            _target.Insert(bevestigdeBestelling1);

            void Act()
            {
                _target.Insert(bevestigdeBestelling2);
            }

            // Assert
            Assert.ThrowsException<InvalidOperationException>((Action) Act);
        }

        [TestMethod]
        public void Update_ShouldUpdateBevestigdeBestelling_When_AnUpdatedBevestigdeBestelling_IsGiven()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling = new BevestigdeBestellingBuilder().SetDummy().Create();
            BevestigdeBestelling bevestigdeBestellingUpdate = bevestigdeBestelling;
            bevestigdeBestellingUpdate.BestelStatus = BestelStatus.Goedgekeurd;

            _target.Insert(bevestigdeBestelling);

            // Act
            _target.Update(bevestigdeBestellingUpdate);
            BevestigdeBestelling result = _target.GetByFactuurnummer(bevestigdeBestellingUpdate.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsEqual(bevestigdeBestellingUpdate));
        }

        [TestMethod]
        public void Delete_Should_Throw_InvalidOperationException()
        {
            // Arrange
            BevestigdeBestelling bevestigdeBestelling = new BevestigdeBestellingBuilder().SetDummy().Create();

            // Act
            void Act()
            {
                _target.Delete(bevestigdeBestelling);
            }

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>((Action) Act);
            Assert.AreEqual("Deleting a bestelling is not allowed.", exception.Message);
        }

        [TestMethod]
        public void GetAll_ShouldReturnNoBestellingen()
        {
            // Act
            List<BevestigdeBestelling> result = _target.GetAll().OrderBy(r => r.Besteldatum).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
