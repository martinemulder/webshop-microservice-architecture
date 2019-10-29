using HalfWerk.CommonModels;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.CommonModels.PcsBetaalService.Models;
using HalfWerk.CommonModels.PcsBetalingService;
using HalfWerk.CommonModels.PcsBetalingService.Models;
using HalfWerk.PcsBetaalService.Controllers;
using HalfWerk.PcsBetaalService.DAL;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
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
using System.Text;

namespace HalfWerk.PcsBetaalService.Test.Intergration
{
    [TestClass]
    public class BetalingControllerIntergrationTest
    {
        private SqliteConnection _connection;
        private BetaalContext _dbContext;
        private ITestBusContext _nijnContext;

        private BetalingController _target;
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


            _target = new BetalingController(betalingVerwerkenService);

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
                    .SetKlantnummer(1)
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
        public void HandleVerWerkBetalingCommand_ShouldInsertNewBetaling()
        {
            // Arrange
            var betalingCM = new BetalingCM(1, 100);
            var command = new VerwerkBetalingCommand(betalingCM, NameConstants.BetaalServiceBetalingVerwerkenCommandQueue);
            
            // Act
            _target.HandleVerWerkBetalingCommand(command);

            // Assert
            var dbResult = _dbContext.Betalingen.SingleOrDefault(b => b.Factuurnummer == 1);
            Assert.AreEqual(betalingCM.Bedrag, dbResult.Bedrag);
            Assert.AreEqual(betalingCM.Factuurnummer, dbResult.Factuurnummer);
            Assert.AreEqual(1, dbResult.Klantnummer);
        }

        [TestMethod]
        public void HandleVerWerkBetalingCommand_ShouldInsertNewBetalingAndPublishEvent()
        {
            // Arrange
            var queueName = "TestQueue";
            var receiver = _nijnContext.CreateMessageReceiver(queueName, new List<string> { NameConstants.BetaalServiceBetalingGeaccrediteerdEvent });
            receiver.DeclareQueue();

            var betalingCM = new BetalingCM(1, 200);
            var command = new VerwerkBetalingCommand(betalingCM, NameConstants.BetaalServiceBetalingVerwerkenCommandQueue);

            // Act
            _target.HandleVerWerkBetalingCommand(command);

            // Assert
            var queue = _nijnContext.EventBus.Queues[queueName];
            var bestellingResult = JsonConvert.DeserializeObject<BestellingGeaccrediteerdEvent>(queue[0].Message);
            Assert.AreEqual(1, queue.MessageQueueLength);
            Assert.AreEqual(1, bestellingResult.Factuurnummer);

            var dbResult = _dbContext.Betalingen.SingleOrDefault(b => b.Factuurnummer == 1);
            Assert.AreEqual(betalingCM.Bedrag, dbResult.Bedrag);
            Assert.AreEqual(betalingCM.Factuurnummer, dbResult.Factuurnummer);
            Assert.AreEqual(1, dbResult.Klantnummer);
        }
    }
}
