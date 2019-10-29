using HalfWerk.CommonModels;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.Controllers;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.WebScale.Events;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalfWerk.DsBestelService.Test.Controllers
{
    [TestClass]
    public class BestelStatusControllerTest
    {
        private Mock<IBusContext<IConnection>> _nijnContextMock;
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;
        private Mock<IEventPublisher> _eventPublisherMock;
        private Mock<ICommandSender> _commandSenderMock;

        private BestelStatusController _target;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            AutoMapperConfiguration.Configure();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _nijnContextMock = new Mock<IBusContext<IConnection>>(MockBehavior.Strict);
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
            _commandSenderMock = new Mock<ICommandSender>(MockBehavior.Strict);

            _nijnContextMock.Setup(ctx => ctx.CreateCommandSender()).Returns(_commandSenderMock.Object);

            _target = new BestelStatusController(_nijnContextMock.Object, _bestellingDataMapperMock.Object, _eventPublisherMock.Object, new LoggerFactory());
        }

        [TestMethod]
        public async Task HandleUpdateBestelStatus_ShouldCallUpdateOnDataMapper()
        {
            // Arrange
            Bestelling bestelling = null;
            var bestelStatusCommand = new UpdateBestelStatusCommand(1, BestelStatus.Goedgekeurd, NameConstants.BestelServiceUpdateBestelStatusCommandQueue);

            _bestellingDataMapperMock.Setup(m => m.GetByFactuurnummer(1)).Returns(new Bestelling { BestelStatus = BestelStatus.Geplaatst });
            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()))
                .Callback<Bestelling>(b => bestelling = b);

            _eventPublisherMock.Setup(publisher => publisher.Publish(It.IsAny<DomainEvent>()));
            
            // Act
            var result = await _target.HandleUpdateBestelStatus(bestelStatusCommand);

            // Assert
            _bestellingDataMapperMock.VerifyAll();
            Assert.AreEqual(1, result);
            Assert.AreEqual(BestelStatus.Goedgekeurd, bestelling.BestelStatus);
        }

        [TestMethod]
        public async Task HandleUpdateBestelStatus_ShouldThrowAnExcpetionWhenFindBestellingFailed()
        {
            // Arrange
            var bestelStatusCommand = new UpdateBestelStatusCommand(1, BestelStatus.Geplaatst, NameConstants.BestelServiceUpdateBestelStatusCommandQueue);

            _bestellingDataMapperMock.Setup(m => m.GetByFactuurnummer(1)).Throws(new Exception());

            // Act
            Func<Task> action = async () => await _target.HandleUpdateBestelStatus(bestelStatusCommand);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<DatabaseException>(action);
            Assert.AreEqual("Something unexpected happend while find bestelling in the database", ex.Message);
        }

        [TestMethod]
        public async Task HandleUpdateBestelStatus_ShouldThrowAnExceptionWhenUpdateFailed()
        {
            // Arrange
            var bestelStatusCommand = new UpdateBestelStatusCommand(1, BestelStatus.Goedgekeurd, NameConstants.BestelServiceUpdateBestelStatusCommandQueue);

            _bestellingDataMapperMock.Setup(m => m.GetByFactuurnummer(1)).Returns(new Bestelling());
            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()))
                .Throws(new Exception());

            // Act
            Func<Task> action = async () => await _target.HandleUpdateBestelStatus(bestelStatusCommand);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<DatabaseException>(action);
            Assert.AreEqual("Something unexpected happend while updating the database", ex.Message);
        }

        [TestMethod]
        public async Task HandleUpdateBestelStatus_ShouldThrowAnExceptionWhenBestelStatusUpdateIsNotAllowed()
        {
            // Arrange
            var bestelStatusCommand = new UpdateBestelStatusCommand(1, BestelStatus.Afgerond, NameConstants.BestelServiceUpdateBestelStatusCommandQueue);

            var existing = new Bestelling {BestelStatus = BestelStatus.Geplaatst};
            _bestellingDataMapperMock.Setup(m => m.GetByFactuurnummer(1)).Returns(existing);
            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()))
                .Throws(new Exception());

            // Act
            Func<Task> action = async () => await _target.HandleUpdateBestelStatus(bestelStatusCommand);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidUpdateException>(action);
            Assert.AreEqual("BestelStatus update not allowed", ex.Message);
        }

        [TestMethod]
        public async Task HandleUpdateBestelStatus_ShouldLowerStockWhenBestelStatusVerzonden()
        {
            // Arrange
            var artikel = new ArtikelBuilder()
                .SetArtikelNummer(1)
                .SetLeveranciercode("1-abc-xyz")
                .SetNaam("Fietsband")
                .SetPrijs(12.99m)
                .Create();

            var existing = new Bestelling
            {
                Klantnummer = 1234,
                BestelStatus = BestelStatus.WordtIngepakt,
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>
                {
                    new BestelRegel { Artikelnummer = 1, Aantal = 2 },
                }
            };

            var bestelStatusCommand = new UpdateBestelStatusCommand(1, BestelStatus.Verzonden, NameConstants.BestelServiceUpdateBestelStatusCommandQueue);

            _bestellingDataMapperMock.Setup(m => m.GetByFactuurnummer(1)).Returns(existing);
            _bestellingDataMapperMock.Setup(m => m.Update(It.IsAny<Bestelling>()));

            _eventPublisherMock.Setup(publisher => publisher.Publish(It.IsAny<DomainEvent>()));

            var response = new ResponseCommandMessage(JsonConvert.SerializeObject(true), "type", "correlationId");
            _commandSenderMock.Setup(sendr => sendr.SendCommandAsync(It.Is<RequestCommandMessage>(cm =>
                cm.RoutingKey == NameConstants.MagazijnServiceCommandQueue && cm.Type == NameConstants.MagazijnServiceHaalVoorraadUitMagazijnCommand
            ))).ReturnsAsync(response);

            // Act
            await _target.HandleUpdateBestelStatus(bestelStatusCommand);

            // Assert
            _nijnContextMock.VerifyAll();
            _bestellingDataMapperMock.VerifyAll();
            _eventPublisherMock.VerifyAll();
            _commandSenderMock.VerifyAll();
        }
    }
}
