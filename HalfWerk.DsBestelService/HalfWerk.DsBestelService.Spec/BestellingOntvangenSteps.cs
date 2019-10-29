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
using Minor.Nijn.WebScale.Events;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using TechTalk.SpecFlow;

namespace HalfWerk.DsBestelService.Spec
{
    [Binding]
    public class BestellingOntvangenSteps
    {
        private DbConnection _connection;
        private BestelContext _context;

        [BeforeFeature()]
        public static void BeforeFeature()
        {
            AutoMapperConfiguration.Configure();
        }

        [BeforeScenario()]
        public void BeforeScenario()
        {
            SpecFlowTestLock.Lock();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<BestelContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new BestelContext(options);
            _context.Database.EnsureCreated();

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Artikelen.AddRange(new List<Artikel>
            {
                new Artikel
                {
                    Artikelnummer = 1,
                    Leveranciercode = "BAT-001-H ",
                    Naam = "Batavus Heren Fiets Blauw",
                    Prijs = 799.00m,
                },
                new Artikel
                {
                    Artikelnummer = 2,
                    Leveranciercode = "POC-BL-009",
                    Naam = "POC Blauwe Fietshelm",
                    Prijs = 117.9m,
                }
            });

            _context.SaveChanges();
        }

        [AfterScenario()]
        public void AfterScenario()
        {
            _context.Dispose();
            _connection.Dispose();

            SpecFlowTestLock.Unlock();
        }

        private int _klantnummer;
        private DateTime _besteldatum;
        private List<Tuple<long, string, string, decimal, int>> _bestelregels = new List<Tuple<long, string, string, decimal, int>>();

        [Given(@"er is bestelling binnen gekomen:")]
        public void GivenErIsBestellingBinnenGekomen(Table table)
        {
            Assert.IsTrue(table.Rows[0].TryGetValue("Klantnummer", out string klantnummer));

            Assert.IsTrue(Int32.TryParse(klantnummer, out _klantnummer));
        }

        [When(@"de bestelling bestelregels bevat:")]
        public void WhenDeBestellingBestelregelsBevat(Table table)
        {
            Assert.IsTrue(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                string artikelnummer;
                Assert.IsTrue(row.TryGetValue("Artikelnummer", out artikelnummer));

                string leverancierCode;
                Assert.IsTrue(row.TryGetValue("LeverancierCode", out leverancierCode));

                string naam;
                Assert.IsTrue(row.TryGetValue("Naam", out naam));

                string prijs;
                Assert.IsTrue(row.TryGetValue("Prijs", out prijs));

                string aantal;
                Assert.IsTrue(row.TryGetValue("Aantal", out aantal));

                _bestelregels.Add(new Tuple<long, string, string, decimal, int>(Int64.Parse(artikelnummer), leverancierCode,
                    naam, Decimal.Parse(prijs), Int32.Parse(aantal)));
            }

            Assert.AreEqual(table.Rows.Count, _bestelregels.Count);
        }

        [Then(@"verwerk een bestelling in het systeem met het klantnummer, besteldatum en bestelregels")]
        public void ThenVerwerkEenBestellingInHetSysteemMetHetKlantnummerBesteldatumEnBestelregels()
        {
            var bestellingDataMapper = new BestellingDataMapper(_context);
            var artikelDataMapper = new ArtikelDataMapper(_context);
            var eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
            var commandSenderMock = new Mock<ICommandSender>(MockBehavior.Strict);

            var response = new ResponseCommandMessage(JsonConvert.SerializeObject(true), "type", "correlationId");
            eventPublisherMock.Setup(p => p.Publish(It.IsAny<DomainEvent>()));
            commandSenderMock.Setup(sendr => sendr.SendCommandAsync(It.IsAny<RequestCommandMessage>())).ReturnsAsync(response);

            var controller = new BestellingController(bestellingDataMapper, artikelDataMapper, eventPublisherMock.Object, new LoggerFactory());

            var bestelling = new BestellingCM
            {
                Klantnummer = _klantnummer,
                BestelRegels = new List<BestelRegelCM>()                
            };

            foreach (var regel in _bestelregels)
            {
                bestelling.BestelRegels.Add(new BestelRegelCM()
                {
                    Artikelnummer = regel.Item1,
                    Aantal = regel.Item5
                });
            }

            var result = controller.HandlePlaatsBestelling(new PlaatsBestellingCommand(bestelling, ""));

            eventPublisherMock.VerifyAll();
            Assert.AreEqual(1, result);
        }
        
        [Then(@"genereer een factuurnummer")]
        public void ThenGenereerEenFactuurnummer()
        {
            var dataMapper = new BestellingDataMapper(_context);
            var result = dataMapper.GetById(1);

            Assert.AreEqual(1, result.Factuurnummer);
            Assert.AreEqual(2, result.BestelRegels.Count);
            Assert.AreEqual(_klantnummer, result.Klantnummer);
            Assert.AreEqual(DateTime.Now.Date, result.Besteldatum.Date);
        }
    }
}
