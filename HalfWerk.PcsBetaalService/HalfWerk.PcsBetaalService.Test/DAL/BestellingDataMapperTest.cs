using HalfWerk.CommonModels.PcsBetaalService.Models;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.DAL;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Test.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HalfWerk.PcsBetaalService.Test.DAL
{
    [TestClass]
    public class BestellingDataMapperTest
    {
        private DbContextOptions<BetaalContext> _options;
        private SqliteConnection _connection;
        private BetaalContext _betaalContext;

        private BestellingBuilder _bestellingBuilder;

        private BestellingDataMapper _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BetaalContext>()
                .UseSqlite(_connection)
                .Options;

            _betaalContext = new BetaalContext(_options);
            _betaalContext.Database.EnsureCreated();

            _bestellingBuilder = new BestellingBuilder();
            _target = new BestellingDataMapper(_betaalContext);
        }

        [TestCleanup]
        public void AfterEach()
        {
            _betaalContext.Dispose();
            _connection.Dispose();
        }

        [TestMethod]
        public void Insert_ShouldInsertNewBestellingIntoTheDatabase()
        {
            // Arrange
            var bestelling = new Bestelling()
            {
                Klantnummer = 1,
                FactuurTotaalInclBtw = 200.0m,
                FactuurTotaalExclBtw = 188.88m,
                Factuurnummer = 1,
                BestelStatus = BestelStatus.Goedgekeurd,
                Besteldatum = DateTime.Now,
                BestelRegels = _bestellingBuilder.SetDummyBestelRegels().ToList()
            };

            // Act
            _target.Insert(bestelling);

            // Assert
            var result = _betaalContext.Bestellingen.SingleOrDefault(b => b.Factuurnummer == 1);

            Assert.AreEqual(2, result.BestelRegels.Count);
            Assert.AreEqual(1, result.Klantnummer);
            Assert.AreEqual(200.0m, result.FactuurTotaalInclBtw);
            Assert.AreEqual(BestelStatus.Goedgekeurd, result.BestelStatus);
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllBestellingen()
        {
            // Arrange
            Bestelling bestelling1 = new BestellingBuilder().SetDummy().Create();
            Bestelling bestelling2 = new BestellingBuilder().SetDummy().Create();

            _betaalContext.Bestellingen.Add(bestelling1);
            _betaalContext.Bestellingen.Add(bestelling2);
            _betaalContext.SaveChanges();

            var dataMapper = new BestellingDataMapper(_betaalContext);

            // Act
            List<Bestelling> result = dataMapper.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(bestelling1.BestelRegels.Count, result[0].BestelRegels.Count);
            Assert.AreEqual(bestelling2.BestelRegels.Count, result[1].BestelRegels.Count);
            Assert.AreEqual(bestelling1.Besteldatum, result[0].Besteldatum);
            Assert.AreEqual(bestelling2.Besteldatum, result[1].Besteldatum);
        }

        [TestMethod]
        public void GetByFactuurnummer_Should_ReturnBestellingWithCertainId_When_Factuurnummer_IsGiven()
        {
            // Arrange
            Bestelling bestelling = new BestellingBuilder().SetDummy().Create();

            _betaalContext.Bestellingen.Add(bestelling);
            _betaalContext.SaveChanges();

            var dataMapper = new BestellingDataMapper(_betaalContext);

            // Act
            Bestelling result = dataMapper.GetByFactuurnummer(bestelling.Factuurnummer);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(bestelling.Factuurnummer, result.Factuurnummer);
        }

        [TestMethod]
        public void Find_Should_ReturnArtikelWithCertainPredicate_When_Predicate_IsGiven()
        {
            // Arrange
            Bestelling bestelling1 = new BestellingBuilder().SetDummy().Create();
            Bestelling bestelling2 = new BestellingBuilder().SetDummy().SetBestelStatus(BestelStatus.Goedgekeurd).Create();

            _betaalContext.Bestellingen.Add(bestelling1);
            _betaalContext.Bestellingen.Add(bestelling2);
            _betaalContext.SaveChanges();

            var dataMapper = new BestellingDataMapper(_betaalContext);

            // Act
            List<Bestelling> result = dataMapper.Find(b => b.BestelStatus == BestelStatus.Goedgekeurd).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(bestelling2.Factuurnummer, result[0].Factuurnummer);
        }

        [TestMethod]
        public void NotImplementedMethods_ShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => _target.GetById(1));
            Assert.ThrowsException<NotImplementedException>(() => _target.Delete(new Bestelling()));
            Assert.ThrowsException<NotImplementedException>(() => _target.Update(new Bestelling()));
        }
    }
}
