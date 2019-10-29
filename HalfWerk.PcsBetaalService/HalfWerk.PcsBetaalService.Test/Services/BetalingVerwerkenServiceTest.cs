using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.PcsBetaalService.Models;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.Controllers;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Services;
using HalfWerk.PcsBetaalService.Test.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Events;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HalfWerk.PcsBetaalService.Test.Services
{
    [TestClass]
    public class BetalingVerwerkenServiceTest
    {
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;
        private Mock<IBetalingDataMapper> _betalingDataMapperMock;
        private Mock<IEventPublisher> _eventPublisherMock;
        private BestellingBuilder _bestellingBuilder;

        private BetalingVerwerkenService _target;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            AutoMapperConfiguration.Configure();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _betalingDataMapperMock = new Mock<IBetalingDataMapper>(MockBehavior.Strict);
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
            _bestellingDataMapperMock.Setup(b => b.GetByFactuurnummer(1)).Returns(new Bestelling()
            {
                Klantnummer = 1
            });

            _eventPublisherMock.Setup(s => s.Publish(It.IsAny<DomainEvent>()));
            _bestellingBuilder = new BestellingBuilder();
            _target = new BetalingVerwerkenService(_betalingDataMapperMock.Object, _bestellingDataMapperMock.Object, _eventPublisherMock.Object, new LoggerFactory());
        }

        [TestMethod]
        public void BestellingVerwerken_ShouldPublish1Event_WhenItsBelow500()
        {
            // Arrange
            _bestellingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Bestelling, bool>>>())).Returns(new List<Bestelling>
            {
                _bestellingBuilder.SetDummy().Create()
            });

            _betalingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Betaling, bool>>>())).Returns(new List<Betaling>());
            // Act 
            _target.HandleBestellingVerwerken(1);

            // Assert
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Exactly(1));

        }

        [TestMethod]
        public void BestellingVerwerken_ShouldPublish0Events_WhenItsAbove500()
        {          
            // Arrange
            _bestellingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Bestelling, bool>>>())).Returns(new List<Bestelling>
            {
                _bestellingBuilder.SetDummy().SetFactuurTotaalInclBtw(600).Create()
            });

            _betalingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Betaling, bool>>>())).Returns(new List<Betaling>());
            // Act 
            _target.HandleBestellingVerwerken(1);

            // Assert
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Exactly(0));
        }

        [TestMethod]
        public void BestellingVerwerken_ShouldPublish2Event_WhenCumultatiefIsBelow500()
        {
            // Arrange
            _bestellingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Bestelling, bool>>>())).Returns(new List<Bestelling>
            {
                _bestellingBuilder.SetDummy().SetFactuurTotaalInclBtw(100).Create(),
                _bestellingBuilder.SetDummy().SetFactuurTotaalInclBtw(400).Create()
            });

            _betalingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Betaling, bool>>>())).Returns(new List<Betaling>());
            // Act 
            _target.HandleBestellingVerwerken(1);

            // Assert
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Exactly(2));
        }

        [TestMethod]
        public void BetalingVerwerken_ShouldPublish0Events_WhenBetalingIsNotEnough()
        {
            // Arrange
            _bestellingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Bestelling, bool>>>())).Returns(new List<Bestelling>
            {
                _bestellingBuilder.SetDummy().SetFactuurTotaalInclBtw(100).SetBesteldatum(DateTime.Now.AddDays(-2)).Create(),
                _bestellingBuilder.SetDummy().SetFactuurTotaalInclBtw(800).SetBesteldatum(DateTime.Now.AddDays(-2)).Create()
            });

            _betalingDataMapperMock.Setup(b => b.Insert(It.IsAny<Betaling>())).Returns(new Betaling());

            _betalingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Betaling, bool>>>())).Returns(new List<Betaling>()
            {
                new Betaling()
                {
                    Bedrag = 399.00m,
                    BetaalDatum = DateTime.Now.AddDays(-1)
                },
                new Betaling()
                {
                    Bedrag = 300.0m,
                    BetaalDatum = DateTime.Now.AddDays(-3)
                }
            });

            // Act
            _target.HandleBetalingVerwerken(new Betaling()
            {
                Factuurnummer = 1
            });

            // Assert
            _betalingDataMapperMock.VerifyAll();
            _bestellingDataMapperMock.VerifyAll();
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Exactly(0));
        }

        [TestMethod]
        public void BetalingVerwerken_ShouldPublish2Events_When2BestellingenAreBelow500()
        {
            // Arrange
            _bestellingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Bestelling, bool>>>())).Returns(new List<Bestelling>()
            {
                new Bestelling()
                {
                    Factuurnummer = 1,
                    FactuurTotaalInclBtw = 300.0m,
                    Klantnummer = 1,
                    Besteldatum = DateTime.Now.AddDays(-2),
                    BestelStatus = BestelStatus.Betaald
                },
                new Bestelling()
                {
                    Factuurnummer = 2,
                    FactuurTotaalInclBtw = 500.0m,
                    Klantnummer = 1,
                    Besteldatum = DateTime.Now.AddDays(-1),
                    BestelStatus = BestelStatus.Goedgekeurd
                }
            });

            _betalingDataMapperMock.Setup(b => b.Insert(It.IsAny<Betaling>())).Returns(new Betaling());

            _betalingDataMapperMock.Setup(b => b.Find(It.IsAny<Expression<Func<Betaling, bool>>>())).Returns(new List<Betaling>()
            {
                new Betaling()
                {
                    Bedrag = 300.0m,
                    BetaalDatum = DateTime.Now.AddDays(-1)
                },
                new Betaling()
                {
                    Bedrag = 300.0m,
                    BetaalDatum = DateTime.Now.AddDays(-3)
                }

            });
            
            // Act
            _target.HandleBetalingVerwerken(new Betaling()
            {
                Factuurnummer = 1
            });

            // Assert
            _betalingDataMapperMock.VerifyAll();
            _bestellingDataMapperMock.VerifyAll();
            _eventPublisherMock.Verify(x => x.Publish(It.IsAny<DomainEvent>()), Times.Exactly(2));
        }
        [TestMethod]
        public void HandleBetaling_ShouldThrowDatabaseException()
        {
            _betalingDataMapperMock.Setup(b => b.Insert(It.IsAny<Betaling>())).Throws(new Exception());
            _bestellingDataMapperMock.Setup(b => b.GetByFactuurnummer(0)).Returns(new Bestelling()
            {
                Klantnummer = 1
            });
            Action action = () => _target.HandleBetalingVerwerken(new Betaling());

            Exception ex = Assert.ThrowsException<InvalidFactuurnummerException>(action);
            Assert.AreEqual("Something unexpected happend while inserting into the database", ex.Message);
        }
    }
}
