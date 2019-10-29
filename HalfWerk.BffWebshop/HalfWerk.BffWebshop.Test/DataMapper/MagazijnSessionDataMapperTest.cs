using System;
using System.Linq;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalfWerk.BffWebshop.Test.DataMapper
{
    [TestClass]
    public class MagazijnSessionDataMapperTest
    {
        private SqliteConnection _connection;
        private DbContextOptions<BffContext> _options;
        private BffContext _context;

        private MagazijnSessionDataMapper _target;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BffContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new BffContext(_options);
            _target = new MagazijnSessionDataMapper(_context);

            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [TestMethod]
        public void GetByFactuurnummer_ShouldReturnSessionByFactuurNummer()
        {
            var session = new MagazijnSessionEntity { Factuurnummer = 10, MedewerkerEmail = "medewerker" };
            _context.MagazijnSessions.Add(session);
            _context.SaveChanges();

            var result = _target.GetByFactuurnummer(session.Factuurnummer);

            Assert.AreEqual(session.Id, result.Id);
            Assert.AreEqual(session.Factuurnummer, result.Factuurnummer);
            Assert.AreEqual(session.MedewerkerEmail, result.MedewerkerEmail);
        }

        [TestMethod]
        public void Insert_ShouldInsertANewMagazijnSessionIntoTheDatabase()
        {
            var session = new MagazijnSessionEntity { Factuurnummer = 10, MedewerkerEmail = "medewerker" };
              
            _target.Insert(session);

            var dbResult = _context.MagazijnSessions.SingleOrDefault(s => s.Id == session.Id);
            Assert.AreEqual(session.Id, dbResult.Id);
            Assert.AreEqual(session.Factuurnummer, dbResult.Factuurnummer);
            Assert.AreEqual(session.MedewerkerEmail, dbResult.MedewerkerEmail);
        }

        [TestMethod]
        public void Find_ShouldReturnSessionMatchingProvidedExpressions()
        {
            var session = new MagazijnSessionEntity { Factuurnummer = 10, MedewerkerEmail = "medewerker" };
            _context.MagazijnSessions.Add(session);
            _context.SaveChanges();

            var result = _target.Find(m => m.MedewerkerEmail == session.MedewerkerEmail).First();

            Assert.AreEqual(session.Id, result.Id);
            Assert.AreEqual(session.Factuurnummer, result.Factuurnummer);
            Assert.AreEqual(session.MedewerkerEmail, result.MedewerkerEmail);
        }

        [TestMethod]
        public void Delete_ShouldRemoveAMagazijnSessionFromTheDatabase()
        {
            var session = new MagazijnSessionEntity { Factuurnummer = 10, MedewerkerEmail = "medewerker" };
            _context.MagazijnSessions.Add(session);
            _context.SaveChanges();

            _target.Delete(session);

            var dbResult = _context.MagazijnSessions.SingleOrDefault(s => s.Id == session.Id);
            Assert.IsNull(dbResult, "dbResult != null");
        }

        [TestMethod]
        public void NotImplementedMethods_ShouldThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => _target.GetAll());
            Assert.ThrowsException<NotImplementedException>(() => _target.GetById(1));
            Assert.ThrowsException<NotImplementedException>(() => _target.Update(new MagazijnSessionEntity()));
            Assert.ThrowsException<NotImplementedException>(() => _target.Update(new MagazijnSessionEntity()));
        }
    }
}