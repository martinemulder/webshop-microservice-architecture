using System;
using System.Collections.Generic;
using System.Linq;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.DAL;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Test.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalfWerk.DsBestelService.Test.DataMapper
{
    [TestClass]
    public class BestellingDataMapperTest
    {
        private SqliteConnection _connection;
        private DbContextOptions<BestelContext> _options;
        private BestelContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BestelContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _context = new BestelContext(_options);
            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllArtikelen()
        {
            // Arrange
            Bestelling bestelling1 = new BestellingBuilder().SetDummy().Create();
            Bestelling bestelling2 = new BestellingBuilder().SetDummy().Create();

            _context.Bestellingen.Add(bestelling1);
            _context.Bestellingen.Add(bestelling2);
            _context.SaveChanges();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            List<Bestelling> result = dataMapper.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(bestelling1.IsEqual(result[0]));
            Assert.IsTrue(bestelling2.IsEqual(result[1]));
        }

        [TestMethod]
        public void GetById_Should_ReturnBestellingWithCertainId_When_Id_IsGiven()
        {
            // Arrange
            Bestelling bestelling = new BestellingBuilder().SetDummy().Create();

            _context.Bestellingen.Add(bestelling);
            _context.SaveChanges();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            Bestelling result = dataMapper.GetById(bestelling.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(bestelling.IsEqual(result));
        }

        [TestMethod]
        public void GetByFactuurnummer_Should_ReturnBestellingWithCertainId_When_Factuurnummer_IsGiven()
        {
            // Arrange
            Bestelling bestelling = new BestellingBuilder().SetDummy().Create();

            _context.Bestellingen.Add(bestelling);
            _context.SaveChanges();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            Bestelling result = dataMapper.GetByFactuurnummer(bestelling.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(bestelling.IsEqual(result));
        }

        [TestMethod]
        public void Find_Should_ReturnArtikelWithCertainPredicate_When_Predicate_IsGiven()
        {
            // Arrange
            Bestelling bestelling1 = new BestellingBuilder().SetDummy().Create();
            Bestelling bestelling2 = new BestellingBuilder().SetDummy().SetBestelStatus(BestelStatus.Goedgekeurd).Create();

            _context.Bestellingen.Add(bestelling1);
            _context.Bestellingen.Add(bestelling2);
            _context.SaveChanges();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            List<Bestelling> result = dataMapper.Find(b => b.BestelStatus == BestelStatus.Goedgekeurd).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(bestelling2.IsEqual(result[0]));
        }

        [TestMethod]
        public void Insert_Should_InsertArtikel_When_NieuwArtikel_IsGiven()
        {
            // Arrange           
            Bestelling bestelling = new BestellingBuilder().SetDummy().Create();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            dataMapper.Insert(bestelling);

            Bestelling result = dataMapper.GetByFactuurnummer(bestelling.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(bestelling.IsEqual(result));
        }

        [TestMethod]
        public void Insert_ShouldNotInsertArtikelInDatabase_When_AnExistingFactuurnummer_IsGiven()
        {
            // Arrange
            Bestelling bestelling1 = new BestellingBuilder().SetDummy()
                .SetFactuurnummer(1)
                .Create();
            Bestelling bestelling2 = new BestellingBuilder().SetDummy()
                .SetFactuurnummer(1)
                .Create();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            dataMapper.Insert(bestelling1);

            void Act()
            {
                dataMapper.Insert(bestelling2);
            }

            // Assert
            Assert.ThrowsException<DbUpdateException>((Action) Act);
        }

        [TestMethod]
        public void Insert_ShouldNotInsertArtikelInDatabase_When_NoKlantNummer_IsGiven()
        {
            // Arrange
            Bestelling bestelling = new BestellingBuilder()
                .SetBesteldatum(DateTime.Now)
                .SetBestelStatus(BestelStatus.Geplaatst)
                .SetFactuurnummer(1)
                .Create();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            void Act()
            {
                dataMapper.Insert(bestelling);
            }

            // Assert
            Assert.ThrowsException<ArgumentNullException>((Action) Act);
        }

        [TestMethod]
        public void Update_ShouldNotUpdateArtikelInDatabase_When_NoFactuurNummer_IsGiven()
        {
            // Arrange
            Bestelling bestelling = new BestellingBuilder()
                .SetBesteldatum(DateTime.Now)
                .SetBestelStatus(BestelStatus.Geplaatst)
                .SetKlantnummer(1)
                .Create();

            _context.Bestellingen.Add(bestelling);             
            var dataMapper = new BestellingDataMapper(_context);

            // Act
            void Act()
            {
                dataMapper.Update(bestelling);
            }

            // Assert
            Assert.ThrowsException<ArgumentNullException>((Action) Act);
        }

        [TestMethod]
        public void Update_ShouldUpdateBestelling_When_AnUpdatedBestelling_IsGiven()
        {
            // Arrange
            Bestelling bestelling = new BestellingBuilder().SetDummy().Create();
            Bestelling bestellingUpdate = bestelling;
            bestellingUpdate.BestelStatus = BestelStatus.Goedgekeurd;

            var dataMapper = new BestellingDataMapper(_context);
            dataMapper.Insert(bestelling);

            // Act
            dataMapper.Update(bestellingUpdate);
            Bestelling result = dataMapper.GetByFactuurnummer(bestellingUpdate.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsEqual(bestellingUpdate));
        }

        [TestMethod]
        public void Delete_Should_Throw_InvalidOperationException()
        {
            // Arrange           
            Bestelling bestelling = new BestellingBuilder().SetDummy().Create();

            var dataMapper = new BestellingDataMapper(_context);

            // Act
            void Act()
            {
                dataMapper.Delete(bestelling);
            }

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>((Action) Act);
            Assert.AreEqual("Deleting bestelling is not allowed", exception.Message);
        }
    }
}
