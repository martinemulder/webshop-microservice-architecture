using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Listeners;
using HalfWerk.PcsBetaalService.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.PcsBetaalService.Test.Listeners
{
    [TestClass]
    public class EventListenerTest
    {
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;
        private Mock<IBetalingVerwerkenService> _betalingVerwerkenServiceMock;

        private EventListener _target;
        
        [TestInitialize]
        public void BeforeEach()
        {
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _betalingVerwerkenServiceMock = new Mock<IBetalingVerwerkenService>(MockBehavior.Strict);

            _target = new EventListener(_bestellingDataMapperMock.Object, _betalingVerwerkenServiceMock.Object);
        }

        [TestMethod]
        public void ReceiveBestellingGeplaatstEvent_ShouldHandleBestellingMessage()
        {
            // Arrange
            var bestellingGeplaatstEvent = new BestellingGeplaatstEvent(new Bestelling()
            {
                Factuurnummer = 1
            }, "");
            _bestellingDataMapperMock.Setup(b => b.Insert(It.IsAny<CommonModels.PcsBetalingService.Models.Bestelling>())).Returns(new CommonModels.PcsBetalingService.Models.Bestelling());
            _betalingVerwerkenServiceMock.Setup(b => b.HandleBestellingVerwerken(1));
            // Act

            _target.ReceiveBestellingGeplaatstEvent(bestellingGeplaatstEvent);

            // Assert
            _bestellingDataMapperMock.VerifyAll();

        }
    }
}
