using HalfWerk.BffWebshop.Controllers;
using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Helpers;
using HalfWerk.BffWebshop.Test.Entities.Bestellingen;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using HalfWerk.CommonModels.DsBestelService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn;
using Minor.Nijn.TestBus;
using Minor.Nijn.WebScale.Commands;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HalfWerk.BffWebshop.Entities;
using Afleveradres = HalfWerk.CommonModels.BffWebshop.BestellingService.Afleveradres;
using Bestelling = HalfWerk.CommonModels.BffWebshop.BestellingService.Bestelling;
using BestelRegel = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelRegel;
using BestelStatus = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelStatus;
using ContactInfo = HalfWerk.CommonModels.BffWebshop.BestellingService.ContactInfo;

namespace HalfWerk.BffWebshop.Test.Controllers
{
    [TestClass]
    public class BestellingControllerIntegrationTest
    {
        private BevestigdeBestellingBuilder _bestellingBuilder;
        private SqliteConnection _connection;
        private BffContext _dbContext;
        private ITestBusContext _nijnContext;
        private Mock<IJwtHelper> _jwtHelperMock;

        private BestellingController _target;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            ConfigAutoMapper.Initialize();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _bestellingBuilder = new BevestigdeBestellingBuilder();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<BffContext>()
                .UseSqlite(_connection)
                .Options;

            _dbContext = new BffContext(options);
            _dbContext.Database.EnsureCreated();

            var bestellingDataMapper = new BestellingDataMapper(_dbContext);
            var klantDataMapper = new KlantDataMapper(_dbContext);
            var magazijnSessionDataMapper = new MagazijnSessionDataMapper(_dbContext);

            _jwtHelperMock = new Mock<IJwtHelper>(MockBehavior.Strict);
            _nijnContext = new TestBusContextBuilder().CreateTestContext();
          
            _target = new BestellingController(
                new CommandPublisher(_nijnContext), 
                bestellingDataMapper, 
                klantDataMapper, 
                magazijnSessionDataMapper,
                _jwtHelperMock.Object,
                new LoggerFactory()
            );

            SeedDatabase();
        }

        [TestCleanup]
        public void AfterEach()
        {
            _dbContext.Dispose();
            _connection.Dispose();
        }

        private void SeedDatabase()
        {
            var date = new DateTime(2018, 11, 10, 10, 42,00);

            var bestellingen = new List<BevestigdeBestelling>
            {
                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(1)
                    .SetBestelStatus(BestelStatus.Geplaatst)
                    .Create(),

                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(2)
                    .SetBestelStatus(BestelStatus.Goedgekeurd)
                    .SetBesteldatum(date)
                    .Create(),

                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(3)
                    .SetBestelStatus(BestelStatus.Goedgekeurd)
                    .SetBesteldatum(date.AddMinutes(1))
                    .Create(),

                _bestellingBuilder.SetDummy()
                    .SetFactuurnummer(4)
                    .SetBestelStatus(BestelStatus.Goedgekeurd)
                    .SetBesteldatum(date.AddDays(10))
                    .Create(),
            };

            _dbContext.AddRange(bestellingen);
            _dbContext.SaveChanges();
        }
        
        [TestMethod]
        public async Task GetNextBestelling_ShouldReturnOldestBestellingenWithStatusGoedgekeurd()
        {
            long expectedFactuurnummer = 2;

            var receiver = _nijnContext.CreateCommandReceiver(NameConstants.BestelServiceUpdateBestelStatusCommandQueue);
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(request => new ResponseCommandMessage($"{expectedFactuurnummer}", "Long", request.CorrelationId));

            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("Email");

            var result = await _target.GetNextBestelling();

            var queue = _nijnContext.CommandBus.Queues[NameConstants.BestelServiceUpdateBestelStatusCommandQueue];

            _jwtHelperMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, queue.CalledTimes);
            Assert.AreEqual(expectedFactuurnummer, result.Value.Factuurnummer);
        }

        [TestMethod]
        public async Task GetNextBestelling_ShouldReturnBestellingFromSessionWhenExists()
        {
            long expectedFactuurnummer = 2;

            var session = new MagazijnSessionEntity { MedewerkerEmail = "Email", Factuurnummer = expectedFactuurnummer };
            _dbContext.MagazijnSessions.Add(session);
            _dbContext.SaveChanges();

            var receiver = _nijnContext.CreateCommandReceiver(NameConstants.BestelServiceUpdateBestelStatusCommandQueue);
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(request => new ResponseCommandMessage($"{expectedFactuurnummer}", "Long", request.CorrelationId));

            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("Email");

            var result = await _target.GetNextBestelling();

            var queue = _nijnContext.CommandBus.Queues[NameConstants.BestelServiceUpdateBestelStatusCommandQueue];

            _jwtHelperMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, queue.CalledTimes);
            Assert.AreEqual(expectedFactuurnummer, result.Value.Factuurnummer);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_ShouldSendUpdateBestelStatusCommand()
        {
            var factuurnummer = 10;   
            var status = new UpdateBestelStatus { Status = BestelStatus.Afgerond };

            var receiver = _nijnContext.CreateCommandReceiver(NameConstants.BestelServiceUpdateBestelStatusCommandQueue);
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(request => new ResponseCommandMessage("10", "Long", request.CorrelationId));

            var result = await _target.UpdateBestelStatus(factuurnummer, status);

            var queue = _nijnContext.CommandBus.Queues[NameConstants.BestelServiceUpdateBestelStatusCommandQueue];

            var okResult = result as OkResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, queue.CalledTimes);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_ShouldRemoveSessionFromDatabaseWhenBestelStatusIsVerzonden()
        {
            var factuurnummer = 10;
            var status = new UpdateBestelStatus { Status = BestelStatus.Verzonden };

            var session = new MagazijnSessionEntity { MedewerkerEmail = "Email", Factuurnummer = factuurnummer };
            _dbContext.MagazijnSessions.Add(session);
            _dbContext.SaveChanges();

            var receiver = _nijnContext.CreateCommandReceiver(NameConstants.BestelServiceUpdateBestelStatusCommandQueue);
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(request => new ResponseCommandMessage("10", "Long", request.CorrelationId));

            var result = await _target.UpdateBestelStatus(factuurnummer, status);

            var queue = _nijnContext.CommandBus.Queues[NameConstants.BestelServiceUpdateBestelStatusCommandQueue];

            var okResult = result as OkResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, queue.CalledTimes);
            Assert.AreEqual(0, _dbContext.MagazijnSessions.Count());
        }

        [TestMethod]
        public async Task CreateBestelling_ShouldCreateNewBestelling()
        {
            var klant = new Klant
            {
                Voornaam = "Pieter",
                Achternaam = "Pas",
                Email = "pieter@pas.nl",
                Telefoonnummer = "06123456789",
                Adres = new Adres
                {
                    Straatnaam = "Schepen Canishof",
                    Huisnummer = "71",
                    Postcode = "6831 HJ",
                    Plaats = "Arnhem",
                    Land = "Nederland"
                }
            };

            _dbContext.KlantEntities.Add(klant);
            _dbContext.SaveChanges();
            
            var bestelling = new Bestelling
            {
                ContactInfo = new ContactInfo
                {
                    Naam = $"{klant.Voornaam} {klant.Achternaam}",
                    Email = klant.Email,
                    Telefoonnummer = klant.Telefoonnummer
                },
                Afleveradres = new Afleveradres
                {
                    Adres = $"{klant.Adres.Straatnaam} {klant.Adres.Huisnummer}",
                    Plaats = klant.Adres.Plaats,
                    Postcode = klant.Adres.Postcode,
                    Land = klant.Adres.Land
                },
                BestelRegels = new List<BestelRegel>
                {
                    new BestelRegel
                    {
                        Artikelnummer = 10,
                        Aantal = 5
                    }
                }
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);

            PlaatsBestellingCommand commandResult = null;
            var receiver = _nijnContext.CreateCommandReceiver(NameConstants.BestelServicePlaatsBestellingCommandQueue);
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(request =>
            {
                commandResult = JsonConvert.DeserializeObject<PlaatsBestellingCommand>(request.Message);
                return new ResponseCommandMessage("10", "Long", request.CorrelationId);
            });

            var result = await _target.CreateBestelling(bestelling);

            var queue = _nijnContext.CommandBus.Queues[NameConstants.BestelServicePlaatsBestellingCommandQueue];

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(1, queue.CalledTimes);

            Assert.IsNotNull(commandResult, "commandResult != null");
            var bestellingResult = commandResult.Bestelling;
            Assert.AreEqual(bestelling.ContactInfo.Naam, bestellingResult.ContactInfo.Naam);
            Assert.AreEqual(bestelling.ContactInfo.Email, bestellingResult.ContactInfo.Email);
            Assert.AreEqual(bestelling.ContactInfo.Telefoonnummer, bestellingResult.ContactInfo.Telefoonnummer);

            Assert.AreEqual(bestelling.Afleveradres.Adres, bestellingResult.Afleveradres.Adres);
            Assert.AreEqual(bestelling.Afleveradres.Plaats, bestellingResult.Afleveradres.Plaats);
            Assert.AreEqual(bestelling.Afleveradres.Postcode, bestellingResult.Afleveradres.Postcode);
            Assert.AreEqual(bestelling.Afleveradres.Land, bestellingResult.Afleveradres.Land);

            Assert.AreEqual(1, bestellingResult.BestelRegels.Count);
            Assert.IsTrue(bestellingResult.BestelRegels.Any(r => r.Artikelnummer == 10 && r.Aantal == 5));
        }
    }
}