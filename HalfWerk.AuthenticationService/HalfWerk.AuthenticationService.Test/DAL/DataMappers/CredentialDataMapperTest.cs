using HalfWerk.AuthenticationService.DAL;
using HalfWerk.AuthenticationService.DAL.DataMappers;
using HalfWerk.AuthenticationService.Entities;
using HalfWerk.AuthenticationService.Test.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HalfWerk.AuthenticationService.Test.DAL.DataMappers
{

    [TestClass]
    public class CredentialDataMapperTest
    {
        private SqliteConnection _connection;
        private AuthenticationContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AuthenticationContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AuthenticationContext(options);
            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void GetAll_ShouldThrowNotImplemented()
        {
            // Arrange
            var mapper = new CredentialDataMapper(null);

            // Act
            Action act = () =>
            {
                mapper.GetAll();
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }

        [TestMethod]
        public void GetById_ShouldThrowNotImplemented()
        {
            // Arrange
            var mapper = new CredentialDataMapper(null);

            // Act
            Action act = () =>
            {
                mapper.GetById(1);
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }

        [TestMethod]
        public void Find_ShouldReturnItemsSpecifiedByExpression()
        {
            // Arrange
            var dataMapper = new CredentialDataMapper(_context);
            CredentialEntity credential = new CredentialEntityBuilder().SetDummy().Create();
            _context.CredentialEntities.Add(credential);
            _context.SaveChanges();

            // Act
            CredentialEntity result = dataMapper.Find(x => x.Id == credential.Id).FirstOrDefault();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(credential.IsEqual(result));
        }

        [TestMethod]
        public void Insert_ShouldInsertIntoDatabase()
        {
            // Arrange
            var dataMapper = new CredentialDataMapper(_context);
            CredentialEntity credential = new CredentialEntityBuilder().SetDummy().Create();

            // Act
            dataMapper.Insert(credential);

            CredentialEntity result = dataMapper.Find(x => x.Id == credential.Id).FirstOrDefault();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(credential.IsEqual(result));
        }

        [TestMethod]
        public void Insert_ShouldInsertIntoDatabaseWithMultipleRoles()
        {
            // Arrange
            var dataMapper = new CredentialDataMapper(_context);

            CredentialEntity credential1 = new CredentialEntityBuilder().SetDummy().SetEmail("a@mail.com").SetDummyRole("Gebruiker").Create();
            CredentialEntity credential2 = new CredentialEntityBuilder().SetDummy().SetEmail("b@mail.com").SetDummyRole("Klant").SetDummyRole("Sales").Create();
            CredentialEntity credential3 = new CredentialEntityBuilder().SetDummy().SetEmail("c@mail.com").SetDummyRole("Klant").SetDummyRole("Sales").Create();

            // Act
            credential1 = dataMapper.Insert(credential1);
            credential2 = dataMapper.Insert(credential2);
            credential3 = dataMapper.Insert(credential3);

            // Changing a category should cause the artikel3 assert to fail
            credential3.CredentialRoles.ElementAt(0).Role.Name = "Hotdogverkoper";

            CredentialEntity result1 = dataMapper.Find(x => x.Id == credential1.Id).FirstOrDefault();
            CredentialEntity result2 = dataMapper.Find(x => x.Id == credential2.Id).FirstOrDefault();
            CredentialEntity result3 = dataMapper.Find(x => x.Id == credential3.Id).FirstOrDefault();

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            Assert.AreEqual(3, _context.RoleEntities.Count());

            Assert.IsTrue(credential1.IsEqual(result1));
            Assert.IsTrue(credential2.IsEqual(result2));
            Assert.IsTrue(credential3.IsEqual(result3));
        }

        [TestMethod]
        public void Update_CredentialsWithoutRoleInDatabase()
        {
            // Arrange         
            var dataMapper = new CredentialDataMapper(_context);

            CredentialEntity credential = new CredentialEntityBuilder().SetDummy().SetEmail("a@mail.com").SetDummyRole("Gebruiker").Create();
            _context.CredentialEntities.Add(credential);
            _context.SaveChanges();

            // Act
            credential.Password = "bradwurst47";
            dataMapper.Update(credential);

            var result = dataMapper.Find(x => x.Email == "a@mail.com").FirstOrDefault();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("bradwurst47", result.Password);
        }

        [TestMethod]
        public void Update_CredentialsWithMultipleRolesInDatabase()
        {
            // Arrange
            var dataMapper = new CredentialDataMapper(_context);

            CredentialEntity credential1 = new CredentialEntityBuilder().SetDummy().SetEmail("a@mail.com").SetDummyRole("Gebruiker").Create();
            CredentialEntity credential2 = new CredentialEntityBuilder().SetDummy().SetEmail("b@mail.com").SetDummyRole("Klant").SetDummyRole("Sales").Create();
            CredentialEntity credential3 = new CredentialEntityBuilder().SetDummy().SetEmail("c@mail.com").SetDummyRole("Klant").SetDummyRole("Sales").Create();

            credential1 = dataMapper.Insert(credential1);
            credential2 = dataMapper.Insert(credential2);
            credential3 = dataMapper.Insert(credential3);

            // Act
            credential2.CredentialRoles.Remove(credential2.CredentialRoles.First());
            dataMapper.Update(credential2);

            var cat = credential3.CredentialRoles.ElementAt(0);
            credential3.CredentialRoles.Remove(cat);

            var newCat = new CredentialRoleEntity { Role = new RoleEntity { Name = "Metzger" } };
            credential3.CredentialRoles.Add(newCat);
            dataMapper.Update(credential3);

            CredentialEntity result1 = dataMapper.Find(x => x.Email == "a@mail.com").FirstOrDefault();
            CredentialEntity result2 = dataMapper.Find(x => x.Email == "b@mail.com").FirstOrDefault();
            CredentialEntity result3 = dataMapper.Find(x => x.Email == "c@mail.com").FirstOrDefault();

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);

            Assert.AreEqual(4, _context.RoleEntities.Count());

            Assert.AreEqual(1, result1.CredentialRoles.Count());
            Assert.AreEqual(1, result2.CredentialRoles.Count());
            Assert.AreEqual(2, result3.CredentialRoles.Count());

            Assert.IsTrue(credential1.IsEqual(result1));
            Assert.IsTrue(credential2.IsEqual(result2));
            Assert.IsTrue(credential3.IsEqual(result3));
        }

        [TestMethod]
        public void Delete_ShouldThrowNotImplemented()
        {
            // Arrange
            var mapper = new CredentialDataMapper(null);

            // Act
            Action act = () =>
            {
                mapper.Delete(null);
            };

            // Assert
            Assert.ThrowsException<NotImplementedException>(act);
        }
    }
}
