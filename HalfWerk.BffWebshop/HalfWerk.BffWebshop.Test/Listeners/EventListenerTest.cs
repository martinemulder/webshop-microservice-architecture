using System.Collections.Generic;
using AutoMapper;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.BffWebshop.Listeners;
using HalfWerk.BffWebshop.Test.Entities;
using HalfWerk.BffWebshop.Test.Entities.Artikel;
using HalfWerk.BffWebshop.Test.Entities.Bestellingen;
using HalfWerk.CommonModels.BffWebshop;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsKlantBeheer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Moq;
using Newtonsoft.Json;
using Artikel = HalfWerk.CommonModels.BffWebshop.Artikel;
using DsBestelServiceBestelling = HalfWerk.CommonModels.DsBestelService.Models.Bestelling;
using DsBestelServiceBestelStatus = HalfWerk.CommonModels.DsBestelService.Models.BestelStatus;

namespace HalfWerk.BffWebshop.Test.Listeners

{
    [TestClass]
    public class EventListenerTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            ConfigAutoMapper.Initialize();
        }

        [TestMethod]
        public void ReceiveAddArtikelToCatalogusWithCompleteEventMessageShouldAddArtikelsToTheDatabase()
        {
            // Arrange
            ArtikelEntity insertParam = null;

            var mock = new Mock<IArtikelDataMapper>();
            mock.Setup(repo => repo.Insert(It.IsAny<ArtikelEntity>())).Returns(insertParam)
                .Callback<ArtikelEntity>(entity =>
                {
                    insertParam = entity;
                });                

            ArtikelEntity expected = new ArtikelEntityBuilder().SetDummy().SetDummyCategorie("Cat1").SetDummyCategorie("Cat2").Create();
            Artikel artikel = expected.ToArtikel();

            var auditLogListener = new EventListener(mock.Object, null, null);
            var eventMessage = new EventMessage("", JsonConvert.SerializeObject(artikel));

            // Act
            auditLogListener.ReceiveAddArtikelToCatalogus(eventMessage);

            // Assert
            Assert.IsTrue(expected.IsEqual(insertParam));
        }

        [TestMethod]
        public void HandleVoorraadChanged_ShouldUpdateVoorraadOnVoorraadDataMapper()
        {
            // Arrange
            ArtikelEntity artikel = new ArtikelEntityBuilder()
                .SetDummy()
                .SetVoorraad(10)
                .SetDummyCategorie("Cat1")
                .SetDummyCategorie("Cat2")
                .Create();

            var mock = new Mock<IArtikelDataMapper>();
            mock.Setup(repo => repo.GetById(1)).Returns(artikel);

            ArtikelEntity result = null;
            mock.Setup(repo => repo.Update(It.IsAny<ArtikelEntity>()))
                .Callback<ArtikelEntity>(entity =>
                {
                    result = entity;
                });

            var voorraad = new Voorraad {Artikelnummer = 1, Aantal = 10, NieuweVoorraad = 6};

            var auditLogListener = new EventListener(mock.Object, null, null);
            var eventMessage = new EventMessage("", JsonConvert.SerializeObject(voorraad));

            // Act
            auditLogListener.HandleVoorraadChanged(eventMessage);

            // Assert
            Assert.AreEqual(voorraad.NieuweVoorraad, result.Voorrraad);
        }

        [TestMethod]
        public void HandleVoorraadChanged_ShouldSetVoorraadTo_8_WhenNieuweVoorraadIsGreatherThen_8()
        {
            // Arrange
            ArtikelEntity artikel = new ArtikelEntityBuilder()
                .SetDummy()
                .SetVoorraad(10)
                .SetDummyCategorie("Cat1")
                .SetDummyCategorie("Cat2")
                .Create();

            var mock = new Mock<IArtikelDataMapper>();
            mock.Setup(repo => repo.GetById(1)).Returns(artikel);

            ArtikelEntity result = null;
            mock.Setup(repo => repo.Update(It.IsAny<ArtikelEntity>()))
                .Callback<ArtikelEntity>(entity =>
                {
                    result = entity;
                });

            var voorraad = new Voorraad { Artikelnummer = 1, Aantal = 10, NieuweVoorraad = 9 };

            var auditLogListener = new EventListener(mock.Object, null, null);
            var eventMessage = new EventMessage("", JsonConvert.SerializeObject(voorraad));

            // Act
            auditLogListener.HandleVoorraadChanged(eventMessage);

            // Assert
            Assert.AreEqual(Constants.MaxVoorraad, result.Voorrraad);
        }

        [TestMethod]
        public void ReceiveAddKlant_WithCompleteEvent_ShouldAddKlantToDatabase()
        {
            // Arrange
            Klant insertParam = null;

            var mock = new Mock<IKlantDataMapper>();
            mock.Setup(repo => repo.Insert(It.IsAny<Klant>())).Returns(insertParam)
                .Callback<Klant>(entity =>
                {
                    insertParam = entity;
                });

            Klant expected = new KlantBuilder().SetDummy().Create();
            
            var eventmessage = new KlantToegevoegdEvent(Mapper.Map<CommonModels.DsKlantBeheer.Models.Klant>(expected), "");

            var listener = new EventListener(null, mock.Object, null);

            // Act
            listener.ReceiveKlantToegevoegdEvent(eventmessage);

            // Assert
            Assert.AreEqual(expected.Achternaam, insertParam.Achternaam);
        }

        [TestMethod]
        public void ReceiveBestelling_WithCompleteEvent_ShouldAddBevestigdeBestellingToDatabase()
        {
            // Arrange
            BevestigdeBestelling insertParam = null;

            var bestellingMock = new Mock<IBestellingDataMapper>();
            bestellingMock.Setup(repo => repo.Insert(It.IsAny<BevestigdeBestelling>())).Returns(insertParam)
                .Callback<BevestigdeBestelling>(entity =>
                {
                    insertParam = entity;
                    insertParam.Id = 1;

                });
            var artikelMock = new Mock<IArtikelDataMapper>();
            artikelMock.Setup(dm => dm.GetById(It.IsAny<int>())).Returns(new ArtikelEntity()
            {
                AfbeeldingUrl = "test"
            });
            BevestigdeBestelling expected = new BevestigdeBestellingBuilder().SetDummy().Create();

            var eventmessage = new BestellingGeplaatstEvent(Mapper.Map<CommonModels.DsBestelService.Models.Bestelling>(expected), "");

            var listener = new EventListener(artikelMock.Object, null, bestellingMock.Object);

            // Act
            listener.ReceiveBestellingGeplaatstEvent(eventmessage);

            // Assert
            Assert.AreEqual(expected.BestelStatus, insertParam.BestelStatus);
        }

        [TestMethod]
        public void ReceiveUpdateBestelling_WithCompleteBestelling_ShouldUpdateBestelling()
        {
            // Arrange
            long bestellingId = 10;
            BevestigdeBestelling bestelling = new BevestigdeBestellingBuilder()
                .SetDummy()
                .SetId(bestellingId)
                .SetBestelRegels(new List<BevestigdeBestelRegel>
                {
                    new BevestigdeBestelRegel
                    {
                        PrijsExclBtw = 10m,
                        PrijsInclBtw = 10.10m
                    }
                })
                .SetFactuurTotaalExclBtw(42m)
                .SetFactuurTotaalInclBtw(42.42m)
                .Create();

            var mock = new Mock<IBestellingDataMapper>();
            var artikelMock = new Mock<IArtikelDataMapper>();

            BevestigdeBestelling result = null;
            mock.Setup(repo => repo.GetById(bestellingId)).Returns(bestelling);
            mock.Setup(repo => repo.Update(It.IsAny<BevestigdeBestelling>()))
                .Callback<BevestigdeBestelling>(b => result = b);

            var updatedBestelling = new DsBestelServiceBestelling { Id = bestellingId, BestelStatus = DsBestelServiceBestelStatus.Goedgekeurd };
            var eventmessage = new BestelStatusBijgewerktEvent(updatedBestelling, "");

            var listener = new EventListener(artikelMock.Object, null, mock.Object);

            // Act
            listener.ReceiveBestelStatusBijgewerktEvent(eventmessage);

            // Assert
            mock.VerifyAll();
            Assert.AreEqual(bestelling, result);
        }
    }
}
