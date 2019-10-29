using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Listeners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.WebScale.Events;
using Moq;
using Newtonsoft.Json;
using System;

namespace HalfWerk.DsBestelService.Test.Listeners
{
    [TestClass]
    public class EventListenerTest
    {
        private Mock<IArtikelDataMapper> _artikelDataMapperMock;
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;

        private EventListener _target;
        private Mock<IEventPublisher> _eventPublisherMock;

        [TestInitialize]
        public void BeforeEach()
        {
            _artikelDataMapperMock = new Mock<IArtikelDataMapper>(MockBehavior.Strict);
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);

            _target = new EventListener(_artikelDataMapperMock.Object, _bestellingDataMapperMock.Object, _eventPublisherMock.Object);
        }

        [TestMethod]
        public void HandleArtikelToegevoegdEvent_ShouldHandleReplayMessage()
        {
            Artikel artikel = new Artikel
            {
                Artikelnummer = 1,
                Leveranciercode = "1-abc-2",
                LeverbaarTot = new DateTime(2018, 11, 10),
                Naam = "Fietsband",
                Prijs = 10.99m
            };

            _artikelDataMapperMock.Setup(repo => repo.Insert(It.Is<Artikel>(a => 
                a.Artikelnummer == artikel.Artikelnummer
                && a.Leveranciercode == artikel.Leveranciercode
                && a.LeverbaarTot == artikel.LeverbaarTot
                && a.Naam == artikel.Naam
                && a.Prijs == artikel.Prijs
            ))).Returns(artikel);

            var eventMessage = new EventMessage("", JsonConvert.SerializeObject(artikel));
            _target.HandleArtikelToegevoegdEvent(eventMessage);

            _artikelDataMapperMock.VerifyAll();
        }

        [TestMethod]
        public void HandleBetalingGeaccrediteerdEvent_ShouldSetBestelStatusToGoedGekeurd_WhenBestelStatusSmallerThenBetaald()
        {
            var bestelling = new Bestelling { BestelStatus = BestelStatus.Geplaatst };

            var request = new BestellingGeaccrediteerdEvent(10, NameConstants.BetaalServiceBetalingGeaccrediteerdEvent);
            _bestellingDataMapperMock.Setup(d => d.GetByFactuurnummer(request.Factuurnummer)).Returns(bestelling);
            _bestellingDataMapperMock.Setup(d => d.Update(bestelling));

            _eventPublisherMock.Setup(p => p.Publish(It.Is<BestelStatusBijgewerktEvent>(e => 
                e.Bestelling == bestelling 
                && e.RoutingKey == NameConstants.BestelServiceBestelStatusUpgedateEvent
                && e.Bestelling.BestelStatus == BestelStatus.Goedgekeurd
            )));

            _target.HandleBetalingGeaccrediteerdEvent(request);

            _bestellingDataMapperMock.VerifyAll();
            _eventPublisherMock.VerifyAll();
        }

        [TestMethod]
        public void HandleBetalingGeaccrediteerdEvent_ShouldSetBestelStatusToAfgerond_WhenBestelStatusIsVerzonden()
        {
            var bestelling = new Bestelling { BestelStatus = BestelStatus.Verzonden };

            var request = new BestellingGeaccrediteerdEvent(10, NameConstants.BetaalServiceBetalingGeaccrediteerdEvent);
            _bestellingDataMapperMock.Setup(d => d.GetByFactuurnummer(request.Factuurnummer)).Returns(bestelling);
            _bestellingDataMapperMock.Setup(d => d.Update(bestelling));

            _eventPublisherMock.Setup(p => p.Publish(It.Is<BestelStatusBijgewerktEvent>(e =>
                e.Bestelling == bestelling
                && e.RoutingKey == NameConstants.BestelServiceBestelStatusUpgedateEvent
                && e.Bestelling.BestelStatus == BestelStatus.Afgerond
            )));

            _target.HandleBetalingGeaccrediteerdEvent(request);

            _bestellingDataMapperMock.VerifyAll();
            _eventPublisherMock.VerifyAll();
        }
    }
}
