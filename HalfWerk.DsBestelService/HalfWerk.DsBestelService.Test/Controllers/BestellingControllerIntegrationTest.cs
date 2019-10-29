using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsBestelService;
using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.Controllers;
using HalfWerk.DsBestelService.DAL;
using HalfWerk.DsBestelService.DAL.DataMappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.TestBus;
using Minor.Nijn.WebScale.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfWerk.DsBestelService.Test.Controllers
{
    [TestClass]
    public class BestellingControllerIntegrationTest
    {
        private SqliteConnection _connection;
        private DbContextOptions<BestelContext> _options;
        private ITestBusContext _nijnHost;

        private BestellingController _target;
        private BestelContext _context;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            AutoMapperConfiguration.Configure();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<BestelContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _nijnHost = new TestBusContextBuilder().CreateTestContext();

            _context = new BestelContext(_options);
            _context.Database.EnsureCreated();
            SeedDatabase();

            var bestellingDataMapper = new BestellingDataMapper(_context);
            var artikelDataMapper = new ArtikelDataMapper(_context);
            var eventPublisher = new EventPublisher(_nijnHost);

            _target = new BestellingController(
                bestellingDataMapper: bestellingDataMapper, 
                artikelDataMapper: artikelDataMapper, 
                eventPublisher: eventPublisher, 
                loggerFactory: new LoggerFactory()
            );
        }

        private void SeedDatabase()
        {
            _context.Artikelen.AddRange(new List<Artikel>
            {
                new Artikel
                {
                    Artikelnummer = 1,
                    Leveranciercode = "abc-123-xyz",
                    Naam = "Fietsband",
                    Prijs = 12.95m,
                    LeverbaarTot = new DateTime(2018, 10, 11)
                }
            });

            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void ShouldHandlePlaatsBestellingCommand()
        {
            var queueName = "PlaatsOrderQueue";
            _nijnHost.EventBus.DeclareQueue(queueName, new List<string> { NameConstants.BestelServiceBestellingGeplaatstEvent });

            var bestellingCM = new BestellingCM
            {
                Klantnummer = 1234,
                ContactInfo = new ContactInfoCM
                {
                    Naam = "Pieter Pas",
                    Email = "pieter@pas.nl",
                    Telefoonnummer = "1234567890"
                },
                Afleveradres = new AfleveradresCM
                {
                    Adres = "Straat 1",
                    Plaats = "Plaats",
                    Postcode = "1234 AB",
                    Land = "Nederland"
                },
                BestelRegels = new List<BestelRegelCM>
                {
                    new BestelRegelCM
                    {
                        Artikelnummer = 1,
                        Aantal = 42,
                    }
                }
            };

            var receiver = _nijnHost.CreateCommandReceiver(NameConstants.MagazijnServiceCommandQueue);
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(request => new ResponseCommandMessage(JsonConvert.SerializeObject(true), "boolean", request.CorrelationId));

            var requestCommand = new PlaatsBestellingCommand(bestellingCM, NameConstants.BestelServicePlaatsBestellingCommandQueue);

            _target.HandlePlaatsBestelling(requestCommand);

            var result = _context.Bestellingen.Include(b => b.BestelRegels).SingleOrDefault(b => b.Id == 1);

            Assert.IsNotNull(result, "result != null");
            Assert.AreEqual(1234, result.Klantnummer);
            Assert.AreEqual(DateTime.Now.Date, result.Besteldatum.Date);
            Assert.AreEqual(1, result.Factuurnummer);
            Assert.AreEqual(BestelStatus.Geplaatst, result.BestelStatus);

            Assert.AreEqual(bestellingCM.ContactInfo.Naam, result.ContactInfo.Naam);
            Assert.AreEqual(bestellingCM.ContactInfo.Email, result.ContactInfo.Email);
            Assert.AreEqual(bestellingCM.ContactInfo.Telefoonnummer, result.ContactInfo.Telefoonnummer);

            Assert.AreEqual(bestellingCM.Afleveradres.Adres, result.Afleveradres.Adres);
            Assert.AreEqual(bestellingCM.Afleveradres.Postcode, result.Afleveradres.Postcode);
            Assert.AreEqual(bestellingCM.Afleveradres.Plaats, result.Afleveradres.Plaats);
            Assert.AreEqual(bestellingCM.Afleveradres.Land, result.Afleveradres.Land);

            Assert.AreEqual(1, result.BestelRegels.Count);
            Assert.IsTrue(result.BestelRegels.Any(b => b.Id == 1 && b.Artikelnummer == 1 && b.Naam == "Fietsband" && b.PrijsExclBtw == 12.95m), "Should contain BestelRegel");

            Assert.AreEqual(1, _nijnHost.EventBus.Queues[queueName].MessageQueueLength);
        }
    }
}
