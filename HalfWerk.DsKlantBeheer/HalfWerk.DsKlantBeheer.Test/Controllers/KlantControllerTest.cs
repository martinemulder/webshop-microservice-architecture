using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsKlantBeheer;
using HalfWerk.CommonModels.DsKlantBeheer.Models;
using HalfWerk.DsKlantBeheer.Controllers;
using HalfWerk.DsKlantBeheer.DAL.DataMappers;
using HalfWerk.DsKlantBeheer.Test.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Events;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HalfWerk.DsKlantBeheer.Test.Controllers
{
    [TestClass]
    public class KlantControllerTest
    {
        private Mock<IKlantDataMapper> _klantDataMapperMock;
        private Mock<IEventPublisher> _eventPublisherMock;

        private KlantController _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _klantDataMapperMock = new Mock<IKlantDataMapper>(MockBehavior.Strict);
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);

            _target = new KlantController(_klantDataMapperMock.Object, _eventPublisherMock.Object, new LoggerFactory());
        }

        [TestMethod]
        public void HandleVoegKlantToe_ShouldCallDataMapperInsert()
        {
            // Arrange
            Klant klant = new KlantBuilder().SetDummy().Create();
            var command = new VoegKlantToeCommand(klant, "");

            Klant insertParam = null;
            _klantDataMapperMock.Setup(repo => repo.Insert(It.IsAny<Klant>())).Returns(klant)
                .Callback<Klant>(entity =>
                {
                    insertParam = entity;
                });
            _eventPublisherMock.Setup(p => p.Publish(It.IsAny<KlantToegevoegdEvent>()));

            // Act
            var result = _target.HandleVoegKlantToe(command);

            // Assert
            _klantDataMapperMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsTrue(klant.IsEqual(klant));
        }

        [TestMethod]
        public void HandleVoegKlantToe_ShouldThrowException_When_IncorrectInput()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().Create();
            var command = new VoegKlantToeCommand(klant, "");


            _klantDataMapperMock.Setup(repo => repo.Insert(It.IsAny<Klant>())).Throws<Exception>();

            // Act
            Action act = () =>
            {
                _target.HandleVoegKlantToe(command);
            };

            // Assert
            var exception = Assert.ThrowsException<DatabaseException>(act);
            Assert.AreEqual("Something unexpected happend while inserting into the database", exception.Message);
        }

        [TestMethod]
        public void HandleVoegKlantToe_ShouldReturnExistingKlantId_WhenEmailExists()
        {
            // Arrange
            long klantId = 42;
            var klant = new KlantBuilder().SetDummy().SetId(klantId).SetEmail("test@test.nl").Create();
            var klanten = new List<Klant> { klant };
            var command = new VoegKlantToeCommand(klant, "");

            _klantDataMapperMock.Setup(k => k.Insert(It.IsAny<Klant>())).Throws(new DbUpdateException("",new Exception()));

            // Act
            Action result = () => _target.HandleVoegKlantToe(command);

            // Assert
            Assert.ThrowsException<EmailAllreadyExistsException>(result);
        }
    }
}
