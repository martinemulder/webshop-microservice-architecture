using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.CommonModels.PcsBetaalService;
using HalfWerk.DsBestelService.DAL;
using HalfWerk.DsBestelService.DAL.DataMappers;
using HalfWerk.DsBestelService.Listeners;
using HalfWerk.DsBestelService.Test.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.TestBus;
using Minor.Nijn.WebScale.Events;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.DsBestelService.Test.Listeners
{
    [TestClass]
    public class EventListenerIntegrationTest
    {
        private SqliteConnection _connection;
        private BestelContext _dbContext;
        private ITestBusContext _nijnContext;

        private EventListener _target;
        private BestellingBuilder _bestellingBuilder;

        [TestInitialize]
        public void BeforeEach()
        {
            _bestellingBuilder = new BestellingBuilder();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<BestelContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _nijnContext = new TestBusContextBuilder().CreateTestContext();

            _dbContext = new BestelContext(options);
            _dbContext.Database.EnsureCreated();
            SeedDatabase();

            var artikelDataMapper = new ArtikelDataMapper(_dbContext);
            var bestellingDataMapper = new BestellingDataMapper(_dbContext);
            var eventPublisher = new EventPublisher(_nijnContext);

            _target = new EventListener(artikelDataMapper, bestellingDataMapper, eventPublisher);

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
                    .Create(),

                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(2)
                    .SetBestelStatus(BestelStatus.Goedgekeurd)
                    .Create()
            });

            _dbContext.SaveChanges();
        }

        [TestMethod]
        public void HandleArtikelToegevoegdEvent_ShouldAddArtikelToDatabase()
        {
            var artikel = new Artikel { Artikelnummer = 1, Leveranciercode = "abc-xyz-1", Naam = "Fietsband", Prijs = 10.99m };
            var request = new EventMessage(
                routingKey: NameConstants.CatalogusServiceCategorieAanCatalogusToegevoegdEvent,
                message: JsonConvert.SerializeObject(artikel)
            );

            _target.HandleArtikelToegevoegdEvent(request);

            var dbResult = _dbContext.Artikelen.SingleOrDefault(a => a.Artikelnummer == 1);
            Assert.AreEqual(artikel.Artikelnummer, dbResult.Artikelnummer);
            Assert.AreEqual(artikel.Leveranciercode, dbResult.Leveranciercode);
            Assert.AreEqual(artikel.Naam, dbResult.Naam);
            Assert.AreEqual(artikel.Prijs, dbResult.Prijs);
        }

        [TestMethod]
        public void HandleBetalingGeaccrediteerdEvent_ShouldSetBestelStatusToGoedGekeurd()
        {
            var queueName = "TestQueue";
            var receiver = _nijnContext.CreateMessageReceiver(queueName, new List<string> { NameConstants.BestelServiceBestelStatusUpgedateEvent });
            receiver.DeclareQueue();

            var request = new BestellingGeaccrediteerdEvent(1, NameConstants.BetaalServiceBetalingGeaccrediteerdEvent);

            _target.HandleBetalingGeaccrediteerdEvent(request);

            var queue = _nijnContext.EventBus.Queues[queueName];
            var bestellingResult = JsonConvert.DeserializeObject<BestelStatusBijgewerktEvent>(queue[0].Message);
            Assert.AreEqual(1, queue.MessageQueueLength);
            Assert.AreEqual(1, bestellingResult.Bestelling.Factuurnummer);
            Assert.AreEqual(BestelStatus.Goedgekeurd, bestellingResult.Bestelling.BestelStatus);

            var dbResult = _dbContext.Bestellingen.SingleOrDefault(b => b.Id == 1);
            Assert.AreEqual(1, dbResult.Factuurnummer);
            Assert.AreEqual(BestelStatus.Goedgekeurd, dbResult.BestelStatus);
        }
    }
}