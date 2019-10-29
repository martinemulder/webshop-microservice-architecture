using HalfWerk.AuthenticationService.DAL.DataMappers;
using HalfWerk.AuthenticationService.Entities;
using HalfWerk.AuthenticationService.Listeners;
using HalfWerk.AuthenticationService.Models;
using HalfWerk.AuthenticationService.Test.Entities;
using HalfWerk.CommonModels.AuthenticationService;
using HalfWerk.CommonModels.AuthenticationService.Models;
using HalfWerk.CommonModels.CommonExceptions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Events;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HalfWerk.AuthenticationService.Test.Listeners
{
    [TestClass]
    public class AccountListenerTest
    {
        [TestMethod]
        public void Login_ShouldReturnJWT_When_UserFound()
        {
            // Arrange
            var credentialEntities = new List<CredentialEntity>
            {
                new CredentialEntityBuilder().SetId(1).SetEmail("hans@gmail.com").SetPassword("42818AAC3AEFB82B8A9E136A40657A0DD8582A0C6C544755806BC20E9D32D4E3").SetDummyRole("Gebruikert").Create(),
            };

            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Returns(credentialEntities);

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            var credentials = new Credential
            {
                Email = "hans@gmail.com",
                Password = "Heeeeeulsterk"
            };

            // Act
            var result = listener.Login(new LoginCommand(credentials, ""));

            // Assert
            mapperMock.VerifyAll();
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void Login_ShouldReturnEmptyJWT_When_NoUserFound()
        {
            // Arrange
            var credentialEntities = new List<CredentialEntity>();

            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Returns(credentialEntities);

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            var credentials = new Credential
            {
                Email = "hans@gmail.com",
                Password = "Heeeeeulsterk"
            };

            // Act
            var result = listener.Login(new LoginCommand(credentials, ""));

            // Assert
            mapperMock.VerifyAll();
            Assert.IsTrue(result.Length == 0);
        }

        [TestMethod]
        public void Login_ShouldThrow_DatabaseException_OnError()
        {
            // Arrange         
            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Throws<Exception>();

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            // Act
            Action act = () =>
            {
                listener.Login(new LoginCommand(new Credential(), ""));
            };

            // Assert
            var exception = Assert.ThrowsException<DatabaseException>(act);
            Assert.AreEqual("Something unexpected happened while searching through the database", exception.Message);
            mapperMock.VerifyAll();
        }

        [TestMethod]
        public void Register_ShouldReturn_RegisterResultOk()
        {
            // Arrange
            var credentialEntities = new List<CredentialEntity>();

            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Returns(credentialEntities);
            mapperMock.Setup(x => x.Insert(It.IsAny<CredentialEntity>())).Returns(new CredentialEntity());

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            var credentials = new Credential
            {
                Email = "hans@gmail.com",
                Password = "Heeeeeulsterk"
            };

            // Act
            var result = listener.Register(new RegisterCommand(credentials, ""));

            // Assert
            mapperMock.VerifyAll();
            Assert.AreEqual(RegisterResult.Ok, result);
        }

        [TestMethod]
        public void Register_ShouldReturn_RegisterResultEmailInUse()
        {
            // Arrange
            var credentialEntities = new List<CredentialEntity>
            {
                new CredentialEntityBuilder().SetId(1).SetEmail("hans@gmail.com").SetPassword("42818AAC3AEFB82B8A9E136A40657A0DD8582A0C6C544755806BC20E9D32D4E3").SetDummyRole("Gebruikert").Create(),
            };

            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Returns(credentialEntities);

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            var credentials = new Credential
            {
                Email = "hans@gmail.com",
                Password = "Heeeeeulsterk"
            };

            // Act
            var result = listener.Register(new RegisterCommand(credentials, ""));

            // Assert
            mapperMock.VerifyAll();
            Assert.AreEqual(RegisterResult.EmailInUse, result);
        }

        [TestMethod]
        public void Register_ShouldThrow_DatabaseException_OnError()
        {
            // Arrange
            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Throws<Exception>();

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            // Act
            Action act = () =>
            {
                listener.Register(new RegisterCommand(new Credential(), ""));
            };

            // Assert
            var exception = Assert.ThrowsException<DatabaseException>(act);
            Assert.AreEqual("Something unexpected happened while inserting in the database", exception.Message);
            mapperMock.VerifyAll();
        }

        [TestMethod]
        public void Validate_ShouldSucceed()
        {
            // Arrange
            var listener = new AccountListener(null, new LoggerFactory());

            var jwt = new JSONWebToken(1, "hans@gmail.com", new List<string>() { "Gebruikert" });

            // Act
            var isValid = listener.Validate(new ValidateCommand(jwt.Token, ""));

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_When_NoJwtProvided()
        {
            // Arrange
            var listener = new AccountListener(null, new LoggerFactory());

            // Act
            var isValid = listener.Validate(new ValidateCommand(null, ""));

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void AddRole_ShouldReturn_True()
        {
            // Arrange
            var credentials = new Credential
            {
                Email = "hans@gmail.com",
                Password = "Heeeeeulsterk"
            };

            var credentialEntities = new List<CredentialEntity>() { new CredentialEntity() { Email = credentials.Email, Password = credentials.Password, CredentialRoles = new List<CredentialRoleEntity>() } };
            CredentialEntity updateParam = null;

            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Returns(credentialEntities);
            mapperMock.Setup(x => x.Update(It.IsAny<CredentialEntity>()))
                .Callback<CredentialEntity>(entity =>
                {
                    updateParam = entity;
                });

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            var roleRequest = new RoleRequest() { Email = credentials.Email, Role = Roles.Sales };

            // Act
            var result = listener.AddRole(new AddRoleCommand(roleRequest, ""));

            // Assert
            mapperMock.VerifyAll();
            Assert.IsTrue(result);
            Assert.IsNotNull(updateParam);
            Assert.AreEqual(roleRequest.Email, updateParam.Email);
            Assert.AreEqual(roleRequest.Role.ToString(), updateParam.CredentialRoles.FirstOrDefault().Role.Name);
        }

        [TestMethod]
        public void AddRole_ShouldThrow_DatabaseException()
        {
            // Arrange
            var credentialEntities = new List<CredentialEntity>();

            var mapperMock = new Mock<ICredentialDataMapper>(MockBehavior.Strict);
            mapperMock.Setup(x => x.Find(It.IsAny<Expression<Func<CredentialEntity, bool>>>())).Returns(credentialEntities);

            var listener = new AccountListener(mapperMock.Object, new LoggerFactory());

            // Act
            Action act = () =>
            {
                listener.AddRole(new AddRoleCommand(new RoleRequest(), ""));
            };

            // Assert
            var exception = Assert.ThrowsException<DatabaseException>(act);
            Assert.AreEqual("Something unexpected happened while updating in the database", exception.Message);
            mapperMock.VerifyAll();
        }
    }
}
