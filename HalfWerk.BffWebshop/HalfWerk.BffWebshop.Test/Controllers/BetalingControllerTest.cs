using HalfWerk.BffWebshop.Controllers;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.CommonModels.PcsBetalingService;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Commands;
using Moq;
using System;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Test.Controllers
{
    [TestClass]
    public class BetalingControllerTest
    {
        private Mock<ICommandPublisher> _commandPublisherMock;

        private BetalingController _target;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            ConfigAutoMapper.Initialize();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);

            _target = new BetalingController(_commandPublisherMock.Object, new LoggerFactory());
        }

        [TestMethod]
        public async Task BetalingVerwerken_ShouldReturn200StatusCode()
        {
            // Arrange
            var betaling = new BetalingCM(1, 200);

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<VerwerkBetalingCommand>()))
                .ReturnsAsync(1);

            // Act
            var result = await _target.BetalingVerwerken(betaling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as StatusCodeResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(200, requestResult.StatusCode);
        }

        [TestMethod]
        public async Task BetalingVerwerken_ShouldReturnBadRequestOnInvalidFactuurnummerException()
        {
            // Arrange
            var betaling = new BetalingCM(1, 200);

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<VerwerkBetalingCommand>()))
                .Throws(new InvalidFactuurnummerException(""));

            // Act
            var result = await _target.BetalingVerwerken(betaling);

            // Assert
            _commandPublisherMock.VerifyAll();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.AreEqual("Factuurnummer 1 kon niet worden gevonden", objectResult.Value);
        }

        [TestMethod]
        public async Task BetalingVerwerken_ShouldReturn408OnTimeout()
        {
            // Arrange
            var betaling = new BetalingCM(1, 200);

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<VerwerkBetalingCommand>()))
                .Throws<TimeoutException>();

            // Act
            var result = await _target.BetalingVerwerken(betaling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(408, requestResult.StatusCode);
        }

        [TestMethod]
        public async Task BetalingVerwerken_ShouldReturn500OnUnknownException()
        {
            // Arrange
            var betaling = new BetalingCM(1, 200);

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<VerwerkBetalingCommand>()))
                .Throws<InsufficientExecutionStackException>();

            // Act
            var result = await _target.BetalingVerwerken(betaling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as StatusCodeResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(500, requestResult.StatusCode);
        }
    }
}
