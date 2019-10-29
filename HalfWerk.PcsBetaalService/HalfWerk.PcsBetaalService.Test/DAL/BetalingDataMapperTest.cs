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

namespace HalfWerk.PcsBetaalService.Test.DAL
{
    [TestClass]
    public class BetalingDataMapperTest
    {
        private DbContextOptions<BetaalContext> _options;
        private SqliteConnection _connection;
        private BetaalContext _betaalContext;

        private BestellingBuilder _bestellingBuilder;

        private BetalingDataMapper _target;

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
            _target = new BetalingDataMapper(_betaalContext);
        }

        [TestCleanup]
        public void AfterEach()
        {
            _betaalContext.Dispose();
            _connection.Dispose();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllBetalingen()
        {
            // Arrange
            Betaling betaling1 = new Betaling()
            {
                Bedrag = 100.0m,
                BetaalDatum = DateTime.Now
            };
            Betaling betaling2 = new Betaling()
            {
                Bedrag = 120.0m,
                BetaalDatum = DateTime.Now
            };

            _betaalContext.Betalingen.Add(betaling1);
            _betaalContext.Betalingen.Add(betaling2);
            _betaalContext.SaveChanges();

            // Act
            List<Betaling> result = _target.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(100.0m, result[0].Bedrag);
            Assert.AreEqual(120.0m, result[1].Bedrag);
        }

        [TestMethod]
        public void Find_Should_ReturnBetalingWithCertainPredicate_When_Predicate_IsGiven()
        {
            // Arrange
            Betaling betaling1 = new Betaling()
            {
                Bedrag = 100.0m,
                BetaalDatum = DateTime.Now
            };
            Betaling betaling2 = new Betaling()
            {
                Bedrag = 120.0m,
                BetaalDatum = DateTime.Now 
            };

            _betaalContext.Betalingen.Add(betaling1);
            _betaalContext.Betalingen.Add(betaling2);
            _betaalContext.SaveChanges();

            // Act
            List<Betaling> result = _target.Find(b => b.Bedrag == 120.0m).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(betaling2.Bedrag, result[0].Bedrag);
        }

        [TestMethod]
        public void Insert_ShouldInsertNewBetalingIntoTheDatabase()
        {
            // Arrange
            var betaling = new Betaling()
            {
                Bedrag = 100.0m,
                BetaalDatum = DateTime.Now,
                Factuurnummer = 1
            };

            // Act
            _target.Insert(betaling);

            // Assert
            var result = _betaalContext.Betalingen.SingleOrDefault(b => b.Factuurnummer == 1);
            Assert.AreEqual(betaling.Bedrag, result.Bedrag);
            Assert.AreEqual(betaling.BetaalDatum, result.BetaalDatum);
            Assert.AreEqual(betaling.Factuurnummer, result.Factuurnummer);
        }

        [TestMethod]
        public void NotImplementedMethods_ShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => _target.GetById(1));
            Assert.ThrowsException<NotImplementedException>(() => _target.Delete(new Betaling()));
            Assert.ThrowsException<NotImplementedException>(() => _target.Update(new Betaling()));
        }
    }
}
