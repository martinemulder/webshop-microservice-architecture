using System.Linq;
using HalfWerk.CommonModels;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.TestBus;
using Minor.Nijn.WebScale.Events;
using Moq;
using RabbitMQ.Client;

namespace HalfWerk.DsBestelService.Test.Services
{
    [TestClass]
    public class EventReplayServiceTest
    {
        private ITestBusContext _nijnContext;
        private Mock<IConnection> _connectionMock;
        private Mock<IModel> _channelMock;
        private Mock<IArtikelDataMapper> _artikelDataMapperMock;
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;
        private Mock<IEventPublisher> _eventPublisherMock;

        private EventReplayService _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            _channelMock = new Mock<IModel>(MockBehavior.Strict);
            _artikelDataMapperMock = new Mock<IArtikelDataMapper>(MockBehavior.Loose);
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);

            _connectionMock.Setup(conn => conn.CreateModel()).Returns(_channelMock.Object);

            _nijnContext = new TestBusContextBuilder()
                .WithMockConnection(_connectionMock.Object)
                .CreateTestContext();

            var services = new ServiceCollection();
            services.AddSingleton(_artikelDataMapperMock.Object);
            services.AddSingleton(_bestellingDataMapperMock.Object);
            services.AddSingleton(_eventPublisherMock.Object);

            _target = new EventReplayService(_nijnContext, services, 1000);
        }

        [TestMethod]
        public void StartEventReplay_ShouldRegisterListenersToTheExchange()
        {
            _target.StartEventReplay();

            _channelMock.VerifyAll();
            _connectionMock.VerifyAll();

            var replayEventQueue = _nijnContext.EventBus.Queues[NameConstants.BestelServiceEventReplayQueue];
            Assert.IsNotNull(replayEventQueue, "ReplayEventQueue should be declared");
            Assert.IsTrue(replayEventQueue.TopicExpressions.Contains(NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent));
        }

        [TestMethod]
        public void StartEventReplay_ShouldSendCommand()
        {
            _nijnContext.CommandBus.DeclareCommandQueue(NameConstants.AuditlogQueue);
            _target.StartEventReplay();

            _channelMock.VerifyAll();
            _connectionMock.VerifyAll();

            Assert.AreEqual(1, _nijnContext.CommandBus.Queues[NameConstants.AuditlogQueue].MessageQueueLength);
        }

        [TestMethod]
        public void StartEventReplay_ShouldUnbindResourcesWhenDone()
        {
            _channelMock.Setup(chan => chan.QueueDelete(NameConstants.BestelServiceEventReplayQueue, false, false)).Returns(1);
            _channelMock.Setup(chan => chan.ExchangeDelete(NameConstants.BestelServiceEventReplayExchange, false));
            _channelMock.Setup(chan => chan.Dispose());
            

            _target.StartEventReplay();
            _target.Dispose();

            _channelMock.VerifyAll();
            _connectionMock.VerifyAll();
        }

        [TestMethod]
        public void StartEventReplay_ShouldNotUnbindResourcesAgainWhenAlreadyDisposed()
        {
            _channelMock.Setup(chan => chan.QueueDelete(NameConstants.BestelServiceEventReplayQueue, false, false)).Returns(1);
            _channelMock.Setup(chan => chan.ExchangeDelete(NameConstants.BestelServiceEventReplayExchange, false));
            _channelMock.Setup(chan => chan.Dispose());


            _target.StartEventReplay();
            _target.Dispose();

            _channelMock.Verify(chan => chan.Dispose(), Times.Once);
            _connectionMock.VerifyAll();
        }
    }
}