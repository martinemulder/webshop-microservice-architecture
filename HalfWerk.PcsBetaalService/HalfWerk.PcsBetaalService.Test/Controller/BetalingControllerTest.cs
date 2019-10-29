using HalfWerk.CommonModels.PcsBetalingService;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.Controllers;
using HalfWerk.PcsBetaalService.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.WebScale.Events;
using Moq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.PcsBetaalService.Test.Controller
{
    [TestClass]
    public class BetalingControllerTest
    {
        private Mock<IBetalingVerwerkenService> _betalingVerwerkenServiceMock;
        private BetalingController _target;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            AutoMapperConfiguration.Configure();
        }
        [TestInitialize]
        public void BeforeEach()
        {
            _betalingVerwerkenServiceMock = new Mock<IBetalingVerwerkenService>(MockBehavior.Strict);
            

            _target = new BetalingController(_betalingVerwerkenServiceMock.Object);
        }

        [TestMethod]
        public void HandleVerwerkBetalingCommand_ShouldReturnFactuurnummer()
        {
            // Arrange
            var commandModel = new BetalingCM(1, 400);
            var command = new VerwerkBetalingCommand(commandModel, "");

            _betalingVerwerkenServiceMock.Setup(b => b.HandleBetalingVerwerken(It.IsAny<Betaling>()));

            // Act
            var result = _target.HandleVerWerkBetalingCommand(command);

            // Assert 
            _betalingVerwerkenServiceMock.VerifyAll();
            Assert.AreEqual(1, result);
        }
        
    }
}
