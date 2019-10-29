using System;
using System.Threading;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Services;
using HalfWerk.CommonModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.TestBus;
using Minor.Nijn.TestBus.CommandBus;
using Moq;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace HalfWerk.BffWebshop.Test.Services
{
    [TestClass]
    public class EventReplayServiceTest
    {
        private ITestBusContext _nijnContext;
        private Mock<IConnection> _connectionMock;
        private Mock<IModel> _channelMock;

        private Mock<IArtikelDataMapper> _artikelDataMapperMock;
        private Mock<IKlantDataMapper> _klantDataMapperMock;
        private Mock<IBestellingDataMapper> _bestellingDataMapper;

        private EventReplayService _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            _channelMock = new Mock<IModel>(MockBehavior.Strict);

            _klantDataMapperMock = new Mock<IKlantDataMapper>(MockBehavior.Loose);
            _artikelDataMapperMock = new Mock<IArtikelDataMapper>(MockBehavior.Loose);
            _bestellingDataMapper = new Mock<IBestellingDataMapper>(MockBehavior.Loose);

            _connectionMock.Setup(conn => conn.CreateModel()).Returns(_channelMock.Object);

            _nijnContext = new TestBusContextBuilder()
                .WithConnectionTimeout(1000)
                .WithMockConnection(_connectionMock.Object)
                .CreateTestContext();

            var services = new ServiceCollection();
            services.AddSingleton(_artikelDataMapperMock.Object);
            services.AddSingleton(_klantDataMapperMock.Object);
            services.AddSingleton(_bestellingDataMapper.Object);

            _target = new EventReplayService(_nijnContext, services, 200);
        }

        [TestMethod]
        public void StartEventReplay_ShouldSendCommands()
        {
            _channelMock.Setup(chan => chan.QueueDelete(It.IsAny<string>(), false, false)).Returns(1);

            _nijnContext.CommandBus.DeclareCommandQueue(NameConstants.AuditlogQueue);
            _target.StartEventReplay();

            _connectionMock.VerifyAll();
            _channelMock.Verify(chan => chan.QueueDelete(It.IsAny<string>(), false, false), Times.Exactly(2));

            var queue = _nijnContext.CommandBus.Queues[NameConstants.AuditlogQueue];
            Assert.AreEqual(2, queue.MessageQueueLength);

            var command1 = queue[0].Command;
            var json1 = JObject.Parse(command1.Message);
            Assert.AreEqual(NameConstants.AuditlogQueue, command1.RoutingKey);
            Assert.AreEqual(NameConstants.AuditlogReplayCommandType, command1.Type);
            Assert.AreEqual(NameConstants.BffWebshopEventReplayExchange, json1["ExchangeName"]);
            Assert.AreEqual(NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent, json1["Topic"]);

            var command2 = queue[1].Command;
            var json2 = JObject.Parse(command2.Message);
            Assert.AreEqual(NameConstants.AuditlogQueue, command2.RoutingKey);
            Assert.AreEqual(NameConstants.AuditlogReplayCommandType, command2.Type);
            Assert.AreEqual(NameConstants.BffWebshopEventReplayExchange, json2["ExchangeName"]);
            Assert.AreEqual(NameConstants.MagazijnServiceVoorraadChangedEvent, json2["Topic"]);
        }

        [TestMethod]
        public void StartEventReplay_ShouldUnbindResourcesWhenDone()
        {
            _channelMock.Setup(chan => chan.QueueDelete(NameConstants.BffWebshopEventReplayQueue, false, false)).Returns(1);
            _channelMock.Setup(chan => chan.ExchangeDelete(NameConstants.BffWebshopEventReplayExchange, false));
            _channelMock.Setup(chan => chan.Dispose());


            _target.StartEventReplay();
            _target.Dispose();

            _channelMock.VerifyAll();
            _connectionMock.VerifyAll();
        }

        [TestMethod]
        public void StartEventReplay_ShouldNotUnbindResourcesAgainWhenAlreadyDisposed()
        {
            _channelMock.Setup(chan => chan.QueueDelete(NameConstants.BffWebshopEventReplayQueue, false, false)).Returns(1);
            _channelMock.Setup(chan => chan.ExchangeDelete(NameConstants.BffWebshopEventReplayExchange, false));
            _channelMock.Setup(chan => chan.Dispose());


            _target.StartEventReplay();
            _target.Dispose();

            _channelMock.Verify(chan => chan.Dispose(), Times.Once);
            _connectionMock.VerifyAll();
        }
    }
}
