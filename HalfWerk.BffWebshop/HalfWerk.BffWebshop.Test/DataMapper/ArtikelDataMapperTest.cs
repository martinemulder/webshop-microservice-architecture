using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.BffWebshop.Test.Entities;
using HalfWerk.BffWebshop.Test.Entities.Artikel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.BffWebshop.Test.DataMapper
{
    [TestClass]
    public class ArtikelDataMapperTest
    {
        private BffContext _context;
        private SqliteConnection _connection;
        private DbContextOptions<BffContext> _options;

        private ArtikelDataMapper _target;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BffContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new BffContext(_options);
            _target = new ArtikelDataMapper(_context);

            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [TestMethod]
        public void GetAllArtikelenFromDatabase()
        {
            // Arrange
            ArtikelEntity artikel1 = new ArtikelEntityBuilder().SetDummy().Create();
            ArtikelEntity artikel2 = new ArtikelEntityBuilder().SetDummy().SetArtikelnummer(2).Create();

            _context.ArtikelEntities.Add(artikel1);
            _context.ArtikelEntities.Add(artikel2);
            
            _context.SaveChanges();

            // Act
            List<ArtikelEntity> result = _target.GetAll().ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(artikel1.IsEqual(result[0]));
            Assert.IsTrue(artikel2.IsEqual(result[1]));
        }

        [TestMethod]
        public void GetArtikelFromDatabaseById()
        {
            ArtikelEntity artikel = new ArtikelEntityBuilder().SetDummy().SetArtikelnummer(12).Create();
            _context.ArtikelEntities.Add(artikel);

            _context.SaveChanges();

            // Act
            ArtikelEntity result1 = _target.GetById(12);
            ArtikelEntity result2 = _target.GetById(23);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.IsEqual(artikel));
            Assert.IsNull(result2);
        }

        [TestMethod]
        public void GetArtikelFromDatabaseByPredicate()
        {
            // Arrange              
            ArtikelEntity artikel1 = new ArtikelEntityBuilder().SetDummy().SetArtikelnummer(12).Create();
            ArtikelEntity artikel2 = new ArtikelEntityBuilder().SetDummy().SetArtikelnummer(13).Create();
            ArtikelEntity artikel3 = new ArtikelEntityBuilder().SetDummy().SetArtikelnummer(14).Create();

            _context.ArtikelEntities.Add(artikel1);
            _context.ArtikelEntities.Add(artikel2);
            _context.ArtikelEntities.Add(artikel3);

            _context.SaveChanges();

            // Act
            var result = _target.Find(x => x.Artikelnummer > 12);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(artikel2.IsEqual(result.ElementAt(0)));
            Assert.IsTrue(artikel3.IsEqual(result.ElementAt(1)));
        }

        [TestMethod]
        public void InsertArtikelInDatabase()
        {
            // Arrange           
            ArtikelEntity artikel = new ArtikelEntityBuilder().SetDummy().Create();

            // Act
            _target.Insert(artikel);

            ArtikelEntity result = _target.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(artikel.IsEqual(result));
        }
       
        [TestMethod]
        public void InsertArtikelInDatabaseWithMultipleCategories()
        {
            // Arrange           
            ArtikelEntity artikel1 = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat1").SetDummyCategorie("Cat2").Create();
            ArtikelEntity artikel2 = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat2").SetDummyCategorie("Cat3").SetArtikelnummer(2).Create();
            ArtikelEntity artikel3 = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat2").SetDummyCategorie("Cat3").SetArtikelnummer(3).Create();

            // Act
            artikel1 = _target.Insert(artikel1);
            artikel2 = _target.Insert(artikel2);
            artikel3 = _target.Insert(artikel3);

            // Changing a category should cause the artikel3 assert to fail
            artikel3.ArtikelCategorieen.ElementAt(0).Categorie.Categorie = "Cat4";

            ArtikelEntity result1 = _target.GetById(1);
            ArtikelEntity result2 = _target.GetById(2);
            ArtikelEntity result3 = _target.GetById(3);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            Assert.AreEqual(3, _context.CategorieEntities.Count());

            Assert.IsTrue(artikel1.IsEqual(result1));
            Assert.IsTrue(artikel2.IsEqual(result2));
            Assert.IsTrue(artikel3.IsEqual(result3));
        }

        [TestMethod]
        public void DeleteArtikelFromDatabase()
        {
            // Arrange           
            ArtikelEntity artikel = new ArtikelEntityBuilder().SetDummy().Create();
            _context.ArtikelEntities.Add(artikel);
            _context.SaveChanges();

            // Act
            _target.Delete(artikel);

            var result = _target.GetAll();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void UpdatedArtikelFromDatabase()
        {
            // Arrange           
            ArtikelEntity artikel = new ArtikelEntityBuilder().SetDummy().Create();
            _context.ArtikelEntities.Add(artikel);
            _context.SaveChanges();

            // Act
            artikel.Naam = "Bewerkt";
            _target.Update(artikel);

            var result = _target.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Bewerkt", result.Naam);
        }

        [TestMethod]
        public void UpdateArtikelInDatabaseWithMultipleCategories()
        {
            // Arrange           
            ArtikelEntity artikel1 = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat1").SetDummyCategorie("Cat2").Create();
            ArtikelEntity artikel2 = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat2").SetDummyCategorie("Cat3").SetArtikelnummer(2).Create();
            ArtikelEntity artikel3 = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat2").SetDummyCategorie("Cat3").SetArtikelnummer(3).Create();

            _target.Insert(artikel1);
            artikel2 = _target.Insert(artikel2);
            _target.Insert(artikel3);

            // Act
            artikel2.ArtikelCategorieen.Remove(artikel2.ArtikelCategorieen.First());
            _target.Update(artikel2);

            var cat = artikel3.ArtikelCategorieen.ElementAt(0);
            artikel3.ArtikelCategorieen.Remove(cat);

            var newCat = new ArtikelCategorieEntity {Categorie = new CategorieEntity {Categorie = "Cat4"}};
            artikel3.ArtikelCategorieen.Add(newCat);
            _target.Update(artikel3);

            ArtikelEntity result1 = _target.GetById(1);
            ArtikelEntity result2 = _target.GetById(2);
            ArtikelEntity result3 = _target.GetById(3);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            Assert.AreEqual(4, _context.CategorieEntities.Count());

            Assert.AreEqual(2, result1.ArtikelCategorieen.Count());
            Assert.AreEqual(1, result2.ArtikelCategorieen.Count());
            Assert.AreEqual(2, result3.ArtikelCategorieen.Count());

            Assert.IsTrue(artikel1.IsEqual(result1));
            Assert.IsTrue(artikel2.IsEqual(result2));            
            Assert.IsTrue(artikel3.IsEqual(result3));
        }
        
        [TestMethod]
        public void ShouldNotInsertArtikelInDatabaseWithSameArtikelnummer()
        {
            // Arrange
            ArtikelEntity artikel1 = new ArtikelEntityBuilder().SetDummy().Create();
            ArtikelEntity artikel2 = new ArtikelEntityBuilder().SetDummy().Create();

            // Act
            _target.Insert(artikel1);

            Action act = () =>
            {
                _target.Insert(artikel2);
            };

            // Assert
            Assert.ThrowsException<InvalidOperationException>(act);
        }
    }
}
