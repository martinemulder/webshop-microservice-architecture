using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.CommonModels.PcsBetaalService.Models;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.DAL;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Listeners;
using HalfWerk.PcsBetaalService.Services;
using HalfWerk.PcsBetaalService.Test.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.TestBus;
using Minor.Nijn.WebScale.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.PcsBetaalService.Test.Intergration
{
    [TestClass]
    public class EventListenerIntergrationTest
    {
        private SqliteConnection _connection;
        private BetaalContext _dbContext;
        private ITestBusContext _nijnContext;

        private EventListener _target;
        private BestellingBuilder _bestellingBuilder;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            AutoMapperConfiguration.Configure();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _bestellingBuilder = new BestellingBuilder();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<BetaalContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _nijnContext = new TestBusContextBuilder().CreateTestContext();

            _dbContext = new BetaalContext(options);
            _dbContext.Database.EnsureCreated();
            SeedDatabase();
            var bestellingDataMapper = new BestellingDataMapper(_dbContext);
            var betalingDataMapper = new BetalingDataMapper(_dbContext);
            var eventPublisher = new EventPublisher(_nijnContext);

            var betalingVerwerkenService = new BetalingVerwerkenService(betalingDataMapper, bestellingDataMapper, eventPublisher, new LoggerFactory());


            _target = new EventListener(bestellingDataMapper, betalingVerwerkenService);

            _dbContext.Database.EnsureCreated();
        }

        [TestCleanup]
        public void AfterEach()
        {
            _dbContext.Dispose();
            _connection.Dispose();
        }

        private void SeedDatabase()
        {
            _dbContext.Bestellingen.AddRange(new List<Bestelling>
            {
                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(1)
                    .SetBestelStatus(BestelStatus.Geplaatst)
                    .SetBesteldatum(DateTime.Now.AddDays(-2))
                    .SetFactuurTotaalInclBtw(700)
                    .Create(),

                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(2)
                    .SetBestelStatus(BestelStatus.Goedgekeurd)
                    .SetFactuurTotaalInclBtw(700)
                    .Create()
            });

            _dbContext.SaveChanges();
        }

        [TestMethod]
        public void ReceiveBestellingGeplaatstEvent_ShouldAddBestellingToDatabase()
        {
            // Arrange
            var bestelling = new CommonModels.DsBestelService.Models.Bestelling()
            {
                FactuurTotaalInclBtw = 700,
                BestelStatus = CommonModels.DsBestelService.Models.BestelStatus.Geplaatst,
                Factuurnummer = 3,
                Klantnummer = 1,
                Besteldatum = DateTime.Now.AddDays(-2)
            };

            var request = new BestellingGeplaatstEvent(bestelling, NameConstants.BestelServiceBestellingGeplaatstEvent);

            // Act
            _target.ReceiveBestellingGeplaatstEvent(request);

            // Assert
            var dbResult = _dbContext.Bestellingen.SingleOrDefault(b => b.Factuurnummer == 3);
            Assert.AreEqual(bestelling.FactuurTotaalInclBtw, dbResult.FactuurTotaalInclBtw);
            Assert.AreEqual(bestelling.BestelStatus.ToString(), dbResult.BestelStatus.ToString());
            Assert.AreEqual(bestelling.Factuurnummer, dbResult.Factuurnummer);
            Assert.AreEqual(bestelling.Klantnummer, dbResult.Klantnummer);
            Assert.AreEqual(bestelling.Besteldatum, dbResult.Besteldatum);
        }

        [TestMethod]
        public void ReceiveBestellingGeplaatstEvent_ShouldAddBestellingToDatabaseAndRaiseEvent()
        {
            // Arrange
            var queueName = "TestQueue";
            var receiver = _nijnContext.CreateMessageReceiver(queueName, new List<string> { NameConstants.BetaalServiceBetalingGeaccrediteerdEvent });
            receiver.DeclareQueue();

            var bestelling = new CommonModels.DsBestelService.Models.Bestelling()
            {
                FactuurTotaalInclBtw = 400,
                BestelStatus = CommonModels.DsBestelService.Models.BestelStatus.Geplaatst,
                Factuurnummer = 3,
                Klantnummer = 1,
                Besteldatum = DateTime.Now.AddDays(-2)
            };

            var request = new BestellingGeplaatstEvent(bestelling, NameConstants.BestelServiceBestellingGeplaatstEvent);

            // Act
            _target.ReceiveBestellingGeplaatstEvent(request);
            bestelling.Factuurnummer = 4;
            _target.ReceiveBestellingGeplaatstEvent(request);

            // Assert
            var queue = _nijnContext.EventBus.Queues[queueName];
            var bestellingResult = JsonConvert.DeserializeObject<BestellingGeaccrediteerdEvent>(queue[0].Message);
            Assert.AreEqual(1, queue.MessageQueueLength);
            Assert.AreEqual(3, bestellingResult.Factuurnummer);

            var dbResult = _dbContext.Bestellingen.SingleOrDefault(b => b.Factuurnummer == 3);
            Assert.AreEqual(bestelling.FactuurTotaalInclBtw, dbResult.FactuurTotaalInclBtw);
            Assert.AreEqual(bestelling.BestelStatus.ToString(), dbResult.BestelStatus.ToString());
            Assert.AreEqual(3, dbResult.Factuurnummer);
            Assert.AreEqual(bestelling.Klantnummer, dbResult.Klantnummer);
            Assert.AreEqual(bestelling.Besteldatum, dbResult.Besteldatum);
        }

    }
}
