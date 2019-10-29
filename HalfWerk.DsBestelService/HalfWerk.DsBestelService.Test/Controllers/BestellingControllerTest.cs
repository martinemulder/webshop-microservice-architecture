using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.Controllers;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Events;
using Moq;
using System;
using System.Collections.Generic;

namespace HalfWerk.DsBestelService.Test.Controllers
{
    [TestClass]
    public class BestellingControllerTest
    {
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;
        private Mock<IArtikelDataMapper> _artikelDataMapperMock;
        private Mock<IEventPublisher> _eventPublisherMock;

        private BestellingController _target;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            AutoMapperConfiguration.Configure();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _artikelDataMapperMock = new Mock<IArtikelDataMapper>(MockBehavior.Strict);
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);

            _target = new BestellingController(
                bestellingDataMapper: _bestellingDataMapperMock.Object,
                artikelDataMapper: _artikelDataMapperMock.Object,
                eventPublisher: _eventPublisherMock.Object, 
                loggerFactory: new LoggerFactory()
            );
        }

        [TestMethod]
        public void HandlePlaatsBestelling_ShouldCallInsertOnDataMapper()
        {
            var artikel = new ArtikelBuilder()
                .SetArtikelNummer(1)
                .SetLeveranciercode("1-abc-xyz")
                .SetNaam("Fietsband")
                .SetPrijs(12.99m)
                .Create();

            var bestellingCM = new BestellingCM
            {
                Klantnummer = 1234,
                ContactInfo = new ContactInfoCM(),
                Afleveradres = new AfleveradresCM(),
                BestelRegels = new List<BestelRegelCM>
                {
                    new BestelRegelCM { Artikelnummer = 1, Aantal = 2 },
                }
            };

            Bestelling bestelling = null;
            _artikelDataMapperMock.Setup(a => a.GetById(1)).Returns(artikel);

            _bestellingDataMapperMock.Setup(m => m.Insert(It.IsAny<Bestelling>()))
                .Callback<Bestelling>(b => { b.Id = 42; bestelling = b; })
                .Returns(bestelling);

            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()))
                .Callback<Bestelling>(b => bestelling = b);

            _eventPublisherMock.Setup(p => p.Publish(It.IsAny<DomainEvent>()));

            var requestCommand = new PlaatsBestellingCommand(bestellingCM, NameConstants.BestelServicePlaatsBestellingCommandQueue);
            var result = _target.HandlePlaatsBestelling(requestCommand);

            _bestellingDataMapperMock.VerifyAll();
            _artikelDataMapperMock.VerifyAll();
            _eventPublisherMock.VerifyAll();

            Assert.AreEqual(42, result);
            Assert.AreEqual(42, bestelling.Factuurnummer);
            Assert.AreEqual(DateTime.Now.Date, bestelling.Besteldatum.Date);
            Assert.AreEqual(BestelStatus.Geplaatst, bestelling.BestelStatus);
        }

        [TestMethod]
        public void HandlePlaatsBestelling_ShouldThrowDatabaseExceptionWhenInsertFailed()
        {
            var bestelling = new BestellingCM();
            _bestellingDataMapperMock.Setup(b => b.Insert(It.IsAny<Bestelling>())).Throws(new DatabaseException("error"));

            var requestCommand = new PlaatsBestellingCommand(bestelling, NameConstants.BestelServicePlaatsBestellingCommandQueue);

            Action action = () => _target.HandlePlaatsBestelling(requestCommand);

            _bestellingDataMapperMock.Verify(d => d.Insert(It.IsAny<Bestelling>()), Times.Never);
            _eventPublisherMock.Verify(e => e.Publish(It.IsAny<DomainEvent>()), Times.Never);

            var ex = Assert.ThrowsException<DatabaseException>(action);
            Assert.AreEqual("Something unexpected happend while inserting into the database", ex.Message);
        }

        [TestMethod]
        public void HandlePlaatsBestelling_ShouldPublishBestellingGeplaatstEvent()
        {
            var artikel = new ArtikelBuilder()
                .SetArtikelNummer(1)
                .SetLeveranciercode("1-abc-xyz")
                .SetNaam("Fietsband")
                .SetPrijs(12.99m)
                .Create();

            var bestellingCM = new BestellingCM
            {
                Klantnummer = 1234,
                ContactInfo = new ContactInfoCM(),
                Afleveradres = new AfleveradresCM(),
                BestelRegels = new List<BestelRegelCM>
                {
                    new BestelRegelCM { Artikelnummer = 1, Aantal = 2 },
                }
            };

            Bestelling bestelling = null;
            _artikelDataMapperMock.Setup(a => a.GetById(1)).Returns(artikel);

            _bestellingDataMapperMock.Setup(m => m.Insert(It.IsAny<Bestelling>()))
                .Callback<Bestelling>(b => { b.Id = 42; bestelling = b; })
                .Returns(bestelling);

            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()));

            _eventPublisherMock.Setup(e => e.Publish(It.Is<BestellingGeplaatstEvent>(evt =>
                evt.RoutingKey == NameConstants.BestelServiceBestellingGeplaatstEvent
                && evt.Bestelling == bestelling
            ))).Verifiable("Publish event should be called");

            var requestCommand = new PlaatsBestellingCommand(bestellingCM, NameConstants.BestelServicePlaatsBestellingCommandQueue);
            _target.HandlePlaatsBestelling(requestCommand);

            _bestellingDataMapperMock.VerifyAll();
            _artikelDataMapperMock.VerifyAll();
            _eventPublisherMock.VerifyAll();
        }

        [TestMethod]
        public void HandlePlaatsBestelling_ShouldCalculateBestellingTotaal()
        {
            var artikel1 = new ArtikelBuilder()
                .SetArtikelNummer(1)
                .SetLeveranciercode("1-abc-xyz")
                .SetNaam("Fietsband")
                .SetPrijs(12.99m)
                .Create();

            var artikel2 = new ArtikelBuilder()
                .SetArtikelNummer(2)
                .SetLeveranciercode("2-abc-xyz")
                .SetNaam("Ventieldopje")
                .SetPrijs(.99m)
                .Create();

            _artikelDataMapperMock.Setup(a => a.GetById(1)).Returns(artikel1);
            _artikelDataMapperMock.Setup(a => a.GetById(2)).Returns(artikel2);

            Bestelling bestelling = null;
            _bestellingDataMapperMock.Setup(m => m.Insert(It.IsAny<Bestelling>()))
                .Callback<Bestelling>(b => { b.Id = 42; bestelling = b; })
                .Returns(bestelling);

            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()));

            BestellingGeplaatstEvent result = null;
            _eventPublisherMock.Setup(p => p.Publish(It.IsAny<DomainEvent>()))
                .Callback<DomainEvent>(b => result = (BestellingGeplaatstEvent) b);

            var bestellingCM = new BestellingCM
            {
                Klantnummer = 1234,
                ContactInfo = new ContactInfoCM(),
                Afleveradres = new AfleveradresCM(),
                BestelRegels = new List<BestelRegelCM>
                {
                    new BestelRegelCM { Artikelnummer = 1, Aantal = 2 },
                    new BestelRegelCM { Artikelnummer = 2, Aantal = 1 }
                }
            };

            var requestCommand = new PlaatsBestellingCommand(bestellingCM, NameConstants.BestelServicePlaatsBestellingCommandQueue);
            _target.HandlePlaatsBestelling(requestCommand);

            _bestellingDataMapperMock.VerifyAll();
            _artikelDataMapperMock.VerifyAll();
            _eventPublisherMock.VerifyAll();

            Assert.AreEqual(25.98m, result.Bestelling.BestelRegels[0].RegelTotaalExclBtw);
            Assert.AreEqual(31.44m, result.Bestelling.BestelRegels[0].RegelTotaalInclBtw);

            Assert.AreEqual(26.97m, result.Bestelling.FactuurTotaalExclBtw);
            Assert.AreEqual(32.64m, result.Bestelling.FactuurTotaalInclBtw);
        }
    }
}
