using HalfWerk.CommonModels.DsKlantBeheer.Models;
using HalfWerk.DsKlantBeheer.DAL;
using HalfWerk.DsKlantBeheer.DAL.DataMappers;
using HalfWerk.DsKlantBeheer.Test.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.DsKlantBeheer.Test.DAL.DataMappers
{
    [TestClass]
    public class KlantDataMapperTest
    {
        private SqliteConnection _connection;
        private KlantContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<KlantContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _context = new KlantContext(options);
            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllKlanten()
        {
            // Arrange
            List<Klant> klanten = new List<Klant>()
            {
                new KlantBuilder().SetDummy().Create(),
                new KlantBuilder().SetDummy().SetEmail("jan@burger.com").Create()
            };
            klanten = klanten.OrderBy(x => x.Id).ToList();

            _context.Klanten.Add(klanten[0]);
            _context.Klanten.Add(klanten[1]);
            _context.SaveChanges();

            var dataMapper = new KlantDataMapper(_context);

            // Act
            List<Klant> result = dataMapper.GetAll().OrderBy(x => x.Id).ToList();

            // Assert
            Assert.AreEqual(klanten.Count, result.Count());
            Assert.IsTrue(klanten[0].IsEqual(result[0]));
            Assert.IsTrue(klanten[1].IsEqual(result[1]));
        }


        [TestMethod]
        public void GetById_ShouldReturnKlant_When_ExistingIdIsGiven()
        {
            // Arrange
            Klant klant = new KlantBuilder().SetDummy().Create();

            _context.Klanten.Add(klant);
            _context.SaveChanges();

            var dataMapper = new KlantDataMapper(_context);

            // Act
            Klant result = dataMapper.GetById(klant.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(klant.IsEqual(result));
        }

        [TestMethod]
        public void Find_ShouldReturnKlantMatchingACertainPredicate_When_PredicateIsGiven()
        {
            // Arrange
            Klant klant1 = new KlantBuilder().SetDummy().Create();
            Klant klant2 = new KlantBuilder().SetDummy().SetEmail("hans@worst.com").Create();

            _context.Klanten.Add(klant1);
            _context.Klanten.Add(klant2);
            _context.SaveChanges();

            var dataMapper = new KlantDataMapper(_context);

            // Act
            List<Klant> result = dataMapper.Find(x => x.Email.Contains("worst")).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(klant2.IsEqual(result[0]));
        }
        
        [TestMethod]
        public void Insert_ShouldInsertKlant()
        {
            // Arrange           
            Klant klant = new KlantBuilder().SetDummy().Create();

            var dataMapper = new KlantDataMapper(_context);

            // Act
            dataMapper.Insert(klant);

            Klant result = dataMapper.GetById(klant.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(klant.IsEqual(result));
        }

        [TestMethod]
        public void Update_ShouldUpdateKlant()
        {
            // Arrange
            Klant klant = new KlantBuilder().SetDummy().Create();
            var dataMapper = new KlantDataMapper(_context);

            klant = dataMapper.Insert(klant);
            klant.Achternaam = "Worst";

            // Act
            dataMapper.Update(klant);
            Klant result = dataMapper.Find(x => x.Achternaam == "Worst").FirstOrDefault();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsEqual(klant));
            Assert.AreEqual("Worst", klant.Achternaam);
        }

        [TestMethod]
        public void Delete_ShouldThrowInvalidOperationException()
        {
            // Arrange           
            var klant = new KlantBuilder().SetDummy().Create();

            var dataMapper = new KlantDataMapper(_context);

            // Act
            void Act()
            {
                dataMapper.Delete(klant);
            }

            // Assert
            var exception = Assert.ThrowsException<InvalidOperationException>((Action)Act);
            Assert.AreEqual("Deleting klanten is not allowed", exception.Message);
        }
    }
}
